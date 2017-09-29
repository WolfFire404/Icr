using System;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    [HideInInspector] public bool expanded = false;
    
    public readonly Vector2 BlockSize = new Vector2(1f,1f);
    
    [SerializeField] private Point _startPoint = new Point(0,0);
    [SerializeField] private Point _endPoint = new Point(20, 0);

    [SerializeField, HideInInspector] 
    public List<GridPositionInfo> chunkLayout = new List<GridPositionInfo>();
    
    public int Weight { get; set; }

    private void Awake()
    {
        if (Weight < 0 || Weight > 100)
            Weight = 1;
    }
    
    public Point StartPoint
    {
        get { return _startPoint; }
    }

    public Point EndPoint
    {
        get { return _endPoint; }
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

[Serializable]
public class GridPositionInfo
{
    public Point point;
    public GameObject asset = null;
    public bool hasCollider = false;

    public void SetAsset(GameObject asset)
    {
        this.asset = asset;
    }
}
