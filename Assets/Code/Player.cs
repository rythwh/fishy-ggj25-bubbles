using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float movementSmoothness = 0.1f;
    [SerializeField] private float rotationSmoothness = 0.1f;

    [SerializeField] private FishingMinigame fishingMinigame;
    [SerializeField] private GameObject fishingSpotIndicator;
    private Vector2 movementInput;
    private float currentSpeed;
    private float currentRotationSpeed;
    private bool isCollidingWithFish = false;
    private GameObject currentFishingSpot;
    private Vector3 indicatorInitialScale;
    

    public event Action<Vector3> OnPlayerMoved;
    

    [UsedImplicitly]
    public void OnMove(InputAction.CallbackContext context) {
        movementInput = context.ReadValue<Vector2>();
    }
    
    [UsedImplicitly]
    public void OnInteract(InputAction.CallbackContext context) {
        if (isCollidingWithFish && !fishingMinigame.reelingFish)
        {
            StartFish();
        }
    }

    private void Start()
    {
        if (fishingSpotIndicator != null)
        {
            indicatorInitialScale = fishingSpotIndicator.transform.localScale;
            fishingSpotIndicator.SetActive(false);
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

    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Fish"))
        {
            isCollidingWithFish = true;
            if (!fishingSpotIndicator.activeSelf)
            {
                fishingSpotIndicator.SetActive(true);
                fishingSpotIndicator.transform.localScale = Vector3.zero;
                fishingSpotIndicator.transform.DOScale(indicatorInitialScale, 0.3f)
                    .SetEase(Ease.OutBack);
            }
            currentFishingSpot = other.gameObject;
            fishingMinigame.SetCurrentFishingSpot(currentFishingSpot);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Fish"))
        {
            isCollidingWithFish = false;
            fishingSpotIndicator.transform.DOScale(Vector3.zero, 0.2f)
                .SetEase(Ease.InBack)
                .OnComplete(() => fishingSpotIndicator.SetActive(false));
            currentFishingSpot = null;
            fishingMinigame.SetCurrentFishingSpot(null);
        }
    }

    private void StartFish()
    {
        Debug.Log("Fish!");
        fishingMinigame.StartReeling();
    }
}