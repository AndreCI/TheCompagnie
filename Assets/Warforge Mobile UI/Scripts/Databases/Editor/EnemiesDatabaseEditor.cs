using UnityEngine;
using DuloGames.UI;
using UnityEditor;

namespace DuloGamesEditor.UI
{
    public class EnemiesDatabaseEditor
    {
        private static string GetSavePath()
        {
            return EditorUtility.SaveFilePanelInProject("New enemies database", "New enemies database", "asset", "Create a new enemy database.");
        }

        [MenuItem("Assets/Create/Databases/Enemies Database")]
        public static void CreateDatabase()
        {
            string assetPath = GetSavePath();
            EnemiesDatabase asset = ScriptableObject.CreateInstance("EnemiesDatabase") as EnemiesDatabase;  //scriptable object
            AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(assetPath));
            AssetDatabase.Refresh();
        }
    }
}