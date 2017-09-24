using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[ExecuteInEditMode]
public class ChunkBuilder : MonoBehaviour
{
    public List<GameObject> Chunks { get; private set; }
    public List<GameObject> Blocks { get; private set; }
    private ChunkBuilderCurrentInfo _currentEditingChunk;
    public bool InEditMode = false;
    
    public int LoadedPrefabs { get; private set; }
    public EditModes EditMode { get; set; }

    private void Start()
    {
        if (Application.isEditor && Application.isPlaying)
        {
            ExitEditMode();
        }
    }

    private void LoadBlocks()
    {
        Blocks = Resources.LoadAll<GameObject>("Blocks/").ToList();
    }
    
    public void LoadAllChunks()
    {
        Chunks = new List<GameObject>();
        LoadedPrefabs = 0;
        bool loading = true;
        while (loading)
        {
            GameObject prefab = Resources.Load<GameObject>("Chunks/chunk" + ++LoadedPrefabs);

            if (prefab == null)
            {
                loading = false;
                LoadedPrefabs--;
            }
            else
            {
                Chunks.Add(prefab);
            }
        }
    }

    public void EditChunk(int index)
    {
        // Exit and save the current chunk if it is currently loaded...
        if(InEditMode)
            ExitEditMode();

        if (index < 0 || index >= Chunks.Count) // calm down friend!
            return;
        
        _currentEditingChunk.Reset(); // Just reset it to be sure.
        var chunk = Chunks[index];
        _currentEditingChunk.CurrentChunkIndex = index + 1;
        _currentEditingChunk.PrefabObject = chunk.gameObject;
        _currentEditingChunk.GoCurrentEditing =
            Instantiate(chunk.gameObject, new Vector3(0, 0, 0), Quaternion.identity);
        _currentEditingChunk.ChunkCurrentEditing = _currentEditingChunk.GoCurrentEditing.GetComponent<Chunk>();
        LoadBlocks();
        InEditMode = true;
    }

    public void ExitEditMode()
    {
        // Destroy the currently spawned gameobject...
        SaveCurrentChunk();
        DestroyCurrentChunk();

        _currentEditingChunk.Reset();
        LoadAllChunks(); // Is this even needed? This _should_ be a fix to a glitch I don't even know there is yet.
        InEditMode = false;
    }

    /// <summary>
    /// Literally does what the title says.
    /// </summary>
    public void CreateNewChunk()
    {
        // Create new chunk...
        var prefab = new GameObject(); 
        const string path = "Assets/Prefabs/Resources/Chunks/";
        _currentEditingChunk.PrefabObject = PrefabUtility.CreatePrefab(path + "chunk" + ++LoadedPrefabs + ".prefab", prefab);
        _currentEditingChunk.GoCurrentEditing = prefab;
        _currentEditingChunk.ChunkCurrentEditing = prefab.AddComponent<Chunk>();
        _currentEditingChunk.CurrentChunkIndex = LoadedPrefabs;

        prefab.name = "Chunk " + LoadedPrefabs;
        InEditMode = true;
        ExitEditMode();
        LoadAllChunks();
    }


    private void DestroyCurrentChunk()
    {
        if (!InEditMode || _currentEditingChunk.GoCurrentEditing == null)
            return;
        
        DestroyImmediate(_currentEditingChunk.GoCurrentEditing);
    }

    public void PlaceBlockAtPosition(Vector3 mousePosition)
    {
        var point = GetPointFromMousePosition(_currentEditingChunk.ChunkCurrentEditing, mousePosition);
        
    }

    private static Point GetPointFromMousePosition(Chunk chunk, Vector3 mousePosition)
    {
        if (chunk == null) return null;
        
        int x = Mathf.FloorToInt(mousePosition.x / chunk.BlockSize.x);
        int y = Mathf.FloorToInt(mousePosition.y / chunk.BlockSize.y);
        return new Point(x,y);
    }
    
    /// <summary>
    /// Saves the current edited chunk.
    /// </summary>
    /// <returns>False on fail, true on succession.</returns>
    private bool SaveCurrentChunk()
    {
        if (!InEditMode)
            return false;
        
        if (_currentEditingChunk.GoCurrentEditing == null || _currentEditingChunk.PrefabObject == null)
        {
            _currentEditingChunk.Reset();
            DestroyCurrentChunk();
            InEditMode = false;
            Debug.LogError("An unknown error has occurred. Please try again or request backup.");
            return false;
        }

        Debug.Log("Saved chunk" + _currentEditingChunk.CurrentChunkIndex);
        PrefabUtility.ReplacePrefab(_currentEditingChunk.GoCurrentEditing, _currentEditingChunk.PrefabObject);

        return true;
    }
}

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

public enum EditModes
{
    Remove,
    AddAssets,
    AddCollision
}
