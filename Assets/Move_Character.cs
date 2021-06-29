using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_Character : MonoBehaviour
{
    float timeCounter;

    // Orientation variables
    Vector3 alignedUp;
    bool reorient;
    int thetaOffset; // Deg offset. Look at a unit circle. 0 to pi/2 rad is thetaOffset=0; pi/2 to pi is thetaOffset=90
    sbyte xOffset;
    sbyte yOffset;

    // Motion variables
    bool isMoving;
    float angleTransform;
    float speed = 100f; // STATIC - specifies the speed the character moves at (in degrees/milisecond; 100 speed = 0.9s)
    Vector3 startPos;
    Quaternion startRotation;

    // Input variables
    sbyte rotateDir; // +/- 1 ONLY
    sbyte cwX; // +/- 1 ONLY
    sbyte cwY; // +/- 1 ONLY

    // Start is called before the first frame update
    void Start()
    {
        isMoving = false;
        angleTransform = 0f;
        startPos = transform.position;
        startRotation = transform.rotation;

        alignedUp = NearestWorldAxis(transform.up);
        reorient = true;
    }

    // Update is called once per frame
    void Update()
    {
        timeCounter += Time.deltaTime;
        
        if (isMoving)
        {
            // ===== MOVING =====
            angleTransform += speed * Time.deltaTime;
            if (angleTransform > 90) { angleTransform = 90; }

            // TODO screw it I give up for now :) we're manually assigning offset
            Vector3 pos = new Vector3(
                startPos.x + cwX * Mathf.Cos(Mathf.PI * 2 * (angleTransform + thetaOffset) / 360) / 2 + xOffset,
                startPos.y + cwY * Mathf.Sin(Mathf.PI * 2 * (angleTransform + thetaOffset) / 360) / 2 + yOffset,
                startPos.z 
            );
            
            transform.rotation *= Quaternion.AngleAxis(speed * Time.deltaTime * rotateDir, Vector3.forward);
            transform.position = Vector3.Lerp(transform.position, pos, 1f);


            // ===== STOP MOVING =====
            if (angleTransform == 90)
            {
                isMoving = false;

                angleTransform = 0f;
                startPos = transform.position;
                startRotation = transform.rotation;

                // Snap rotation
                Debug.Log(alignedUp);
                alignedUp = NearestWorldAxis(transform.up);
                transform.rotation = Quaternion.LookRotation(Vector3.forward, alignedUp);
            }
        }
        else // Else await input
        {
            // ===== REORIENT =====
            if (reorient) {
                if (alignedUp == Vector3.up)
                {
                    thetaOffset = 0;
                }
                else if (alignedUp == Vector3.down)
                {
                    thetaOffset = 180;
                }
                else if (alignedUp == Vector3.left)
                {
                    thetaOffset = 90;
                }
                else if (alignedUp == Vector3.right)
                {
                    thetaOffset = 270;
                }
            }

            // ===== GET INPUT =====
            if (Mathf.Abs(Input.GetAxis("Vertical")) > Mathf.Abs(Input.GetAxis("Horizontal")))
            {
                if (Input.GetAxis("Vertical") > 0) // UP
                {
                    isMoving = true;
                    rotateDir = 1;
                    cwX = 1;
                    cwY = 1;
                }
                else if (Input.GetAxis("Vertical") < 0) // DOWN
                {
                    isMoving = true;
                    rotateDir = -1;
                    cwX = -1;
                    cwY = 1;
                }
            }
            else // TODO currently placeholder as backwords movement, expressly state verbage at some point
            {
                if (Input.GetAxis("Horizontal") < 0) // LEFT 
                {
                    isMoving = true;
                    rotateDir = -1;
                    cwX = 1;
                    cwY = -1;
                }
                else if (Input.GetAxis("Horizontal") > 0) // RIGHT
                {
                    isMoving = true;
                    rotateDir = 1;
                    cwX = -1;
                    cwY = -1;
                }
            }
        }
    }

    private static Vector3 NearestWorldAxis(Vector3 v)
    {
        if (Mathf.Abs(v.x) < Mathf.Abs(v.y))
        {
            v.x = 0;
            if (Mathf.Abs(v.y) < Mathf.Abs(v.z))
                v.y = 0;
            else
                v.z = 0;
        }
        else
        {
            v.y = 0;
            if (Mathf.Abs(v.x) < Mathf.Abs(v.z))
                v.x = 0;
            else
                v.z = 0;
        }
        return v;
    }
}