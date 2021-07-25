using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private ShipController controller;
    private bool inputEnabled = true;
    private const float KEY_FORCE = 0.5f;

    void Update()
    {
        if (inputEnabled && controller != null)
        {
            float t = Time.deltaTime;
            if (Input.GetKeyDown("w")) controller.ChangeSpeedMultiplier(KEY_FORCE);
            else
            {
                if (Input.GetKeyDown("s")) controller.ChangeSpeedMultiplier(-KEY_FORCE);
            }
            if (Input.GetKey("a")) controller.SteerHorizontal(KEY_FORCE * 4f  * t);
            else
            {
                if (Input.GetKey("d")) controller.SteerHorizontal(-4f * KEY_FORCE  * t);
            }

            if (Input.GetKeyDown(KeyCode.Space)) controller.SwitchJumpEngine();
            else
            {
                if (Input.GetKeyUp(KeyCode.Space)) controller.SwitchJumpEngine();
            }

            if (Input.GetKey(KeyCode.Q)) controller.RotateShip(KEY_FORCE * t * 4);
            else
            {
                if (Input.GetKey(KeyCode.E)) controller.RotateShip(-KEY_FORCE * t * 4);
            }
        }
    }
}
