using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// Keeps a single active EventSystem when the shop scene is loaded additively over main.
/// </summary>
public static class EventSystemCoordinator
{
    const string k_ShopSceneName = "shop";

    static EventSystem s_SuspendedPeer;
    static bool s_Registered;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        s_SuspendedPeer = null;
        s_Registered = false;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Register()
    {
        if (s_Registered)
            return;

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        s_Registered = true;
    }

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode != LoadSceneMode.Additive || scene.name != k_ShopSceneName)
            return;

        var shopEventSystem = FindEventSystemInScene(scene);
        if (shopEventSystem == null)
            return;

        s_SuspendedPeer = null;
        foreach (var es in EventSystem.FindObjectsByType<EventSystem>(FindObjectsSortMode.None))
        {
            if (es == shopEventSystem)
                continue;

            if (es.isActiveAndEnabled)
            {
                s_SuspendedPeer = es;
                es.enabled = false;
                break;
            }
        }

        if (!shopEventSystem.enabled)
            shopEventSystem.enabled = true;
    }

    static void OnSceneUnloaded(Scene scene)
    {
        if (scene.name != k_ShopSceneName)
            return;

        if (s_SuspendedPeer != null)
        {
            s_SuspendedPeer.enabled = true;
            s_SuspendedPeer = null;
        }
    }

    static EventSystem FindEventSystemInScene(Scene scene)
    {
        if (!scene.isLoaded)
            return null;

        foreach (var root in scene.GetRootGameObjects())
        {
            var es = root.GetComponentInChildren<EventSystem>(true);
            if (es != null)
                return es;
        }

        return null;
    }
}
