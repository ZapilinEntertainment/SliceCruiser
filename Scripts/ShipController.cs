using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public sealed class ShipController : MonoBehaviour
{
    private float maxspeed = GameConstants.MAX_SPEED, accelerationSpeed = 40f, rotatingSpeed = 70f, shipHeight = 5, 
        maxEnergy = 30f, maxJumpForce = 11f, wallAlignmentCf = 15f;
    [SerializeField] private Vector3 basepoint = Vector3.zero;
    public bool verticalBrakesEffectActive { get; private set; }
    public bool grounded { get; private set; }
    public bool powered { get; private set; }
    public float energyPercent { get { if (maxEnergy != 0f) return energy / maxEnergy; else return 1f; } }
    public Action<float> speedChangedEvent;

    private bool jumping = false;
    private float speed = 0f,prevSpeed = 0f, speedMultiplier = 0f, energy, gravity = 0f, jumpForce = 0f;
    private Vector3 localRotationVector = Vector3.zero;

    private const float ROTATION_DAMP = 0.6f, GRAVITY = 9.8f, GROUNDING_HEIGHT = 3f, GROUNDING_SPEED = 5f,
        JUMP_ENERGY_COST = 2f, JUMP_ACCELERATION = 10f, ENERGY_GAINING_SPEED = 5f, SHIP_ALIGNMENT_SPEED = 3f,
        DISCHARGE_SPEED = 0.3f;
    private int watermask = 255;

    private void Awake()
    {
        watermask = LayerMask.GetMask("Water");
        energy = maxEnergy;
        verticalBrakesEffectActive = false;
        jumping = false;
        grounded = true;

        RaycastHit rh;
        Vector3 bpos = transform.TransformPoint(basepoint), mdir = transform.TransformDirection(Vector3.up);
        if (Physics.Raycast(bpos + mdir * shipHeight, -mdir,
            out rh, shipHeight * 2f, watermask, QueryTriggerInteraction.Collide))
        {
            transform.position = new Vector3(transform.position.x, rh.point.y - basepoint.y, transform.position.z);
        }

        GameMaster.currentShip = this;
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
                        jumpForce = Mathf.MoveTowards(jumpForce, maxJumpForce, JUMP_ACCELERATION * t);
                    }
                    gravity = 0f;
                }
                else
                {
                   jumping = false;
                }                
            }
            else
            {
                if (jumpForce != 0f)
                {
                    jumpForce = Mathf.MoveTowards(jumpForce, 0f, gravity * t);
                }
            }
            if (jumpForce != 0f) endPos += transform.TransformDirection(Vector3.up * jumpForce * t);

            RaycastHit rh;
            Vector3 bpos = transform.TransformPoint(basepoint), mdir = transform.TransformDirection(Vector3.up);          
                 
            var vects = new Vector2[] { Vector2.zero, new Vector2(2, 4), new Vector2(-2, 4), new Vector2(2, -4), new Vector2(-2, -4) };            
           Vector3 centralCastPoint = bpos + mdir * shipHeight;
           float maxDistance = shipHeight * 2f;
           powered = false;
           bool touching = false;

            #region unusing
            /*
            var resultingVector = Vector3.zero;
            Vector3 pt;
            //cast positions
            //1              3
            //       0   
            //2              4
            var heights = new float[5];
            for (int i = 0; i < 5; i++) {
                pt = new Vector3(vects[i].x, 0f, vects[i].y);
                if (Physics.Raycast(centralCastPoint + transform.TransformVector(pt), -mdir, out rh, maxDistance, watermask, QueryTriggerInteraction.Collide))
                {                    
                    if (rh.distance < GROUNDING_HEIGHT)
                    {
                        heights[i] = transform.InverseTransformPoint(rh.point).y;
                        touching = true;
                        
                        resultingVector = rh.normal;
                    }
                    else heights[i] = 0f;
                }
            }
            //
            */
            #endregion

            bpos = transform.TransformPoint(basepoint); mdir = transform.TransformDirection(Vector3.up);
            if (Physics.Raycast(bpos + mdir * shipHeight, -mdir,
                out rh, shipHeight * 2f, watermask, QueryTriggerInteraction.Collide))
            {
                powered = rh.collider.tag == GameMaster.waterTag;
                float draft = transform.InverseTransformPoint(rh.point).y - basepoint.y;// + LevelMaster.GetWaveHeight(rh.point.x, rh.point.z);
                grounded = draft > 0f || (draft <= 0f && draft > -GROUNDING_HEIGHT);
                
                if (powered & energy == 0f & jumping) jumping = false;
                if (grounded && draft != 0f)
                {
                    float materialResistance = powered ? 1f : 20f;
                    float s = draft > 0f ? GROUNDING_SPEED * 2f * (speed/10f + 1f) * materialResistance * t : GROUNDING_SPEED  * t;
                    if (!jumping) { endPos = Vector3.MoveTowards(endPos, endPos + rh.normal * draft, s); }
                }
                bpos = transform.TransformPoint(basepoint + Vector3.forward * 4);

                //var vf = transform.TransformDirection(Vector3.up);
                //if (rh.normal != vf && speed > 0f) transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.FromToRotation(vf, rh.normal),3f * t);

                // покачивание на волнах:
                if (!jumping && Physics.Raycast(bpos + mdir * shipHeight, -mdir,
                out rh, shipHeight * 2f, watermask, QueryTriggerInteraction.Collide))
                {
                    float a = transform.InverseTransformPoint(rh.point).y - basepoint.y;// + LevelMaster.GetWaveHeight(rh.point.x, rh.point.z);
                    //localRotationVector.x =   Mathf.MoveTowardsAngle(localRotationVector.x, (a > 0f ? -0.3f : 0.3f), Time.deltaTime * SHIP_ALIGNMENT_SPEED); // 4f
                }

                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, rh.normal), rh.normal), wallAlignmentCf * SHIP_ALIGNMENT_SPEED * t);

            }
            else
            {
                grounded = false;                
                verticalBrakesEffectActive = false;
                powered = false;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, Vector3.up), Vector3.up), SHIP_ALIGNMENT_SPEED * t);
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
                float a, b, c;
                if (localRotationVector.magnitude > 1f)
                {
                    a = Mathf.Clamp(localRotationVector.x, -1f, 1f);
                    b = Mathf.Clamp(localRotationVector.y, -1f, 1f);
                    c = Mathf.Clamp(localRotationVector.z, -1f, 1f);
                    localRotationVector = new Vector3(a, b, c);
                }

                float x = Mathf.Sin(speed / maxspeed);
                transform.Rotate(localRotationVector * rotatingSpeed * t * x, Space.Self);
                
                a = Mathf.MoveTowards(localRotationVector.x, 0f, 0.3f * t);
                x = localRotationVector.y;
                b = Mathf.MoveTowards(x, 0f, ROTATION_DAMP * t);
                x = localRotationVector.z;
                c = Mathf.MoveTowards(x, 0f, ROTATION_DAMP/2f * t);
                localRotationVector = new Vector3(a, b, c);
            }
            speed = Mathf.MoveTowards(speed, maxspeed * speedMultiplier, accelerationSpeed * t);
            if (speed != 0f)  endPos += transform.forward * speed * t;
            if (speed != prevSpeed)
            {
                speedChangedEvent(speed);
                prevSpeed = speed;
            }
            transform.position = endPos;

            if (powered)
            {
             if (energy < maxEnergy)   energy = Mathf.Lerp(energy, maxEnergy, ENERGY_GAINING_SPEED * t);
            }
            else
            {
                if (energy != 0) energy = Mathf.Lerp(energy, 0f, DISCHARGE_SPEED * t);
            }

            if (transform.position.y < GameConstants.LOWEST_HEIGHT) Respawn();
        }        
    }

    public void Respawn()
    {        
        speed = 0f;
        localRotationVector = Vector3.zero;
        speedMultiplier = 0f;
        if (!RespawnFlower.TrySetRespawnPosition(transform)) gameObject.SetActive(false);
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
    public void RotateShip(float f)
    {
        //if (!grounded)
            localRotationVector += Vector3.forward * f;
    }
    public void SteerVertical(float f)
    {
        localRotationVector -= Vector3.right * f;
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
    }
}
