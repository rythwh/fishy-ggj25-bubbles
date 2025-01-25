using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;
using System.Collections;

public class FishingMinigame : MonoBehaviour
{
	/// <summary>
	/// This is the main fishing minigame code. It controls the states of fishing,
	/// then when in the minigame state it controls the movement of the catching
	/// bar and selects which fish we are catching.
	/// </summary>
	
	//These bools are just for keeping track of the state of the minigame we are in
	public bool reelingFish = false;

	[Header("Setup References")]
	//The catching bar is the green bar that you put ontop of the fish to catch it
	[SerializeField] private GameObject catchingbar;
	private Vector3 catchingBarLoc;
	private Rigidbody2D catchingBarRB;
	
	//This is the fish on the UI that you are chasing to catch
	[SerializeField] private GameObject fishBar;
	private bool inTrigger = false; //Whether or not the fish is inside the "catchingbar"
	
	private float catchPercentage = 0f; //0-100 how much you have caught the fish
	[SerializeField] private Slider catchProgressBar; //The bar on the right that shows how much you have caught

	[SerializeField] private GameObject minigameCanvas;

	[Header("Fish Settings")]
	[SerializeField] private Sprite[] fishSprites; // Array of available fish sprites
	private string currentFishName; // Keep track of which fish we caught

	[Header("Game Settings")]
	[SerializeField] private float catchSpeed = 10f;
	[SerializeField] private float catchingForce = 500f;

	private bool isJumpHeld = false;

	private void Start() {
		catchingBarRB = catchingbar.GetComponent<Rigidbody2D>(); //Get reference to the Rigidbody on the catchingbar
		catchingBarLoc = catchingbar.GetComponent<RectTransform>().localPosition; //Use this to reset the catchingbars position to the bottom of the "water"
	}

	public void OnJump(InputAction.CallbackContext context)
	{
		if (context.started && !reelingFish)
		{
			StartReeling();
		}
		
		isJumpHeld = context.performed;
	}

	private void Update() {
		if (!reelingFish) {
			return;
		}

		// Handle catching bar movement
		if (isJumpHeld) {
			catchingBarRB.AddForce(Vector2.up * catchingForce * Time.deltaTime, ForceMode2D.Force);
		}

		// Update catch progress
		catchPercentage += (inTrigger ? catchSpeed : -catchSpeed) * Time.deltaTime;
		catchPercentage = Mathf.Clamp(catchPercentage, 0, 100);
		catchProgressBar.value = catchPercentage;

		if (catchPercentage >= 100) {
			FishCaught();
		}
	}

	private void StartReeling() {
		reelingFish = true;
		
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

		minigameCanvas.SetActive(true);
	}

	public void FishInBar() => inTrigger = true;
	
	public void FishOutOfBar() => inTrigger = false;

	private void FishCaught() {
		Debug.Log($"Success! Caught a: {currentFishName}");
		reelingFish = false;
		minigameCanvas.SetActive(false);
		catchingbar.transform.localPosition = catchingBarLoc;
		catchPercentage = 0f;
	}

	private float Map(float a, float b, float c, float d, float x) {
		return (x - a) / (b - a) * (d - c) + c;
	}
}
