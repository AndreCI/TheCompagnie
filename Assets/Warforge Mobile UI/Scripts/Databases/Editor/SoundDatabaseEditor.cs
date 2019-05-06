using UnityEngine;
using DuloGames.UI;
using UnityEditor;

namespace DuloGamesEditor.UI
{
    public class SoundDatabaseEditor
    {
        private static string GetSavePath()
        {
            return EditorUtility.SaveFilePanelInProject("New Sound database", "New Sound database", "asset", "Create a new Sound database.");
        }

        [MenuItem("Assets/Create/Databases/Sound Database")]
        public static void CreateDatabase()
        {
            string assetPath = GetSavePath();
            SoundDatabase asset = ScriptableObject.CreateInstance("SoundDatabase") as SoundDatabase;  //scriptable object
            AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(assetPath));
            AssetDatabase.Refresh();
        }
    }
}