using UnityEngine;
using DuloGames.UI;
using UnityEditor;

namespace DuloGamesEditor.UI
{
    public class UIEffectDatabaseEditor
    {
        private static string GetSavePath()
        {
            return EditorUtility.SaveFilePanelInProject("New item database", "New item database", "asset", "Create a new item database.");
        }

        [MenuItem("Assets/Create/Databases/Effects Database")]
        public static void CreateDatabase()
        {
            string assetPath = GetSavePath();
            EffectDatabase asset = ScriptableObject.CreateInstance("EffectDatabase") as EffectDatabase;  //scriptable object
            AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(assetPath));
            AssetDatabase.Refresh();
        }
    }
}