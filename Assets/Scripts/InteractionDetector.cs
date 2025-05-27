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

    private void Awake() {
        this.InteractAction = Actions.FindAction("Player/Interact");
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
                }
            }
            CurrentInteractable = interactable;
            InteractionText.gameObject.SetActive(CurrentInteractable != null);
        }
    }

    public void DoInteract(InputAction.CallbackContext ctx) {
        if (ctx.performed == false)
            return;

        if (CurrentInteractable == null)
            return;

        CurrentInteractable.DoInteract();
    }
}
