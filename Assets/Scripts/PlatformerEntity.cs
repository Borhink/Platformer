using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerEntity : MonoBehaviour
{
	public struct RayCastOrigins
	{
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}

	[Header("Physics")]
	[SerializeField] protected float _skinWidth = 0.015f;
	[SerializeField] protected int _horizontalRayCount = 5;
	[SerializeField] protected int _verticalRayCount = 5;
	[SerializeField] protected LayerMask _groundMask;

	protected BoxCollider2D _boxCol;
	protected RayCastOrigins _raycastOrigins;
	protected float _horizontalRaySpacing;
	protected float _verticalRaySpacing;

	protected bool GroundCheck()
	{
		float rayLength = 0.01f + _skinWidth;

		for (int i = 0; i < _verticalRayCount; i++)
		{
			Vector2 rayOrigin = _raycastOrigins.bottomLeft + (Vector2.right * _verticalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, _groundMask);
			Debug.DrawRay(rayOrigin, Vector2.down * rayLength, Color.red);

			if (hit)
			{
				Vector2 direction = ((Vector2)hit.collider.bounds.center - hit.point).normalized;
				Debug.Log(direction);
				Debug.DrawRay(hit.point, direction, Color.blue);
				if (direction.y > 0 && Mathf.Abs(direction.y) >= Mathf.Abs(direction.x))
					return true;
				else
					return false;
			}
		}
		return false;
	}

	// protected bool WallCheck(float _skinWidth, int rayCount, Vector2 origin, float spacing, LayerMask groundMask)
	// {
	//	 float rayLength = 0.1f + _skinWidth;

	//	 for (int i = 0; i < rayCount; i++)
	//	 {
	//		 Vector2 rayOrigin = origin + (Vector2.up * spacing * i);
	//		 if (Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, groundMask))
	//		 Debug.DrawRay(rayOrigin, Vector2.down * rayLength, Color.red);
	//		 if (hit)
	//			 return true;
	//	 }
	//	 return false;
	// }

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