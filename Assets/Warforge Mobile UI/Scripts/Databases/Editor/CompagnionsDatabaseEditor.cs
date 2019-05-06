using UnityEngine;
using DuloGames.UI;
using UnityEditor;

namespace DuloGamesEditor.UI
{
    public class CompagnionsDatabaseEditor
    {
        private static string GetSavePath()
        {
            return EditorUtility.SaveFilePanelInProject("New Compagnions database", "New Compagnions database", "asset", "Create a new Compagnions database.");
        }

        [MenuItem("Assets/Create/Databases/Compagnions Database")]
        public static void CreateDatabase()
        {
            string assetPath = GetSavePath();
            CompagnionsDatabase asset = ScriptableObject.CreateInstance("CompagnionsDatabase") as CompagnionsDatabase;  //scriptable object
            AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(assetPath));
            AssetDatabase.Refresh();
        }
    }
}