using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraEffectsController : MonoBehaviour
{
    private Camera myCam;
    private float normalFOV = 60f, speededFOV = 90f;

    private void Start()
    {
        myCam = GetComponent<Camera>();
        GameMaster.currentShip.speedChangedEvent += this.SpeedChanged;
    }
    private void SpeedChanged(float t)
    {
        myCam.fieldOfView = Mathf.Lerp(normalFOV, speededFOV, t / GameConstants.MAX_SPEED);
    }
}
