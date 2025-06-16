using Assets.Scripts;
using Assets.Utils;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get => _instance; private set => _instance = value; }

    public ObservableValue<bool> IsPaused = new(false);

    public PauseMenu PauseMenu;

    public float GameOverFadeDuration = 1.5f;
    public CanvasGroup FadeOutCGroup;
    public CanvasGroup GameOverMenuCGroup;

    private void Awake() {
        if (_instance == null)
            _instance = this;

        IsPaused.OnValueChanged.AddListener((o, n) => Time.timeScale = System.Convert.ToSingle(!n));
        IsPaused.OnValueChanged.AddListener((o, n) => PauseMenu.Toggle(n));
    }

    private void OnDestroy() {
        if (_instance == this)
            _instance = null;
    }

    private void Update() {
#if UNITY_EDITOR
        if (Keyboard.current.zKey.wasPressedThisFrame) {
            QuickRestartScene();
        }
#endif
    }

    public void TogglePause_Action(InputAction.CallbackContext ctx) {
        if (ctx.performed == false)
            return;

        TogglePause();
    }

    public void TogglePause() {
        IsPaused.Value = !IsPaused.Value;
    }

    public void DoGameOver() {
        FadeOutCGroup.DOFade(1, GameOverFadeDuration).SetUpdate(true).onComplete += () => {
            GameOverMenuCGroup.DOFade(1, GameOverFadeDuration).SetUpdate(true);
        };
    }

    public void QuickRestartScene() { 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
