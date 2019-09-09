using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Player : MonoBehaviour
{

	private Rigidbody2D _rb;
	private BoxCollider2D _boxCol;

	//Movement
	[SerializeField] private float _speed = 2f;
	[SerializeField] private float _airControl = 40f;
	private bool _isGrounded = false;
	private bool _isOnWall = false;

	//Jump
	[SerializeField] private float _jumpForce = 10f;
	[SerializeField] private float _jumpTime = 0.3f;
	private float _jumpTimeLeft = 0f;
	private bool _isJumping = false;

	//Gun
	[SerializeField] private Gun _gun = null;
	[SerializeField] private Transform _armPivot = null;
	[SerializeField] private Transform _gunHolder = null;
	private Vector2 _target;
	private bool _isFiring = false;

	[SerializeField] private float _skinWidth = 0.05f;
	private PhysicTools.RayCastOrigins _raycastOrigins;

	[SerializeField] private int _horizontalRayCount = 3;
	[SerializeField] private int _verticalRayCount = 3;
	private float _horizontalRaySpacing;
	private float _verticalRaySpacing;
	[SerializeField] private LayerMask _groundMask;


	void Start()
	{
		_rb = GetComponent<Rigidbody2D>();
		_boxCol = GetComponent<BoxCollider2D>();

		PhysicTools.CalculateRaySpacing(_boxCol.bounds, _skinWidth,
											ref _horizontalRayCount, ref _verticalRayCount,
											ref _horizontalRaySpacing, ref _verticalRaySpacing);
	}

	void FixedUpdate()
	{
		PhysicTools.UpdateRaycastOrigins(_boxCol.bounds, _skinWidth, ref _raycastOrigins);
		_isGrounded = PhysicTools.GroundCheck(_skinWidth, _verticalRayCount, _raycastOrigins.bottomLeft, _verticalRaySpacing, _groundMask);

        if (_gun && !_isFiring && _isGrounded)
			_gun.Reload();

		if (_isJumping)
		{
			if (_isGrounded || _jumpTimeLeft <= 0)
				StopJump();
			else
				AddJumpForce();
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

	IEnumerator FiringTimer()
	{
		 _isFiring = true;
		 yield return new WaitForSeconds(0.15f);
		 _isFiring = false;
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
				StartCoroutine("FiringTimer");
			}
		}
	}

	void Flip()
	{
		Vector3 Scaler = transform.localScale;
		Scaler.x *= -1;
		transform.localScale = Scaler;
	}
}
