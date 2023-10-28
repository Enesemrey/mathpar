using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(Level))]
public class LevelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var level = target as Level;

        if (GUILayout.Button("Generate"))
        {
            if (level.Instances != null)
            {
                foreach (var piece in level.Instances)
                {
                    if (piece == null) continue;
                    DestroyImmediate(piece.gameObject);
                }
                level.Instances.Clear();
            }
            else
            {
                level.Instances = new List<LevelPiece>();
            }

            var sum = level.transform.position;
            for (var i = 0; i < level.LevelPieces.Length; i++)
            {
                var pieceType = level.LevelPieces[i];
                if (level.Prefabs.Length <= (int)pieceType) continue;
                var piece = PrefabUtility.InstantiatePrefab(level.Prefabs[(int)pieceType], level.transform) as LevelPiece;
                piece.PieceType = pieceType;
                sum += Vector3.forward * piece.Length;
                piece.transform.position = sum;
                level.Instances.Add(piece);
            }

            EditorUtility.SetDirty(target);
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
    }
}