using UnityEngine;
using DuloGames.UI;
using UnityEditor;

namespace DuloGamesEditor.UI
{
    public class OverworldEventDatabaseEditor
    {
        private static string GetSavePath()
        {
            return EditorUtility.SaveFilePanelInProject("New OverworldEvent database", "New OverworldEvent database", "asset", "Create a new OverworldEvent database.");
        }

        [MenuItem("Assets/Create/Databases/OverworldEvent Database")]
        public static void CreateDatabase()
        {
            string assetPath = GetSavePath();
            OverworldEventDatabase asset = ScriptableObject.CreateInstance("OverworldEventDatabase") as OverworldEventDatabase;  //scriptable object
            AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(assetPath));
            AssetDatabase.Refresh();
        }
    }
}