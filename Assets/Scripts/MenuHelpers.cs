using UnityEngine;
namespace Assets.Scripts {
    public class MenuHelpers : MonoBehaviour {
        public string GameSceneName;
        public void ExitToDesktop() {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void NewGame() {
            UnityEngine.SceneManagement.SceneManager.LoadScene(GameSceneName);
        }
    }
}
