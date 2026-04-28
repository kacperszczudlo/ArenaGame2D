using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MissingScriptCleaner
{
    [MenuItem("Tools/Arena2D/Clean Missing Scripts In Selected Objects")]
    private static void CleanMissingScriptsInSelection()
    {
        int removed = 0;

        foreach (GameObject go in Selection.gameObjects)
        {
            removed += RemoveMissingScriptsRecursive(go);
        }

        if (removed > 0)
        {
            EditorSceneManager.MarkAllScenesDirty();
            Debug.Log($"[MissingScriptCleaner] Removed {removed} missing script component(s) from selection.");
        }
        else
        {
            Debug.Log("[MissingScriptCleaner] No missing scripts found in selection.");
        }
    }

    [MenuItem("Tools/Arena2D/Clean Missing Scripts In Open Scenes")]
    private static void CleanMissingScriptsInOpenScenes()
    {
        int removed = 0;

        for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++)
        {
            Scene scene = SceneManager.GetSceneAt(sceneIndex);
            if (!scene.isLoaded) continue;

            foreach (GameObject root in scene.GetRootGameObjects())
            {
                removed += RemoveMissingScriptsRecursive(root);
            }

            EditorSceneManager.MarkSceneDirty(scene);
        }

        if (removed > 0)
        {
            Debug.Log($"[MissingScriptCleaner] Removed {removed} missing script component(s) from open scene(s).");
        }
        else
        {
            Debug.Log("[MissingScriptCleaner] No missing scripts found in open scene(s).");
        }
    }

    private static int RemoveMissingScriptsRecursive(GameObject root)
    {
        if (root == null) return 0;

        int removed = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(root);
        foreach (Transform child in root.transform)
        {
            removed += RemoveMissingScriptsRecursive(child.gameObject);
        }

        return removed;
    }
}