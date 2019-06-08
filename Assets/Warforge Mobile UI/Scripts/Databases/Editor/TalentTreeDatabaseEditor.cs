using UnityEngine;
using DuloGames.UI;
using UnityEditor;

namespace DuloGamesEditor.UI
{
    public class TalentTreeDatabaseEditor
    {
        private static string GetSavePath()
        {
            return EditorUtility.SaveFilePanelInProject("New TalentTreeDatabase database", "New TalentTreeDatabase database", "asset", "Create a new TalentTreeDatabase database.");
        }

        [MenuItem("Assets/Create/Databases/TalentTreeDatabase Database")]
        public static void CreateDatabase()
        {
            string assetPath = GetSavePath();
            TalentTreeDatabase asset = ScriptableObject.CreateInstance("TalentTreeDatabase") as TalentTreeDatabase;  //scriptable object
            AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(assetPath));
            AssetDatabase.Refresh();
        }
    }
}