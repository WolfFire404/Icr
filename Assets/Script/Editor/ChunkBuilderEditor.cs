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
                GUIStyle guiStyle;
                GameObject t = cb.Chunks[i];
                Chunk chunk = t.GetComponent<Chunk>();
                guiStyle = new GUIStyle(EditorStyles.foldout);

                using (var h = new EditorGUILayout.HorizontalScope())
                {
                    chunk.expanded = EditorGUILayout.Foldout(chunk.expanded, chunk.name, true);
                    guiStyle = new GUIStyle(GUI.skin.button) {fixedWidth = 50};

                    if (GUILayout.Button("Edit", guiStyle))
                    {
                        cb.EditChunk(i);
                    }
                }

                if (chunk.expanded)
                {
                    
                }
            }
        }

        private void DrawEditingGui(ChunkBuilder cb)
        {
            DrawChunkArray(cb);
        }
    }
}