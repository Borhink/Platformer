using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Player : MonoBehaviour
{
	public struct RayCastOrigins
	{
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}

	[Header("Raycasting")]
	[SerializeField] private float _skinWidth = 0.1f;
	[SerializeField] private int _horizontalRayCount = 5;
	[SerializeField] private int _verticalRayCount = 5;
	[SerializeField] private LayerMask _groundMask;
	private RayCastOrigins _raycastOrigins;
	private float _horizontalRaySpacing;
	private float _verticalRaySpacing;
	private float _rayLength;
	private BoxCollider2D _boxCol;
	private Rigidbody2D _rb;
	private bool _isGrounded = false;
	private Vector2 _lastVelocity = Vector2.zero;
	private Vector2 _velocityBeforeCollision = Vector2.zero;


	[Header("Wall")]
	[SerializeField] private float _wallBounciness = 5f;
	private bool _hasShotInAir = false;


	[Header("Gun")]
	[SerializeField] private Gun _gun = null;
	[SerializeField] private Transform _armPivot = null;
	[SerializeField] private Transform _gunHolder = null;
	private Vector2 _target;
	private bool _isFiring = false;

	void Start()
	{
		_rb = GetComponent<Rigidbody2D>();
		_boxCol = GetComponent<BoxCollider2D>();
		_rayLength = 0.02f + _skinWidth;

		CalculateRaySpacing();
	}

	void FixedUpdate()
	{
		UpdateRaycastOrigins();
		_isGrounded = GroundCheck();

		if (_isGrounded)
		{
			_hasShotInAir = false;

			if (_gun && !_isFiring)
				_gun.Reload();
		}

		if (!_isGrounded)
		{
			float direction = WallCheck();
			if (_hasShotInAir && direction != 0)
			{
				if (Mathf.Abs(_velocityBeforeCollision.x) > 4f)
				{
					Vector2 velocity = new Vector2(direction * _wallBounciness, _rb.velocity.y / 4);
					_rb.velocity = velocity;
					_velocityBeforeCollision = Vector2.zero;
				}
				_gun.Reload();
				_hasShotInAir = false;
			}
		}

		_lastVelocity = _rb.velocity;
	}

	////////////////////////////////////////////////////////////////////////////////////////
	////////                                  Gun                                   ////////
	////////////////////////////////////////////////////////////////////////////////////////
	IEnumerator FiringTimer()
	{
		 _isFiring = true;
		 yield return new WaitForSeconds(0.25f);
		 _isFiring = false;
		_hasShotInAir = true;
	}

	public void Fire()
	{
		if (_gun && !_isFiring)
		{
			Vector2 direction = (_target - (Vector2)_armPivot.position).normalized;
			Vector2 shotVelocity = Vector2.zero;

			if (_gun.Fire(direction, ref shotVelocity))
			{
				_rb.velocity = shotVelocity;
				StartCoroutine(FiringTimer());
			}
		}
	}

	public void Target(Vector2 target)
	{
		_target = target;
		if ((_target.x > transform.position.x && transform.localScale.x < 0) || (_target.x < transform.position.x && transform.localScale.x > 0))
			Flip();

		Vector2 directionToTarget = (_target - (Vector2)_armPivot.position).normalized;
		_armPivot.right = transform.localScale.x > 0 ? directionToTarget : -directionToTarget;
	}

	void Flip()
	{
		Vector3 Scaler = transform.localScale;
		Scaler.x *= -1;
		transform.localScale = Scaler;
	}

	////////////////////////////////////////////////////////////////////////////////////////
	////////                               Raycasting                               ////////
	////////////////////////////////////////////////////////////////////////////////////////
	bool GroundCheck()
	{
		for (int i = 0; i < _verticalRayCount; i++)
		{
			Vector2 rayOrigin = _raycastOrigins.bottomLeft + (Vector2.right * _verticalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, _rayLength, _groundMask);
			Debug.DrawRay(rayOrigin, Vector2.down * _rayLength, Color.red);

			if (hit)
				return (hit.point.y <= _boxCol.bounds.min.y + _skinWidth);
		}
		return false;
	}

	float WallCheck()
	{
		for (int i = 1; i < _horizontalRayCount - 1; i++)
		{
			if (_rb.velocity.x <= 0)
			{
				Vector2 rayOrigin = _raycastOrigins.bottomLeft + (Vector2.up * _horizontalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.left, _rayLength, _groundMask);
				Debug.DrawRay(rayOrigin, Vector2.left * _rayLength, Color.red);

				if (hit && hit.point.x <= _boxCol.bounds.min.x + _skinWidth)
					return 1;
			}
			if (_rb.velocity.x >= 0)
			{
				Vector2 rayOrigin = _raycastOrigins.bottomRight + (Vector2.up * _verticalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right, _rayLength, _groundMask);
				Debug.DrawRay(rayOrigin, Vector2.right * _rayLength, Color.red);

				if (hit && hit.point.x >= _boxCol.bounds.max.x - _skinWidth)
					return -1;
			}
		}
		return 0;
	}

	void UpdateRaycastOrigins()
	{
		Bounds bounds = _boxCol.bounds;
		bounds.Expand(_skinWidth * -2);

		_raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
		_raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
		_raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
		_raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
	}

	void CalculateRaySpacing()
	{
		Bounds bounds = _boxCol.bounds;
		bounds.Expand(_skinWidth * -2);

		_horizontalRayCount = Mathf.Clamp(_horizontalRayCount, 2, int.MaxValue);
		_verticalRayCount = Mathf.Clamp(_verticalRayCount, 2, int.MaxValue);

		_horizontalRaySpacing = bounds.size.y / (_horizontalRayCount - 1);
		_verticalRaySpacing = bounds.size.x / (_verticalRayCount - 1);
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		_velocityBeforeCollision = _lastVelocity;
	}
}
