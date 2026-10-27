using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This allows us to store a database of all characters currently in the bundles, indexed by name.
/// </summary>
public class CharacterDatabase
{
    static protected Dictionary<string, Character> m_CharactersDict;

    static public Dictionary<string, Character> dictionary {  get { return m_CharactersDict; } }

    static protected bool m_Loaded = false;
    static public bool loaded { get { return m_Loaded; } }

    static public Character GetCharacter(string type)
    {
        Character c;
        if (m_CharactersDict == null || !m_CharactersDict.TryGetValue(type, out c))
            return null;

        return c;
    }

    static public IEnumerator LoadDatabase()
    {
        if (m_CharactersDict == null)
        {
            m_CharactersDict = new Dictionary<string, Character>();

            var handle = Addressables.LoadAssetsAsync<GameObject>("characters", op =>
            {
                Character c = op.GetComponent<Character>();
                if (c != null && !m_CharactersDict.ContainsKey(c.characterName))
                    m_CharactersDict.Add(c.characterName, c);
            });

            yield return handle;

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError(
                    "CharacterDatabase: Addressables failed to load label 'characters'. "
                    + "Build Addressables for Android (Trash Dash Debug > Build with Bundle) before device builds. "
                    + handle.OperationException);
                m_Loaded = false;
                yield break;
            }

            m_Loaded = true;
        }
    }
}