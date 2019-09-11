using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicEntity : MonoBehaviour
{
	public struct RayCastOrigins
	{
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}

	[Header("Physics")]
	[SerializeField] protected float _skinWidth = 0.1f;
	[SerializeField] protected int _horizontalRayCount = 5;
	[SerializeField] protected int _verticalRayCount = 5;
	[SerializeField] protected LayerMask _groundMask;

	protected bool _isGrounded = false;
	protected bool _isOnWall = false;

	protected BoxCollider2D _boxCol;
	protected Rigidbody2D _rb;
	protected RayCastOrigins _raycastOrigins;
	protected float _horizontalRaySpacing;
	protected float _verticalRaySpacing;
	private float _rayLength;

	protected virtual void Start()
	{
		_rb = GetComponent<Rigidbody2D>();
		_boxCol = GetComponent<BoxCollider2D>();
		_rayLength = 0.01f + _skinWidth;

		CalculateRaySpacing();
	}

	protected virtual void FixedUpdate()
	{
		UpdateRaycastOrigins();
		_isGrounded = GroundCheck();
		_isOnWall = WallCheck();
		Debug.Log(_isOnWall);
	}

	protected bool GroundCheck()
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

	protected bool WallCheck()
	{
		for (int i = 0; i < _horizontalRayCount; i++)
		{
			if (_rb.velocity.x <= 0)
			{
				Vector2 rayOrigin = _raycastOrigins.bottomLeft + (Vector2.up * _horizontalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.left, _rayLength, _groundMask);
				Debug.DrawRay(rayOrigin, Vector2.left * _rayLength, Color.red);

				if (hit)
					return (hit.point.x <= _boxCol.bounds.min.x + _skinWidth);
			}
			if (_rb.velocity.x >= 0)
			{
				Vector2 rayOrigin = _raycastOrigins.bottomRight + (Vector2.up * _verticalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right, _rayLength, _groundMask);
				Debug.DrawRay(rayOrigin, Vector2.right * _rayLength, Color.red);

				if (hit)
					return (hit.point.x >= _boxCol.bounds.max.x - _skinWidth);
			}
		}
		return false;
	}

	protected void UpdateRaycastOrigins()
	{
		Bounds bounds = _boxCol.bounds;
		bounds.Expand(_skinWidth * -2);

		_raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
		_raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
		_raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
		_raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
	}

	protected void CalculateRaySpacing()
	{
		Bounds bounds = _boxCol.bounds;
		bounds.Expand(_skinWidth * -2);

		_horizontalRayCount = Mathf.Clamp(_horizontalRayCount, 2, int.MaxValue);
		_verticalRayCount = Mathf.Clamp(_verticalRayCount, 2, int.MaxValue);

		_horizontalRaySpacing = bounds.size.y / (_horizontalRayCount - 1);
		_verticalRaySpacing = bounds.size.x / (_verticalRayCount - 1);
	}
}