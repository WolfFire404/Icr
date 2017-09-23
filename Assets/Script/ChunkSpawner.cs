using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkSpawner : MonoBehaviour
{
    private Queue<Chunk> _spawnedChunks;
    private UnityEngine.Object[] _possibleChunks;
    private Chunk _lastChunk;

    private Transform _playerPosition;
    

    private const float InitialSpawnX = -10;
    
    private void Start()
    {
        _spawnedChunks = new Queue<Chunk>();
        _possibleChunks = Resources.LoadAll("Chunks/");
        _playerPosition = GameObject.FindGameObjectWithTag("Player").transform;

        for (var i = 0; i < 5; i++)
        {
            SpawnChunk();
        }
    }

    private void Update()
    {
        if (CheckDeleteChunk())
        {
            DeleteFirstChunk();
            SpawnChunk();
        }
    }

    private bool CheckDeleteChunk()
    {
        if (_spawnedChunks.Count == 0 || _playerPosition == null) return false;

        var chunk = _spawnedChunks.Peek();
        float deletepos = chunk.transform.position.x + (chunk.EndPoint.x) * chunk.BlockSize.x + 
                          (chunk.EndPoint.x - chunk.StartPoint.x) * chunk.BlockSize.x;
        Debug.Log(deletepos);
        return _playerPosition.position.x > deletepos;
    }

    private void DeleteFirstChunk()
    {
        if (_spawnedChunks.Count == 0) return;
        Destroy(_spawnedChunks.Dequeue().gameObject,0);
    }

    private void SpawnChunk()
    {
        var spawnLocation = _lastChunk == null ? new Vector3(InitialSpawnX, 0) : GetSpawnLocation(_lastChunk);
        var o = _possibleChunks[Random.Range(0, _possibleChunks.Length)] as GameObject;
        
        if (o == null) return;

        var chunk = o.GetComponent<Chunk>();
        
        Vector3 origin = (Vector2)chunk.StartPoint;
        origin.x = (origin.x + 1) * chunk.BlockSize.x;
        origin.y *= -chunk.BlockSize.y;

        chunk = Instantiate(o, spawnLocation + origin, Quaternion.identity).GetComponent<Chunk>();

        _spawnedChunks.Enqueue(chunk);
        _lastChunk = chunk;
    }

    private static Vector3 GetSpawnLocation(Chunk chunk)
    {
        var location = chunk.transform.position;
        location.x += chunk.BlockSize.x * chunk.EndPoint.x;
        location.y += chunk.BlockSize.y * chunk.EndPoint.y;

        return location;
    }

}