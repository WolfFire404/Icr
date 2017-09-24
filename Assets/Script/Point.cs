using UnityEngine;

[System.Serializable]
public class Point
{
    public int x;
    public int y;
    
    public Point Zero { get { return new Point(0, 0); }}
    
    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    
    
    public static implicit operator Point(Vector2 vector2)
    {
        return new Point((int)vector2.x, (int)vector2.y);
    }

    public static implicit operator Vector2(Point point)
    {
        return new Vector2(point.x, point.y);
    }

    public override int GetHashCode()
    {
        return x.GetHashCode() ^ y.GetHashCode() << 2;
    }
}