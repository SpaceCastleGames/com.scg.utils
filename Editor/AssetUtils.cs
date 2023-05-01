
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace scg.utils.editor
{
    public static class AssetUtils
    {

        public static List<T> GetAssets<T>() where T : UnityEngine.Object
        {
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            List<T> assets = new List<T>();
            if (guids.Length > 0)
            {
                foreach (var guid in guids)
                {
                    assets.Add(AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid)));
                }
                return assets;
            }
            return assets;
        }

        public static T GetAsset<T>() where T : UnityEngine.Object
        {
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            if (guids.Length > 0)
            {
                if (guids.Length > 1)
                {
                    Debug.LogWarning($"Found more than 1 of type: {typeof(T).Name}");
                }
                return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[0]));
            }
            return default;
        }

        public static T GetOrCreateAsset<T>(string createPath) where T : ScriptableObject
        {
            return GetOrCreateAsset<T>(createPath, typeof(T).Name+".asset");
        }

        public static T GetOrCreateAsset<T>(string createPath, string assetName) where T : ScriptableObject
        {
            var found = GetAsset<T>();
            if (found == null)
            {
                found = ScriptableObject.CreateInstance<T>();
                CreateFolderStructure(createPath);
                string path = Path.Combine(createPath, assetName);
                Debug.Log(path);
                AssetDatabase.CreateAsset(found, path);
            }
            return found;
        }

        public static void CreateFolderStructure(string path)
        {
            path = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            string[] folderList = path.Split(Path.DirectorySeparatorChar);

            //Check if folder exists with IsValidFolder if it doesn't create it
            for (int i = 0; i < folderList.Length; i++)
            {
                string currentFolder = folderList[i];
                string parentFolders = Path.Combine(folderList.Take(i).ToArray());
                parentFolders = $"{(string.IsNullOrWhiteSpace(parentFolders) ? "" : parentFolders)}";
                string validPath = $"{parentFolders}{Path.DirectorySeparatorChar}{currentFolder}";
                if (string.IsNullOrWhiteSpace(currentFolder) || AssetDatabase.IsValidFolder(validPath)) continue;
                AssetDatabase.CreateFolder(parentFolders, currentFolder);
            }
        }

       

    }
}