using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleFadeIn : MonoBehaviour
{
    [SerializeField] private float fadeInDuration = 1f;
    private ParticleSystemRenderer particleRenderer;
    private Material materialInstance;
    private float currentAlpha = 0f;
    private float targetAlpha = 1f;
    private bool isFading = false;

    private void Awake()
    {
        particleRenderer = GetComponent<ParticleSystemRenderer>();
        // Create a material instance to avoid affecting other objects using the same material
        materialInstance = new Material(particleRenderer.material);
        particleRenderer.material = materialInstance;
    }

    private void OnEnable()
    {
        currentAlpha = 0f;
        isFading = true;
        UpdateMaterialAlpha();
    }

    private void Update()
    {
        if (!isFading) return;

        currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, Time.deltaTime / fadeInDuration);
        UpdateMaterialAlpha();

        if (Mathf.Approximately(currentAlpha, targetAlpha))
        {
            isFading = false;
        }
    }

    private void UpdateMaterialAlpha()
    {
        Color color = materialInstance.color;
        color.a = currentAlpha;
        materialInstance.color = color;
    }

    private void OnDestroy()
    {
        // Clean up the material instance when the object is destroyed
        if (materialInstance != null)
        {
            Destroy(materialInstance);
        }
    }
} 