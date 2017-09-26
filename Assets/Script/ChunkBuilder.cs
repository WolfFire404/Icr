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
    public int CurrentSelectedBlock { get; set; }

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
        if (InEditMode)
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
        _currentEditingChunk.PrefabObject =
            PrefabUtility.CreatePrefab(path + "chunk" + ++LoadedPrefabs + ".prefab", prefab);
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
        var chunk = _currentEditingChunk.ChunkCurrentEditing;
        var info = GetPositionInfo(point);

        if (info.asset != null) return; // If a block is already present, return.

        var block = Blocks[CurrentSelectedBlock];
        var blockPos = new Vector3(chunk.BlockSize.x * point.x + 0.5f,
            chunk.BlockSize.y * point.y - 0.5f, 0);
        var blockGo = (GameObject) Instantiate(block, blockPos, Quaternion.identity);
        blockGo.transform.parent = _currentEditingChunk.GoCurrentEditing.transform;
        info.SetAsset(blockGo);
    }

    public void RemoveBlockAtPosition(Vector3 mousePosition)
    {
        var chunk = _currentEditingChunk.ChunkCurrentEditing;
        var point = GetPointFromMousePosition(chunk, mousePosition);
        var info = GetPositionInfo(point);

        if (info.asset == null) return;

        DestroyImmediate(info.asset);
        info.SetAsset(null);
    }

    private GridPositionInfo GetPositionInfo(Point position)
    {
        var chunk = _currentEditingChunk.ChunkCurrentEditing;
        var info = chunk.chunkLayout.FirstOrDefault(x => x.point.Equals(position));

        if (info != null) return info;

        info = new GridPositionInfo {point = position};
        chunk.chunkLayout.Add(info);

        return info;
    }

    private static Point GetPointFromMousePosition(Chunk chunk, Vector3 mousePosition)
    {
        if (chunk == null) return null;

        int x = Mathf.FloorToInt(mousePosition.x / chunk.BlockSize.x);
        int y = Mathf.FloorToInt(mousePosition.y / chunk.BlockSize.y) + 1;

        return new Point(x, y);
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

    public void PlaceCollisionAtPosition(Vector3 position)
    {
        var chunk = _currentEditingChunk.ChunkCurrentEditing;
        var mousePosition = GetPointFromMousePosition(chunk, position);
        var info = GetPositionInfo(mousePosition);

        info.hasCollider = true;
    }

    public void GenerateColliders()
    {
        if (_currentEditingChunk.ChunkCurrentEditing == null
            || _currentEditingChunk.ChunkCurrentEditing.chunkLayout.Count == 0) return;

        var chunk = _currentEditingChunk.ChunkCurrentEditing;

        // check if object has a child named "cols"

        GameObject obj;
        if (TryGetChild(chunk.transform, "cols", out obj))
            DestroyImmediate(obj);

        obj = new GameObject("cols");
        obj.transform.parent = chunk.transform;

        bool[] chunkChecked = new bool[chunk.chunkLayout.Count];
        List<Vector3> colliderPositions = new List<Vector3>();
        List<Vector3> colliderSizes = new List<Vector3>();

        for (int i = 0; i < chunk.chunkLayout.Count; i++)
        {
            if (chunkChecked[i]) continue;

            var info = chunk.chunkLayout[i];

            if (!info.hasCollider) continue;
            
            var checkInfo = info;
            int leftStartPoint = info.point.x,
                collisionPoints = 1;
            float width = 1;

            //check left of block until it reaches an empty space.
            while (true)
            {
                int atIndex;
                checkInfo = GetPositionInfoAtPoint(chunk,
                    new Point(checkInfo.point.x - 1, checkInfo.point.y), out atIndex);

                if (checkInfo == null || !checkInfo.hasCollider) break;

                leftStartPoint = info.point.x;
                width += chunk.BlockSize.x;
                collisionPoints++;
                chunkChecked[atIndex] = true;
            }

            checkInfo = info;

            //Check right side
            while (true)
            {
                int atIndex;
                checkInfo = GetPositionInfoAtPoint(chunk,
                    new Point(checkInfo.point.x + 1, checkInfo.point.y), out atIndex);

                if (checkInfo == null || !checkInfo.hasCollider) break;

                collisionPoints++;
                width += chunk.BlockSize.x;
                chunkChecked[atIndex] = true;
            }

            float midpointx = (float)(leftStartPoint + leftStartPoint + collisionPoints) / 2;
            // magic number .873871 is because it was always offset by that amount for some reason..
            // idk why.
            float midpoint = transform.position.x + midpointx * chunk.BlockSize.x - 0.873871f;
            
            
            var collisionObj = new GameObject("Collision");
            collisionObj.AddComponent<BoxCollider2D>();
            collisionObj.transform.localScale = new Vector3(width, 1, 1);
            collisionObj.transform.position = new Vector3(midpoint,
                chunk.transform.position.y + chunk.chunkLayout[i].point.y * chunk.BlockSize.y - 0.5f, 1);
            collisionObj.transform.SetParent(obj.transform);

            chunkChecked[i] = true;
        }
    }

    private static GridPositionInfo GetPositionInfoAtPoint(Chunk chunk, Point point, out int atIndex)
    {
        for (int i = 0; i < chunk.chunkLayout.Count; i++)
        {
            if (!chunk.chunkLayout[i].point.Equals(point)) continue;
            atIndex = i;
            return chunk.chunkLayout[i];
        }

        atIndex = -1;
        return null;
    }

    private static bool TryGetChild(Transform transform, string name, out GameObject gameObject)
    {
        gameObject = null;

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name != name) continue;
            gameObject = transform.GetChild(i).gameObject;
            return true;
        }

        return false;
    }

    public void RemoveColliderAtPosition(Vector3 position)
    {
        var chunk = _currentEditingChunk.ChunkCurrentEditing;
        var mousePosition = GetPointFromMousePosition(chunk, position);
        var info = GetPositionInfo(mousePosition);

        info.hasCollider = false;
    }

    public void RemoveAllColliders()
    {
        foreach (var gridpos in _currentEditingChunk.ChunkCurrentEditing.chunkLayout)
        {
            gridpos.hasCollider = false;
        }
        
        GameObject obj;
        bool a = TryGetChild(_currentEditingChunk.ChunkCurrentEditing.transform, "cols", out obj);
        if (!a) return;
        DestroyImmediate(obj);
        obj = new GameObject("cols");
        obj.transform.SetParent(_currentEditingChunk.ChunkCurrentEditing.transform);
    }
}

public enum EditModes
{
    AddAssets,
    AddCollision
}