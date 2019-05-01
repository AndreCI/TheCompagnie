using UnityEngine;
using DuloGames.UI;
using UnityEditor;

namespace DuloGamesEditor.UI
{
    public class CardDatabaseEditor
    {
        private static string GetSavePath()
        {
            return EditorUtility.SaveFilePanelInProject("New card database", "New card database", "asset", "Create a new card database.");
        }

        [MenuItem("Assets/Create/Databases/Card Database")]
        public static void CreateDatabase()
        {
            string assetPath = GetSavePath();
            CardDatabase asset = ScriptableObject.CreateInstance("CardDatabase") as CardDatabase;  //scriptable object
            AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(assetPath));
            AssetDatabase.Refresh();
        }
    }
}