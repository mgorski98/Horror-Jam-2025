using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class FPSPlayerController : MonoBehaviour
{
    private InputAction MoveAction;
    public Vector3 MoveVector;
    public float MoveSpeed;
    public float CameraSensitivity;
    public Camera PlayerCamera;
    public CharacterController CharController;

    [SerializeField]
    private InputActionAsset Actions;

    private float CurrentCameraRotationX;
    public float MinCameraRotationDegrees;
    public float MaxCameraRotationDegrees;

    private void Awake() {
        this.MoveAction = Actions.FindAction("Player/Move");
    }

    public void RotateCamera(InputAction.CallbackContext ctx) {
        var rotationVector = ctx.ReadValue<Vector2>();

        var rotX = -(rotationVector.y * CameraSensitivity * Time.deltaTime);
        CurrentCameraRotationX += rotX;
        CurrentCameraRotationX = ClampAngle(CurrentCameraRotationX, MinCameraRotationDegrees, MaxCameraRotationDegrees);
        var rotY = transform.eulerAngles.y + CameraSensitivity * rotationVector.x * Time.deltaTime;
        transform.rotation = Quaternion.AngleAxis(rotY, Vector3.up);
        PlayerCamera.transform.rotation = Quaternion.Euler(CurrentCameraRotationX, rotY, 0f);
    }

    private float ClampAngle(float angle, float min, float max) {
        if (angle < -360f) angle += 360f;
        if (angle > 360f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }

    private void Update() {
        var rawInputVector = MoveAction.ReadValue<Vector2>();
        MoveVector = transform.forward * rawInputVector.y + transform.right * rawInputVector.x;
    }

    private void FixedUpdate() {
        CharController.Move(MoveVector * MoveSpeed * Time.deltaTime);
    }

    private void LateUpdate() {
        DoHeadBob();
    }

    private void DoHeadBob() {

    }
}
