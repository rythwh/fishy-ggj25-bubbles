using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;
using TMPro;
using JetBrains.Annotations;
using DG.Tweening;

public class FishingMinigame : MonoBehaviour
{
    /// <summary>
    /// This is the main fishing minigame code. It controls the states of fishing,
    /// then when in the minigame state it controls the movement of the catching
    /// bar and selects which fish we are catching.
    /// </summary>
    
    // These bools are just for keeping track of the state of the minigame we are in
    public bool reelingFish = false;
    private GameObject currentFishingSpot;
    private FishSpawner fishSpawner;

    [Header("Setup References")]
    // The catching bar is the green bar that you put on top of the fish to catch it
    [SerializeField] private GameObject catchingbar;
    private Vector3 catchingBarLoc;
    private Rigidbody2D catchingBarRB;
    
    // This is the fish on the UI that you are chasing to catch
    [SerializeField] private GameObject fishBar;
    private bool inTrigger = false; // Whether or not the fish is inside the "catchingbar"
    
    private float catchPercentage = 0f; // 0-100 how much you have caught the fish
    [SerializeField] private Slider catchProgressBar; // The bar on the right that shows how much you have caught

    [SerializeField] private GameObject minigameCanvas;

    [Header("Fish Settings")]
    [SerializeField] private Sprite[] fishSprites; // Array of available fish sprites
    private string currentFishName; // Keep track of which fish we caught

    [Header("Game Settings")]
    [SerializeField] private float catchSpeed = 10f;
    [SerializeField] private float catchingForce = 500f;

    // Replace FishCounter with direct TMP_Text reference
    [SerializeField] private TMP_Text fishCounterText;
    private int fishCount = 0;

    private bool isJumpHeld = false;

    [Header("Setup References")]
    [SerializeField] private GameObject fishingSpotIndicator;
    [SerializeField] private GameObject failBubble;

    private Transform failBubbleInitialTransform;
    private float fishingTimeout = 10f; // Time in seconds before fishing fails due to inactivity
    private float fishingTimer = 0f;

    private void Start() {
        catchingBarRB = catchingbar.GetComponent<Rigidbody2D>();
        catchingBarLoc = catchingbar.GetComponent<RectTransform>().localPosition;
        // Find the FishSpawner in the scene
        fishSpawner = FindObjectOfType<FishSpawner>();
        if (fishSpawner == null)
        {
            Debug.LogError("No FishSpawner found in scene!");
        }
        if (failBubble != null)
        {
            failBubbleInitialTransform = failBubble.transform;
            failBubble.SetActive(false);
        }
    }

    [UsedImplicitly]
    public void OnJump(InputAction.CallbackContext context)
    {
        isJumpHeld = context.performed;
    }

    private void Update() 
    {
        if (!reelingFish) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            reelingFish = true;
            FishCaught();
        }

        if (isJumpHeld) {
            catchingBarRB.AddForce(Vector2.up * catchingForce * Time.deltaTime, ForceMode2D.Force);
        }

        // Update catch progress
        catchPercentage += (inTrigger ? catchSpeed : -catchSpeed) * Time.deltaTime;
        catchPercentage = Mathf.Clamp(catchPercentage, 0, 100);
        catchProgressBar.value = catchPercentage;

        // Check if player has caught the fish
        if (catchPercentage >= 100) {
            FishCaught();
        }

