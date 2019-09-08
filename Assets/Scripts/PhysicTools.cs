using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PhysicTools
{
    public struct RayCastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    static public bool GroundCheck(float skinWidth, int rayCount, Vector2 origin, float spacing, LayerMask groundMask)
    {
        float rayLength = 0.01f + skinWidth;

        for (int i = 0; i < rayCount; i++)
        {
            Vector2 rayOrigin = origin + (Vector2.right * spacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, groundMask);
            Debug.DrawRay(rayOrigin, Vector2.down * rayLength, Color.red);
            if (hit)
                return true;
        }
        return false;
    }

    // static public bool WallCheck(float skinWidth, int rayCount, Vector2 origin, float spacing, LayerMask groundMask)
    // {
    //     float rayLength = 0.1f + skinWidth;

    //     for (int i = 0; i < rayCount; i++)
    //     {
    //         Vector2 rayOrigin = origin + (Vector2.up * spacing * i);
    //         if (Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, groundMask))
    //         Debug.DrawRay(rayOrigin, Vector2.down * rayLength, Color.red);
    //         if (hit)
    //             return true;
    //     }
    //     return false;
    // }

    static public void UpdateRaycastOrigins(Bounds bounds, float skinWidth, ref RayCastOrigins raycastOrigins)
    {
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    static public void CalculateRaySpacing(Bounds bounds, float skinWidth,
                                            ref int horizontalRayCount, ref int verticalRayCount,
                                            ref float horizontalRaySpacing, ref float verticalRaySpacing)
    {
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }
}