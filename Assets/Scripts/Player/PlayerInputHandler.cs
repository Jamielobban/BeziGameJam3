using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private InputSystem_Actions controls;
    private PlayerController movement;
    private ScreenRotator rotator;

    private void Awake()
    {
        controls = new InputSystem_Actions();
        movement = GetComponent<PlayerController>();
        rotator = FindFirstObjectByType<ScreenRotator>();

        controls.Player.Move.performed += ctx => movement.SetMoveInput(ctx.ReadValue<Vector2>());
        controls.Player.Move.canceled += ctx => movement.SetMoveInput(Vector2.zero);

        controls.Player.RotateLeft.started += ctx => rotator.SetRotationDirection(1f);
        controls.Player.RotateLeft.canceled += ctx => rotator.SetRotationDirection(0f);
        controls.Player.RotateRight.started += ctx => rotator.SetRotationDirection(-1f);
        controls.Player.RotateRight.canceled += ctx => rotator.SetRotationDirection(0f);

        controls.Player.Jump.performed += ctx => movement.Jump();
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void FixedUpdate()
    {
        movement.SetRotation(rotator.CurrentRotation);
    }
}
