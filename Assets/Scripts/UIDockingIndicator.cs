using Assets.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIDockingIndicator : MonoBehaviour {
    public Image RadialFillImage;
    public InputActionSupplier DockActionSupplier;
    public TMP_Text KeybindText;
    public TMP_Text ActionText;

    private void Awake() {
        if (gameObject.activeSelf)
            Hide();

        KeybindText.text = DockActionSupplier.Action.GetBindingDisplayString();
    }

    private void OnDisable() {
        RadialFillImage.fillAmount = 0;
    }

    public void SetDocked(bool docked) {
        ActionText.text = docked ? "Undock" : "Dock";
    }

    public void UpdateFill(float value) => RadialFillImage.fillAmount = value;

    public void Show() {
        this.gameObject.SetActive(true);
    }

    public void Hide() {
        this.gameObject.SetActive(false);
    }
}
