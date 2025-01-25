using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float movementSmoothness = 0.1f;
    [SerializeField] private float rotationSmoothness = 0.1f;
    
    private Vector2 movementInput;
    private float currentSpeed;
    private float currentRotationSpeed;

    public event Action<Vector2> OnPlayerMoved;

    [UsedImplicitly]
    public void OnMove(InputAction.CallbackContext context) {
        movementInput = context.ReadValue<Vector2>();
    }

    private void Update() {
        Move();
    }

    private void Move() {
        // Smoothly adjust rotation speed
        float targetRotationSpeed = movementInput.x * rotationSpeed;
        currentRotationSpeed = Mathf.Lerp(currentRotationSpeed, targetRotationSpeed, rotationSmoothness);
        transform.Rotate(0, currentRotationSpeed * Time.deltaTime, 0);

        // Smoothly adjust movement speed
        float targetSpeed = movementInput.y * moveSpeed;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, movementSmoothness);
        Vector3 moveDirection = transform.forward * currentSpeed;
        transform.position += moveDirection * Time.deltaTime;

        if (Mathf.Approximately(moveDirection.magnitude, 0)) {
            return;
        }
        OnPlayerMoved?.Invoke(transform.position);
    }
}