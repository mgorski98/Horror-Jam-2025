using Assets.Scripts;
using Assets.Utils;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Linq;

public enum GameOverType {
    Drowned, //utoniêcie przez np. kolizjê
    Consumed //rekin
}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get => _instance; private set => _instance = value; }

    public ObservableValue<bool> IsPaused = new(false);

    public PauseMenu PauseMenu;

    public float GameOverFadeDuration = 1.5f;
    public CanvasGroup FadeOutCGroup;
    public CanvasGroup GameOverMenuCGroup;
    public GameObject GameOverMenuObject;

    public GameObject Player;

    public string MenuSceneName;
    public TMP_Text GameOverText;

    public SaltDepositShipStation[] AllDepositStations;
    public bool AreStationsDone => AllDepositStations.All(station => station.IsDone);

    private void Awake() {
        if (_instance == null)
            _instance = this;

        IsPaused.OnValueChanged.AddListener((o, n) => Time.timeScale = System.Convert.ToSingle(!n));
        IsPaused.OnValueChanged.AddListener((o, n) => PauseMenu.Toggle(n));

        HideCursor(true);
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
        if (Keyboard.current.backspaceKey.wasPressedThisFrame) {
            DoGameOver(GameOverType.Drowned);
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
        HideCursor(!IsPaused.Value);
    }

    public void DoGameOver(GameOverType goType) {
        GameOverText.text = goType switch {
            GameOverType.Drowned => "You drowned...",
            GameOverType.Consumed => "You were consumed...",
            _ => ""
        };
        GameOverMenuObject.SetActive(true);
        FadeOutCGroup.DOFade(1, GameOverFadeDuration).SetUpdate(true).onComplete += () => {
            GameOverMenuCGroup.DOFade(1, GameOverFadeDuration).SetUpdate(true).onComplete += PerformGameOverCleanup;
        };
    }

    public void QuitToMenu() => SceneManager.LoadScene(MenuSceneName);

    private void PerformGameOverCleanup() {
        Player.SetActive(false);
    }

    public void QuickRestartScene() { 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void HideCursor(bool value)
    {
        Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !value;
    }
}
