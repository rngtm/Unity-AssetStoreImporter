using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace AssetStoreImporter
{
    internal class AssetStore
    {
        public static void ImportUnityPackage(string path)
        {
            var openPath = Path.Combine(GetAssetStoreDirectory(), path);
            Debug.Log(openPath);
            AssetDatabase.ImportPackage(openPath, true);
        }

        public static string GetAssetStoreDirectory()
        {
            string path = "";
            if (SystemInfo.operatingSystem.Contains("Windows")) // OSがWindowsの場合
            {
                path = InternalEditorUtility.unityPreferencesFolder + Path.DirectorySeparatorChar + "../../Asset Store-5.x";
            }
            else if (SystemInfo.operatingSystem.Contains("Mac")) // OSがMacの場合
            {
                path = InternalEditorUtility.unityPreferencesFolder + Path.DirectorySeparatorChar + "../../../Unity/Asset Store-5.x";
            }

            return path;
        }
    }
}
