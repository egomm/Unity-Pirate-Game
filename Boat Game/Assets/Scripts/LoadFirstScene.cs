#if UNITY_EDITOR // If the game is being run in the unity editor
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoadAttribute]
public static class DefaultSceneLoader {
    static DefaultSceneLoader() {
        EditorApplication.playModeStateChanged += LoadDefaultScene;
    }

    // Method for loading the default scene (the Main Menu scene which is at index 0)
    static void LoadDefaultScene(PlayModeStateChange state) {
        if (state == PlayModeStateChange.ExitingEditMode) {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }
        if (state == PlayModeStateChange.EnteredPlayMode) {
            if (EditorSceneManager.GetActiveScene().buildIndex != 0) {
                EditorSceneManager.LoadScene(0);
            }
        }
    }
}
#endif