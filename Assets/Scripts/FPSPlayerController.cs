using Assets.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

public class FPSPlayerController : MonoBehaviour
{
    public Vector3 MoveVector;
    public float MoveSpeed;
    public float CameraSensitivity;
    public Camera PlayerCamera;
    public CharacterController CharController;

    public InputActionSupplier MoveSupplier;

    private float CurrentCameraRotationX;
    public float MinCameraRotationDegrees;
    public float MaxCameraRotationDegrees;

    private void Awake() {
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
        var rawInputVector = MoveSupplier.Action.ReadValue<Vector2>();
        MoveVector = transform.forward * rawInputVector.y + transform.right * rawInputVector.x;
    }

    private void FixedUpdate() {
        var vec = MoveVector;
        if (CharController.isGrounded == false) {
            vec.y = Physics.gravity.y * Time.fixedDeltaTime;
        }
        CharController.Move(vec * MoveSpeed * Time.fixedDeltaTime);
    }

    private void LateUpdate() {
    }
}
