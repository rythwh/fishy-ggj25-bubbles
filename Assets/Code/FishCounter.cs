using TMPro;
using UnityEngine;

public class FishCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text fishCounterText;
    private int fishCounter = 0;

    private void Start()
    {
        fishCounterText.text = fishCounter.ToString();
    }
    public void AddFish()
    {
        ++fishCounter;
        fishCounterText.text = fishCounter.ToString();
    }
}
