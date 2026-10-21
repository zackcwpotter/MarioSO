using UnityEngine;
using UnityEngine.Rendering;

// URP's Rendering Debugger creates a "[Debug Updater]" object in DontDestroyOnLoad (editor and development builds).
// The game doesn't use the Rendering Debugger, so turn it off before the first scene loads.
public static class DisableRenderDebugUpdater
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Disable()
    {
        DebugManager.instance.enableRuntimeUI = false;
    }
}
