using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    public float InteractionRange;
    public LayerMask InteractablesMask;
    public InteractableObject CurrentInteractable;

    public Transform CheckOrigin;
    public Transform CheckDirectionSupplier;

    public TMPro.TMP_Text InteractionText;

    private InputAction InteractAction;
    [SerializeField]
    private InputActionAsset Actions;

    [ColorUsage(true, true)]
    public Color TintColor;
    private Color[] PreviousDefaultRendererColors = new Color[150];

    private void Awake() {
        this.InteractAction = Actions.FindAction("Player/Interact");
    }

    private void OnDisable() {
        ClearInteractData();
    }

    private void Update() {
        if (Physics.Raycast(CheckOrigin.position, CheckDirectionSupplier.forward, out RaycastHit hit, InteractionRange, InteractablesMask)) {
            var interactable = hit.transform.gameObject.GetComponent<InteractableObject>();
            if (interactable == null) {
                InteractionText.text = string.Empty;
            } else {
                if (interactable != CurrentInteractable) {
                    if (interactable.ShouldShowBindingKey()) {
                        InteractionText.text = $"[{InteractAction.GetBindingDisplayString()}] {interactable.GetInteractionName()}";
                    } else {
                        InteractionText.text = interactable.GetInteractionName();
                    }
                    UpdateInteractableMaterialColors(interactable);
                }
            }
            CurrentInteractable = interactable;
            InteractionText.gameObject.SetActive(CurrentInteractable != null);
        } else {
            ClearInteractData();
        }
    }

    private void UpdateInteractableMaterialColors(InteractableObject obj) {
        var renderers = obj.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; ++i) {
            PreviousDefaultRendererColors[i] = renderers[i].material.color;
            renderers[i].material.color = TintColor;
        }
    }

    private void ClearInteractableMaterialColors(InteractableObject obj) {
        var renderers = obj.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; ++i) {
            renderers[i].material.color = PreviousDefaultRendererColors[i];
        }
    }

    public void ClearInteractData() {
        if (CurrentInteractable != null) {
            ClearInteractableMaterialColors(CurrentInteractable);
            CurrentInteractable = null;
        }
        if (InteractionText != null)
            InteractionText.gameObject.SetActive(false);
    }

    public void DoInteract(InputAction.CallbackContext ctx) {
        if (ctx.performed == false)
            return;

        if (CurrentInteractable == null)
            return;

        CurrentInteractable.DoInteract();
    }
}
