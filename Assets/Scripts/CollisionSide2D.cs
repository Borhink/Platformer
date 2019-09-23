using UnityEngine;

public class ContactSide2D
{
    public bool right {get; private set;}
    public bool left {get; private set;}
    public bool top {get; private set;}
    public bool bottom {get; private set;}

    public ContactSide2D()
    {
        Reset();
    }

    public void Reset()
    {
        right = false;
        left = false;
        top = false;
        bottom = false;
    }
    
    public bool[] Calculate(Collider2D collider, ContactPoint2D[] contacts)
    {
        bool[] sides = new bool[5];
        Vector2 max = collider.bounds.max;
        Vector2 center = collider.bounds.center;
        Vector2 point = Vector2.zero;

        foreach(ContactPoint2D c in contacts)
        {
            point += (c.point - center);
        }

        float diagonalAngle = Mathf.Atan2(max.y - center.y, max.x - center.x) * 180 / Mathf.PI;
        float contactAngle = Mathf.Atan2(point.y, point.x) * 180 / Mathf.PI;
        
        if (contactAngle < 0)
            contactAngle = 360 + contactAngle;
        if (diagonalAngle < 0)
            diagonalAngle = 360 + diagonalAngle;

        right = (((contactAngle >= 360 - diagonalAngle) && (contactAngle <= 360)) || ((contactAngle <= diagonalAngle) && (contactAngle >= 0)));
        left = (((contactAngle >= 180 - diagonalAngle) && (contactAngle <= 180)) || ((contactAngle >= 180) && (contactAngle <= 180 + diagonalAngle)));
        top = (((contactAngle >= diagonalAngle) && (contactAngle <= 90)) || ((contactAngle >= 90) && (contactAngle <= 180 - diagonalAngle)));
        bottom = (((contactAngle >= 180 + diagonalAngle) && (contactAngle <= 270)) || ((contactAngle >= 270) && (contactAngle <= 360 - diagonalAngle)));

        return sides;
    }

    public int WallDirection()
    {
        if (right && !left)
            return 1;
        if (!right && left)
            return -1;
        return 0;
    }

    public int OppositeWallDirection()
    {
        return -WallDirection();
    }
}