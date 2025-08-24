using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;
using UnityEngine.UI;

public class Test: MonoBehaviour
{
    [SerializeField] PUCustomSettings powerUpSettings;
    [SerializeField] Button powerUpButton;
    [SerializeField] GameObject lockedOverlay;

    private void Start()
    {
        UpdateIconState();
    }

    public void UpdateIconState()
    {
        bool isAvailable = powerUpSettings.IsAvailable();

        powerUpButton.interactable = isAvailable;
        lockedOverlay.SetActive(!isAvailable);
    }
}
