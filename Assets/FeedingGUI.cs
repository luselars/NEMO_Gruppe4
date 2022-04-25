using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedingGUI : MonoBehaviour
{
    public Feeding feeding;
    private bool feedState;
    private bool anticipateFeedState;

    public void ToggleFeed()
    {
        feeding.isFeeding = !feeding.isFeeding;
    }

    public void ToggleAnticipateFeed()
    {
        feeding.anticipateFeeding = !feeding.anticipateFeeding;
    }
}
