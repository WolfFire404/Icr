using UnityEngine;

struct ChunkBuilderCurrentInfo
{
    public int CurrentChunkIndex;
    public Chunk ChunkCurrentEditing;
    public GameObject GoCurrentEditing;
    public GameObject PrefabObject;

    public void Reset()
    {
        ChunkCurrentEditing = null;
        GoCurrentEditing = PrefabObject = null;
        CurrentChunkIndex = 0;
    }
}