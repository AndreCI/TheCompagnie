using UnityEngine;
using DuloGames.UI;
using UnityEditor;

namespace DuloGamesEditor.UI
{
    public class AnimationClipDatabaseEditor
    {
        private static string GetSavePath()
        {
            return EditorUtility.SaveFilePanelInProject("New AnimationClip database", "New AnimationClip database", "asset", "Create a new AnimationClip database.");
        }

        [MenuItem("Assets/Create/Databases/AnimationClip Database")]
        public static void CreateDatabase()
        {
            string assetPath = GetSavePath();
            AnimationClipDatabase asset = ScriptableObject.CreateInstance("AnimationClipDatabase") as AnimationClipDatabase;  //scriptable object
            AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(assetPath));
            AssetDatabase.Refresh();
        }
    }
}