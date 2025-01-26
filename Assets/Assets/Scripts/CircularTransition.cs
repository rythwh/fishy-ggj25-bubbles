using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;  // Add this namespace for DOTween

[RequireComponent(typeof(Image))]
public class CircularTransition : MonoBehaviour
{
    private Material transitionMaterial;
    private Image transitionImage;

    [SerializeField]
    private CanvasGroup canvasGroup;
    [SerializeField] private float transitionDuration = 0.5f;
    [SerializeField] private float smoothness = 0.01f;

    private static readonly int CenterProperty = Shader.PropertyToID("_Center");
    private static readonly int RadiusProperty = Shader.PropertyToID("_Radius");
    private static readonly int SmoothnessProperty = Shader.PropertyToID("_Smoothness");

    [SerializeField] private Transform targetObject;
    private bool hasTransitioned = false;
    private Vector2 targetUVPosition; // Cache the UV position


    private void Awake()
    {
        transitionImage = GetComponent<Image>();
        // Create a new material instance
        transitionMaterial = new Material(Shader.Find("UI/CircularTransition"));
        transitionImage.material = transitionMaterial;
    }

    private void Start()
    {
        // Set initial properties
        transitionMaterial.SetFloat(SmoothnessProperty, smoothness);
        transitionMaterial.SetFloat(RadiusProperty, -1f);

        // Pre-calculate the UV position
        if (targetObject != null)
        {
            Vector2 screenPos = Camera.main.WorldToScreenPoint(targetObject.position);
            targetUVPosition = new Vector2(
                screenPos.x / Screen.width,
                screenPos.y / Screen.height
            );
            transitionMaterial.SetVector(CenterProperty, targetUVPosition);
        }
    }

    public void StartTransition()
    {
        if (canvasGroup.isActiveAndEnabled && !hasTransitioned)
        {
            // Apply punch animation before starting the transition
            PunchCanvasGroup();

            // Start the transition coroutine
            StartCoroutine(TransitionCoroutine());
            hasTransitioned = true;
        }
    }

    // Function to apply punch effect on CanvasGroup
    private void PunchCanvasGroup()
    {
        // Use DOTween to apply a punch effect on the CanvasGroup's scale
        // canvasGroup.transform.DOPunchScale(Vector3.one * 0.1f, 0.5f, 10, 1f).OnKill(() =>
        // {
            // Callback to start the transition once the punch animation is done
            StartCoroutine(TransitionCoroutine());
        // });
    }

    private IEnumerator TransitionCoroutine()
    {
        canvasGroup.interactable = false;
        // Define the fade-out duration and transition duration for the material
        float fadeOutDuration = 0.2f; // Make fade-out faster
        float transitionDuration = 3f; // Make material transition faster
        float elapsedTime = 0f;

        // Fade out the CanvasGroup
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeOutDuration;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }

        // Ensure the CanvasGroup is fully transparent at the end of the fade
        canvasGroup.alpha = 0f;

        // Immediately start the material transition after fade-out, without delay
        elapsedTime = 0f;
        float startRadius = -1f;
        float targetRadius = 2f;

        // Transition the material's radius
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionDuration;
            float currentRadius = Mathf.Lerp(startRadius, targetRadius, t);
            transitionMaterial.SetFloat(RadiusProperty, currentRadius);
            yield return null;
        }

        // Ensure the material reaches the target radius at the end
        transitionMaterial.SetFloat(RadiusProperty, targetRadius);

        // Deactivate the game object after the transition completes
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (transitionMaterial != null)
        {
            Destroy(transitionMaterial);
        }
    }

    public void OnStartPressed()
    {
        Debug.Log("Start");
        if (targetObject != null)
        {
            StartCoroutine(TransitionCoroutine());
            hasTransitioned = true;
        }
        else
        {
            Debug.LogWarning("No target object assigned for transition!");
        }
    }
}