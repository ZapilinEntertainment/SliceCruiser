using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    [SerializeField] private float maxspeed = 20f, accelerationSpeed = 7f, rotatingSpeed = 30f, shipHeight = 5, 
        maxEnergy = 30f, maxJumpForce = 11f;
    [SerializeField] private Vector3 basepoint = Vector3.zero;
    public bool verticalBrakesEffectActive { get; private set; }
    public bool powered { get; private set; }
    private bool jumping = false, grounded = true;
    private float speed = 0f, speedMultiplier = 0f, energy, gravity = 0f, jumpForce = 0f;
    private Vector3 localRotationVector = Vector3.zero;

    private const float DAMPENING_SPEED = 2f, GRAVITY = 9.8f, GROUNDING_HEIGHT = 3f, GROUNDING_SPEED = 10f,
        JUMP_ENERGY_COST = 2f, JUMP_ACCELERATION = 10f, ENERGY_GAINING_SPEED = 5f, SHIP_ALIGNMENT_SPEED = 3f;
    private int watermask = 255;

    private void Awake()
    {
        watermask = LayerMask.GetMask("Water");
        energy = maxEnergy;
        verticalBrakesEffectActive = false;
        jumping = false;

        RaycastHit rh;
        Vector3 bpos = transform.TransformPoint(basepoint), mdir = transform.TransformDirection(Vector3.up);
        if (Physics.Raycast(bpos + mdir * shipHeight, -mdir,
            out rh, shipHeight * 2f, watermask, QueryTriggerInteraction.Collide))
        {
            transform.position = new Vector3(transform.position.x, rh.point.y - basepoint.y, transform.position.z);
        }
    }

    void Update()
    {
        float t = Time.deltaTime;
        if (!GameMaster.gamePaused)
        {
            Vector3 endPos = transform.position;
            if (jumping)
            {
                float cost = JUMP_ENERGY_COST * t;
                if (energy >= cost)
                {
                    energy -= cost;
                    if (jumpForce != maxJumpForce)
                    {
                        jumpForce = Mathf.Lerp(jumpForce, maxJumpForce, JUMP_ACCELERATION * t);
                        endPos += transform.TransformDirection(Vector3.up * jumpForce * t);
                    }
                    gravity = 0f;
                }
                else
                {
                    jumping = false;
                    jumpForce = 0f;
                }
            }

            RaycastHit rh;
            Vector3 bpos = transform.TransformPoint(basepoint), mdir = transform.TransformDirection(Vector3.up);
            #region unusing
            //cast positions
            //1              3
            //       0   
            //2              4
            var vects = new Vector2[] { Vector2.zero, new Vector2(2, 4), new Vector2(-2, 4), new Vector2(2, -4), new Vector2(-2, -4) };
            Vector3 centralCastPoint = bpos + mdir * shipHeight;
            float maxDistance = shipHeight * 2f;
            powered = false;
            bool touching = false;
            var resultingVector = Vector3.zero;
            #endregion

            Vector3 pt;
            var heights = new float[5];
            for (int i = 0; i < 5; i++) {
                pt = new Vector3(vects[i].x, 0f, vects[i].y);
                if (Physics.Raycast(centralCastPoint + transform.TransformVector(pt), -mdir, out rh, maxDistance, watermask, QueryTriggerInteraction.Collide))
                {                    
                    if (rh.distance < GROUNDING_HEIGHT)
                    {
                        heights[i] = transform.InverseTransformPoint(rh.point).y;
                        touching = true;
                        powered = rh.collider.tag == GameMaster.waterTag;
                        resultingVector = rh.normal;
                    }
                    else heights[i] = 0f;
                }
            }
            //

            bpos = transform.TransformPoint(basepoint); mdir = transform.TransformDirection(Vector3.up);
            if (Physics.Raycast(bpos + mdir * shipHeight, -mdir,
                out rh, shipHeight * 2f, watermask, QueryTriggerInteraction.Collide))
            {
                float draft = transform.InverseTransformPoint(rh.point).y - basepoint.y;// + LevelMaster.GetWaveHeight(rh.point.x, rh.point.z);
                grounded = draft > 0f || (draft <= 0f && draft > -GROUNDING_HEIGHT);
                
                if (powered & energy == 0f & jumping) jumping = false;
                if (grounded && draft != 0f)
                {
                    float s = draft > 0f ? GROUNDING_SPEED * 0.02f * (speed/10f + 1f) * t : GROUNDING_SPEED * 0.01f * t;
                    if (!jumping) { endPos = Vector3.MoveTowards(endPos, endPos + rh.normal * draft, s); }
                }
                bpos = transform.TransformPoint(basepoint + Vector3.forward * 4);

                //var vf = transform.TransformDirection(Vector3.up);
                //if (rh.normal != vf && speed > 0f) transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.FromToRotation(vf, rh.normal),3f * t);

                if (!jumping && Physics.Raycast(bpos + mdir * shipHeight, -mdir,
                out rh, shipHeight * 2f, watermask, QueryTriggerInteraction.Collide))
                {
                    float a = transform.InverseTransformPoint(rh.point).y - basepoint.y;// + LevelMaster.GetWaveHeight(rh.point.x, rh.point.z);
                    localRotationVector.x =   Mathf.Lerp(localRotationVector.x, (a > 0f ? -0.3f : 0.3f), Time.deltaTime * 4f);
                }

                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, rh.normal), rh.normal), SHIP_ALIGNMENT_SPEED * t);
            }
            else
            {
                grounded = false;                
                verticalBrakesEffectActive = false;
                powered = false;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, Vector3.up), Vector3.up), SHIP_ALIGNMENT_SPEED * t);
            }
            //
            if (grounded)
            {
                if (gravity != 0f) gravity = Mathf.Lerp(gravity, 0f, GROUNDING_SPEED * 2 * t);               
            }
            else
            {
                if (gravity < 120f && !jumping) gravity += GRAVITY * t;
            }
            if (gravity != 0f && !jumping)  endPos+= Vector3.down * gravity * t;

            if (localRotationVector != Vector3.zero)
            {
                float x = Mathf.Sin(speed / maxspeed);
                transform.Rotate(localRotationVector * rotatingSpeed * t * x, Space.Self);
                localRotationVector = Vector3.Lerp(localRotationVector, Vector3.zero, DAMPENING_SPEED * t);
            }
            speed = Mathf.Lerp(speed, maxspeed * speedMultiplier, accelerationSpeed * t);
            if (speed != 0f)  endPos += transform.forward * speed * t;
            transform.position = endPos;

            if (powered && energy != maxEnergy) energy = Mathf.Lerp(energy, maxEnergy, ENERGY_GAINING_SPEED * t);
        }

        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawIcon(transform.TransformPoint(basepoint), "baseposIcon.png");
    }

    public void ChangeSpeedMultiplier(float f) {
        speedMultiplier += f;
        if (speedMultiplier > 1f) speedMultiplier = 1f; 
        else { if (speedMultiplier < 0f) speedMultiplier = 0f; }
    }
    public void SteerHorizontal(float f)
    {
        localRotationVector -= Vector3.up * f;
    }
    public void SwitchJumpEngine()
    {
        jumping = !jumping;
    }
    //
    private void OnGUI()
    {
        GUILayout.Label(localRotationVector.ToString());
        GUILayout.Label(grounded ? "grounded" : "in air");
        GUILayout.Label(powered ? "powered" : "no power");
        GUILayout.Label(gravity.ToString());
        GUILayout.Label(energy.ToString());
    }
}
