using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 movementInput;

    public event Action<Vector2> OnPlayerMoved;

    [UsedImplicitly]
    public void OnMove(InputAction.CallbackContext context) {
        movementInput = context.ReadValue<Vector2>();
    }

    private void Update() {
        Move();
    }

    private void Move() {
        Vector3 moveDirection = new Vector3(movementInput.x, 0f, movementInput.y);
        transform.position += moveDirection * (moveSpeed * Time.deltaTime);
        if (Mathf.Approximately(moveDirection.magnitude, 0)) {
            return;
        }
        OnPlayerMoved?.Invoke(transform.position);
    }
}