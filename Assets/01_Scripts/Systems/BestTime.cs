using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BestTime : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI bestTimeText;

    void Start()
    {
        string bestTime = PlayerPrefs.GetString("bestTime");

        if (bestTime == null) return;

        bestTimeText.text = bestTime;
    }
}
