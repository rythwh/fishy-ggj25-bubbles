using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
public class CircularTransition : MonoBehaviour
{
    private Material transitionMaterial;
    private Image transitionImage;

    [SerializeField] private float transitionDuration = 1.5f;
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && !hasTransitioned)
        {
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

    private IEnumerator TransitionCoroutine()
    {
        // No need to recalculate position here since we're using the cached value
        float elapsedTime = 0f;
        float startRadius = -1f;
        float targetRadius = 2f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionDuration;
            float currentRadius = Mathf.Lerp(startRadius, targetRadius, t);
            transitionMaterial.SetFloat(RadiusProperty, currentRadius);
            yield return null;
        }

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (transitionMaterial != null)
        {
            Destroy(transitionMaterial);
        }
    }
} 