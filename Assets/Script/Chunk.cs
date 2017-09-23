using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chunk : MonoBehaviour
{
    [HideInInspector] public bool expanded = false;
    
    public readonly Vector2 BlockSize = new Vector2(1f,1f);
    [SerializeField] private Point _startPoint = new Point(0,0);
    [SerializeField] private Point _endPoint = new Point(20, 0);
    [Range(1,100), SerializeField] private int _weight = 1;

    public Point StartPoint
    {
        get { return _startPoint; }
    }

    public Point EndPoint
    {
        get { return _endPoint; }
    }

    public int Weight
    {
        get { return _weight; }
    }

    private void OnDrawGizmos()
    {
        Vector2 pos = _startPoint;
        pos.x += transform.position.x * BlockSize.x + BlockSize.x / 2;
        pos.y += transform.position.y * BlockSize.y - BlockSize.y / 2;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(pos,BlockSize);
        
        pos = _endPoint;
        pos.x += transform.position.x * BlockSize.x + BlockSize.x / 2;
        pos.y += transform.position.y * BlockSize.y - BlockSize.y / 2;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(pos,BlockSize);
    }
}

[System.Serializable]
public class Point
{
    public int x;
    public int y;

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
}