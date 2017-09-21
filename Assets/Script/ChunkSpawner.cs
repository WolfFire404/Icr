using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkSpawner : MonoBehaviour
{
    private Queue<Chunk> _spawnedChunks;
    private UnityEngine.Object[] _possibleChunks;
    private Chunk _lastChunk;

    private const float initialSpawnX = -10;
    
    private void Start()
    {
        _spawnedChunks = new Queue<Chunk>();
        _possibleChunks = Resources.LoadAll("Chunks/");

        for (var i = 0; i < 5; i++)
        {
            SpawnChunk();
        }
    }

    private void Update()
    {
        
    }

    private void SpawnChunk()
    {
        var spawnLocation = _lastChunk == null ? new Vector3(initialSpawnX, 0) : GetSpawnLocation(_lastChunk);
        
    }

    private static Vector3 GetSpawnLocation(Chunk chunk)
    {
        var location = chunk.transform.position;
        location.x += chunk.BlockSize.x * chunk.EndPoint.x;
        location.y += chunk.BlockSize.y * chunk.EndPoint.y;

        return location;
    }
}