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


	[Header("Wall")]
	[SerializeField] private float _wallBounciness = 5f;
	private bool _hasShotInAir = false;


	[Header("Movement")]
	[SerializeField] private float _speed = 2f;
	[SerializeField] private float _airControl = 40f;
	private bool _isGrounded = false;


	[Header("Jump")]
	[SerializeField] private float _jumpForce = 10f;
	[SerializeField] private float _jumpTime = 0.3f;
	private float _jumpTimeLeft = 0f;
	private bool _isJumping = false;


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
		_rayLength = 0.01f + _skinWidth;

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

		float direction = WallCheck();
		if (direction != 0)
		{
			if (_hasShotInAir)
			{
				StopJump();
				Vector2 velocity = new Vector2(direction * _wallBounciness, _rb.velocity.y / 4);
				_rb.velocity = velocity;
			}

			_gun.Reload();
			_hasShotInAir = false;
		}

		if (_isJumping)
		{
			if (_isGrounded || _jumpTimeLeft <= 0)
				StopJump();
			else
				AddJumpForce();
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////
	////////                                  Move                                  ////////
	////////////////////////////////////////////////////////////////////////////////////////
	public void Move(float moveInput)
	{
		if (!_isFiring)
		{
			if (_isGrounded)
				_rb.velocity = new Vector2(moveInput * _speed, _rb.velocity.y);
			// We can move a bit in the air, but we can accelerate only if velocity is less than ground max speed
			else if ((moveInput > 0 && _rb.velocity.x < _speed) || (moveInput < 0 && _rb.velocity.x > -_speed))
				_rb.AddForce(Vector2.right * moveInput * _airControl);
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////
	////////                                  Jump                                  ////////
	////////////////////////////////////////////////////////////////////////////////////////
	public void StartJump()
	{
		if (_isGrounded)
		{
			_isJumping = true;
			_isGrounded = false;
			_jumpTimeLeft = _jumpTime;
			_rb.AddForce(Vector3.up * _jumpForce * 15);
		}
	}

	private void AddJumpForce()
	{
		_rb.AddForce(Vector3.up * _jumpForce);
		_jumpTimeLeft -= Time.deltaTime;
	}

	public void StopJump()
	{
		_isJumping = false;
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
		for (int i = 0; i < _horizontalRayCount - 1; i++)
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
}
