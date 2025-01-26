using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class Player : MonoBehaviour
{
    [SerializeField] private float thrustForce = 10f;
    [SerializeField] private float rotationForce = 100f;
    [SerializeField] private float waterDrag = 1f;
    [SerializeField] private float waterAngularDrag = 1f;
    [SerializeField] private float maxUpwardAngle = 20f;

    [SerializeField] private FishingMinigame fishingMinigame;
    [SerializeField] private GameObject fishingSpotIndicator;
    private Vector2 movementInput;
    private Rigidbody rb;
    private bool isCollidingWithFish = false;
    private GameObject currentFishingSpot;
    private Vector3 indicatorInitialScale;
    
    public event Action<Vector3> OnPlayerMoved;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Configure the rigidbody for boat physics
        rb.useGravity = false;
        rb.linearDamping = waterDrag;
        rb.angularDamping = waterAngularDrag;
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void Start()
    {
        if (fishingSpotIndicator != null)
        {
            indicatorInitialScale = fishingSpotIndicator.transform.localScale;
            fishingSpotIndicator.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        MoveBoat();
    }

    private void MoveBoat()
    {
        // Project velocity onto the horizontal plane to ensure movement stays level
        Vector3 horizontalVelocity = Vector3.ProjectOnPlane(rb.linearVelocity, Vector3.up);
        rb.linearVelocity = horizontalVelocity;

        // Apply forward/backward force
        if (movementInput.y != 0)
        {
            Vector3 force = transform.forward * (movementInput.y * thrustForce);
            // Project the force onto the horizontal plane
            force = Vector3.ProjectOnPlane(force, Vector3.up);
            rb.AddForce(force, ForceMode.Force);
        }

        // Apply rotation
        if (movementInput.x != 0)
        {
            rb.AddTorque(transform.up * (movementInput.x * rotationForce), ForceMode.Force);
        }

        // Trigger movement event if the boat is moving
        if (rb.linearVelocity.magnitude > 0.01f)
        {
            OnPlayerMoved?.Invoke(transform.position);
        }
    }

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