using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Image powerBar;
    [SerializeField] private RawImage powerIcon;
    private bool linkReady = false, showEnergyIcon = false, powerIconAnimationInProgress = false;
    private float powerIconAnimationAlpha = 1f;
    private ShipController shipController;
    private const float POWER_ICON_ANIMATION_SPEED = 2f;

    private void Start()
    {
        shipController = GameMaster.currentShip;
        linkReady = shipController != null;
        powerIconAnimationAlpha = 1f; showEnergyIcon = true;
    }

    private void Update()
    {
        if (linkReady)
        {
            powerBar.fillAmount = shipController.energyPercent;
            if (showEnergyIcon != shipController.powered)
            {
                powerIconAnimationInProgress = true;
                showEnergyIcon = shipController.powered;
            }
            if (powerIconAnimationInProgress)
            {
                float t = Time.deltaTime;
                if (showEnergyIcon)
                {
                    powerIconAnimationAlpha = Mathf.MoveTowards(powerIconAnimationAlpha, 1f, POWER_ICON_ANIMATION_SPEED * 2f * t);
                    if (powerIconAnimationAlpha == 1f) powerIconAnimationInProgress = false;
                }
                else
                {
                    powerIconAnimationAlpha = Mathf.MoveTowards(powerIconAnimationAlpha, 0f, POWER_ICON_ANIMATION_SPEED * t);
                    if (powerIconAnimationAlpha == 0f) powerIconAnimationInProgress = false;
                }
                powerIcon.color = new Color(1f, 1f, 1f, powerIconAnimationAlpha);
            }
        }
    }
}
