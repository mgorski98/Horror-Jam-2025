using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts {
    public class PauseMenu : MonoBehaviour {
        public void Toggle(bool active) => this.gameObject.SetActive(active);

        public void Resume() {
            GameManager.Instance.TogglePause();
        }

        public void ExitToMenu() {
            GameManager.Instance.QuitToMenu();
        }

        public void ExitToDesktop() {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying)
                UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
