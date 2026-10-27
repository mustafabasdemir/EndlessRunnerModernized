using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

// Handles loading data from the Asset Bundle to handle different themes for the game
public class ThemeDatabase
{
    static protected Dictionary<string, ThemeData> themeDataList;
    static public Dictionary<string, ThemeData> dictionnary { get { return themeDataList; } }

    static protected bool m_Loaded = false;
    static public bool loaded { get { return m_Loaded; } }

    static public ThemeData GetThemeData(string type)
    {
        ThemeData list;
        if (themeDataList == null || !themeDataList.TryGetValue(type, out list))
            return null;

        return list;
    }

    static public IEnumerator LoadDatabase()
    {
        // If not null the dictionary was already loaded.
        if (themeDataList == null)
        {
            themeDataList = new Dictionary<string, ThemeData>();


            var handle = Addressables.LoadAssetsAsync<ThemeData>("themeData", op =>
            {
                if (op != null && !themeDataList.ContainsKey(op.themeName))
                    themeDataList.Add(op.themeName, op);
            });

            yield return handle;

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError(
                    "ThemeDatabase: Addressables failed to load label 'themeData'. "
                    + "Build Addressables for Android before device builds. "
                    + handle.OperationException);
                m_Loaded = false;
                yield break;
            }

            m_Loaded = true;
        }

    }
}
