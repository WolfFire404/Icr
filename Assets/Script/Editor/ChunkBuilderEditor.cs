using System;
using UnityEditor;
using UnityEngine;
using System.Collections;


namespace Script.Editor
{
    [CustomEditor(typeof(ChunkBuilder))]
    public class ChunkBuilderEditor : UnityEditor.Editor
    {
        private void OnEnable()
        {
            ChunkBuilder chunkBuilder = (ChunkBuilder) target;
            chunkBuilder.LoadAllChunks();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            ChunkBuilder chunkBuilder = (ChunkBuilder) target;

            GUILayout.Label(chunkBuilder.LoadedPrefabs.ToString());
            
            if (chunkBuilder.InEditMode)
            {
                
                if(GUILayout.Button("Exit Edit Mode"))
                    chunkBuilder.ExitEditMode();
                else
                    DrawEditingGui(chunkBuilder);    
            }
            else if (!chunkBuilder.InEditMode)
            {
                DrawSetupMenu(chunkBuilder);

                if (GUILayout.Button("Create New Chunk"))
                    chunkBuilder.CreateNewChunk();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawSetupMenu(ChunkBuilder cb)
        {
            DrawChunkArray(cb);
        }

        private void DrawChunkArray(ChunkBuilder cb)
        {
            for (var i = 0; i < cb.Chunks.Count; i++)
            {
                GameObject t = cb.Chunks[i];
                Chunk chunk = t.GetComponent<Chunk>();

                if (chunk == null) continue;

                using (var h = new EditorGUILayout.HorizontalScope())
                {
                    chunk.expanded = EditorGUILayout.Foldout(chunk.expanded, chunk.name, true);
                    var guiStyle = new GUIStyle(GUI.skin.button) {fixedWidth = 50};

                    if (GUILayout.Button("Edit", guiStyle))
                    {
                        cb.EditChunk(i);
                    }
                }

                if (chunk.expanded)
                {
                    chunk.Weight = EditorGUILayout.IntSlider("Weight", chunk.Weight, 1, 100);
                }
            }
        }

        private void OnSceneGUI()
        {
            ChunkBuilder cb = (ChunkBuilder) target;
            if (!cb.InEditMode) return;

            var @event = GUIUtility.GetControlID(FocusType.Passive);

            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    MouseDown(cb, @event);
                    break;
                case EventType.MouseUp:
                    GUIUtility.hotControl = 0;
                    Event.current.Use();
                    break;
                case EventType.MouseDrag:
                    if (Event.current.button == 2) break;
                    GUIUtility.hotControl = @event;
                    break;
            }
        }

        private void MouseDown(ChunkBuilder cb, int @event)
        {
            if (OutsideSceneView(Event.current.mousePosition)) return;

            switch (Event.current.button)
            {
                    case 0:    // left mouse button - add collider or asset
                        GUIUtility.hotControl = @event;
                        Event.current.Use();

                        var position = ScreenPositionToWorldPoint(Event.current.mousePosition);
                        cb.PlaceBlockAtPosition(position);
                        break;
                    case 1:    // right mouse button - remove collider or asset
                        
                        break;
                    case 2:
                        break;
            }
            
        }

        private static bool OutsideSceneView(Vector2 position)
        {
             return position.x < 0 || position.y < 0 || position.y > SceneView.currentDrawingSceneView.maxSize.y || position
                .x > SceneView.currentDrawingSceneView.maxSize.x;
        }

        private static Vector3 ScreenPositionToWorldPoint(Vector2 mousePosition)
        {
            if(!SceneView.currentDrawingSceneView.in2DMode)
                throw new NotSupportedException("Scene needs to be 2D to work. Sorry dude.");
            
            Vector3 mousepos = new Vector3(mousePosition.x, mousePosition.y, 0);
            return SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(mousePosition);
        }

        private void DrawEditingGui(ChunkBuilder cb)
        {
            DrawChunkArray(cb);

            using (var h = new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Block Mode"))
                    cb.EditMode = EditModes.AddAssets;
                if (GUILayout.Button("Collider Mode"))
                    cb.EditMode = EditModes.AddCollision;
            }

            if (cb.Blocks == null || cb.Blocks.Count == 0) return;

            string[] options = new string[cb.Blocks.Count];
            for (int i = 0; i < options.Length; i++)
            {
                options[i] = cb.Blocks[i].name;
            }

            int selected = EditorGUILayout.Popup(0, options);
        }
    }
}