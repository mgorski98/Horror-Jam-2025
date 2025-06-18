using Assets.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

public class FPSPlayerController : MonoBehaviour
{
    public Vector3 MoveVector;
    public float MoveSpeed;
    public float SprintSpeed;
    public float CameraSensitivity;
    public Camera PlayerCamera;
    public CharacterController CharController;

    public Transform PlayerShipParent;

    public InputActionSupplier MoveSupplier;
    public PlayerInput Input;
    public InputActionAsset InputActions;

    private float CurrentCameraRotationX;
    public float MinCameraRotationDegrees;
    public float MaxCameraRotationDegrees;

    public float CameraHeadBobMoveFrequency;
    public float CameraHeadBobStrength;
    public float HorizontalHeadBobFrequency;
    public float HorizontalHeadBobStrength;
    public float ReturnToStartValueSmoothing = 4f;
    private float HeadBobAccumulator;

    private Vector3 StartLocalCameraPos;

    private bool IsSprinting = false;

    private void Awake() {
        StartLocalCameraPos = PlayerCamera.transform.localPosition;
    }

    private void Start() {
        var globalActions = InputActions.FindActionMap("Global");
        globalActions.Enable(); //pauza, toggle œwiatla gracza, etc.
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

    public void DoSprint(InputAction.CallbackContext ctx) {
        if (ctx.started)
            IsSprinting = true;
        if (ctx.canceled)
            IsSprinting = false;
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
        if (CharController.isGrounded == false && transform.parent == null) {
            vec.y = Physics.gravity.y * Time.fixedDeltaTime;
        }
        CharController.Move((IsSprinting ? SprintSpeed : MoveSpeed) * Time.fixedDeltaTime * vec);
    }

    private void LateUpdate() {
        if (MoveVector != Vector3.zero && CharController.isGrounded) {
            DoHeadBob();
            HeadBobAccumulator += Time.deltaTime;
        }
        else {
            ReturnHeadBobToDefaultPos();
        }
    }

    private void DoHeadBob() {
        var pos = default(Vector3);
        pos.x += Mathf.Cos(HeadBobAccumulator * HorizontalHeadBobFrequency / 2) * HorizontalHeadBobStrength * 2;
        pos.y += Mathf.Sin(HeadBobAccumulator * CameraHeadBobMoveFrequency) * CameraHeadBobStrength;
        PlayerCamera.transform.localPosition = pos + this.StartLocalCameraPos;
    }

    private Vector3 smoothVec;
    private void ReturnHeadBobToDefaultPos() {
        PlayerCamera.transform.localPosition = Vector3.SmoothDamp(PlayerCamera.transform.localPosition, StartLocalCameraPos, ref smoothVec, ReturnToStartValueSmoothing);
    }
}
