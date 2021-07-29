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
            if (Input.GetKeyDown(KeyCode.W)) controller.ChangeSpeedMultiplier(KEY_FORCE);
            else
            {
                if (Input.GetKeyDown(KeyCode.S)) controller.ChangeSpeedMultiplier(-KEY_FORCE);
            }
            if (Input.GetKey(KeyCode.A)) controller.SteerHorizontal(KEY_FORCE * 4f  * t);
            else
            {
                if (Input.GetKey(KeyCode.D)) controller.SteerHorizontal(-4f * KEY_FORCE  * t);
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

            if (Input.GetKeyDown(KeyCode.R))
            {
                controller.Respawn();
            }

            if (Input.GetKey(KeyCode.Z)) controller.SteerVertical(KEY_FORCE * t);
            else
            {
                if (Input.GetKey(KeyCode.C)) controller.SteerVertical(-1f * KEY_FORCE * t);
            }
        }
    }
}
