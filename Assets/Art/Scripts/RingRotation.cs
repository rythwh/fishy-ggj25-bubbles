using UnityEngine;
using DG.Tweening; // Import DoTween namespace

public class RingRotation : MonoBehaviour
{
    public float swayAmount = 15f;   // Max sway angle (degrees)
    public float swaySpeed = 2f;     // Speed of the sway (seconds for one full cycle)
    public bool isAsynchronous = true;  // Flag to control if the sway is asynchronous
    public float swayOffset = 0f;   // Delay offset for asynchronous sway

    void Start()
    {
        // Start the sway effect, considering if it should be asynchronous
        if (isAsynchronous)
        {
            SwaySideToSideAsync();
        }
        else
        {
            SwaySideToSide();
        }
    }

    // Asynchronous Sway: Rings sway with an offset in time
    void SwaySideToSideAsync()
    {
        // Apply a random delay for each ring to make them sway asynchronously
        transform.DORotate(new Vector3(swayAmount, 0, 0), swaySpeed, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Yoyo)  // Yoyo loops make it swing back and forth
            .SetEase(Ease.InOutSine)      // Smooth easing for natural sway
            .SetDelay(Random.Range(0f, swayOffset)); // Add random delay between rings
    }

    // Synchronous Sway: Rings sway in sync (same as before)
    void SwaySideToSide()
    {
        // Animate the rotation of the ring along the Z-axis (side to side sway)
        transform.DORotate(new Vector3(swayAmount, 0, 0), swaySpeed, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Yoyo)  // Yoyo loops make it swing back and forth
            .SetEase(Ease.InOutSine);     // Smooth easing for natural sway
    }
}
