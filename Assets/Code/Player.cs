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

    [SerializeField] private FishCounter fishCounter;
    private Vector2 movementInput;
    private float currentSpeed;
    private float currentRotationSpeed;
    private bool isCollidingWithFish = false;

    public event Action<Vector3> OnPlayerMoved;
    

    [UsedImplicitly]
    public void OnMove(InputAction.CallbackContext context) {
        movementInput = context.ReadValue<Vector2>();
    }
    
    [UsedImplicitly]
    public void OnInteract(InputAction.CallbackContext context) {
        if (isCollidingWithFish) // Check if the interaction is performed and the object is colliding with a fish
        {
            StartFish();
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Fish"))
        {
            isCollidingWithFish = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Fish"))
        {
            isCollidingWithFish = false;
        }
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

    private void StartFish()
    {
        Debug.Log("Fish!");
        fishCounter.AddFish();
    }
}