        // Track time and fail the fishing if not progressing
        if (reelingFish) {
            fishingTimer += Time.deltaTime;
            if (fishingTimer > fishingTimeout) {
                ResetFishing(); // Fail the fishing after timeout
            }
        }
    }

    public void StartReeling() {
        reelingFish = true;
        catchingbar.transform.localPosition = catchingBarLoc;
        
        // Pick a random fish sprite
        int randomFishIndex = Random.Range(0, fishSprites.Length);
        Sprite randomFishSprite = fishSprites[randomFishIndex];
        currentFishName = randomFishSprite.name;
        
        // Set the fish sprite
        fishBar.GetComponent<Image>().sprite = randomFishSprite;
        
        // Adjust size if needed
        var w = Map(0, 32, 0, 100, randomFishSprite.texture.width);
        var h = Map(0, 32, 0, 100, randomFishSprite.texture.height);
        fishBar.GetComponent<RectTransform>().sizeDelta = new Vector2(w, h);

        // Reset fish position and state
        fishBar.transform.localPosition = Vector3.zero;
        var fishTrigger = fishBar.GetComponent<FishingMinigame_FishTrigger>();
        if (fishTrigger != null)
        {
            fishTrigger.beingCaught = false;
        }
        
        // Reset fish evasion
        var fishEvasion = fishBar.GetComponent<FishEvasion>();
        if (fishEvasion != null)
        {
            fishEvasion.currentDestination = fishBar.transform.position;
            fishEvasion.waiting = false;
        }
        
        inTrigger = false;
        minigameCanvas.SetActive(true);
        fishingTimer = 0f; // Reset the timer when starting
    }

    public void FishInBar() => inTrigger = true;
    
    public void FishOutOfBar() => inTrigger = false;

    private void ResetFishing()
    {
        reelingFish = false;
        catchPercentage = 0f;  // Reset the catch progress when the fishing fails or is canceled
        catchingbar.transform.localPosition = catchingBarLoc;
        failBubble.SetActive(true);

        // Show fishing spot indicator again
        AnimateFishingSpotIndicator();

        // Animate fail bubble regardless of reason (failure or cancellation)
        StartCoroutine(AnimateFailBubble());

        // Hide fishing UI if the player backed away (fishing spot is null)
        if (currentFishingSpot == null)
        {
            minigameCanvas.SetActive(false);
        }
    }

    public void SetCurrentFishingSpot(GameObject fishingSpot)
    {
        currentFishingSpot = fishingSpot;
    
        // If we leave the fishing spot while fishing, cancel the minigame and hide the UI
        if (fishingSpot == null && reelingFish)
        {
            ResetFishing();
        }
        else
        {
            // If we're still on the fishing spot, allow for retrying
            if (reelingFish == false)
            {
                minigameCanvas.SetActive(true);
                catchPercentage = 0f;  // Reset progress when re-entering the fishing spot
            }
        }
    }

    private void FishCaught() {
        Debug.Log($"Success! Caught a: {currentFishName}");
        
        reelingFish = false;
        minigameCanvas.SetActive(false);
        catchPercentage = 0f;
        
        // Update fish count directly
        fishCount++;
        fishCounterText.text = fishCount.ToString();
        Vector3 punchAmount = fishCounterText.transform.localScale * 0.3f; // 30% of current scale
        fishCounterText.transform.DOPunchScale(punchAmount, 0.3f, 10, 0.5f)
            .SetEase(Ease.OutElastic);

        // Remove the fishing spot after catching the fish
        currentFishingSpot.SetActive(false);
        
        // Reset the fish's progress after successful catch
        currentFishingSpot = null;
    }

    private IEnumerator AnimateFailBubble()
    {
        failBubble.transform.localScale = failBubbleInitialTransform.localScale;
        Vector3 originalScale = failBubbleInitialTransform.localScale;

        failBubble.transform.DOScale(originalScale * 1.2f, 0.3f) // Slightly larger for the pop effect
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                failBubble.transform.localScale = originalScale;
            });

        yield return new WaitForSeconds(0f);

        failBubble.transform.DOScale(Vector3.zero, 0.3f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                failBubble.SetActive(false);
                failBubble.transform.localScale = originalScale;
            });
    }

    private void AnimateFishingSpotIndicator()
    {
        if (fishingSpotIndicator != null)
        {
            // Animate the fishing spot indicator (e.g., a small bounce or fade effect)
            fishingSpotIndicator.transform.DOPunchScale(Vector3.one * 0.1f, 0.5f, 10, 0.5f)
                .SetEase(Ease.OutBack);
        }
    }

    private float Map(float a, float b, float c, float d, float x) {
        return (x - a) / (b - a) * (d - c) + c;
    }
}
