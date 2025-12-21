using System;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;

public class DropObjControl : MonoBehaviour
{   
    public Transform keyPoint;
    public Transform Player;
    public Transform Box;
    public float groundY;

    private bool isUp;
    private bool finishDown;
    private bool rotateFlag;
    private float rotateTargetUp;
    private float rotateTargetDown;
    private float baseRotateTarget;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isUp = true;
        finishDown = false;
        rotateFlag = true;
        rotateTargetUp = 355f;
        rotateTargetDown = 351f;
        baseRotateTarget = 3.0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isUp)
        {
            RotateControl();
            if(Vector2.Distance(keyPoint.position, Box.position) < 5.0f)
            {
                isUp = false;
            }
        }
        else
        {
            if (finishDown)
            {
                
            }
            else
            {
                Drop();
            }
        }
        
    }

    void RotateControl()
    {
        var rotZ = transform.eulerAngles.z;
        if (rotateFlag)
        {
            if(rotZ < rotateTargetUp)
            {
                rotZ += 3f*Time.deltaTime;
            }
            else
            {
                rotZ = rotateTargetUp;
                rotateFlag = false;
            }
        }
        else
        {
            if(rotZ > rotateTargetDown)
            {
                rotZ -= 3f*Time.deltaTime;
            }
            else
            {
                rotZ = rotateTargetDown;
                rotateFlag = true;
            }
        }
        transform.rotation = Quaternion.Euler(0f,0f,rotZ);

        //如果玩家在水管上
        if(Vector2.Distance(Player.transform.position, keyPoint.position) < 15.0f)
        {
            rotateTargetDown = 351f - 0.1f*Vector2.Distance(Player.transform.position, keyPoint.position);
            rotateTargetUp = 355f - 0.2f*Vector2.Distance(Player.transform.position, keyPoint.position);
        }
        else
        {
            rotateTargetUp = 355f;
            rotateTargetDown = 351f;
        }
    }

    void Drop()
    {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, groundY), 30.0f*Time.deltaTime);
        if(transform.eulerAngles.z >= 0f)
        {
            var rotZ = transform.eulerAngles.z;
            rotZ = 0f;
            transform.rotation = Quaternion.Euler(0f,0f,rotZ);
        }
        else
        {
            transform.Rotate(new Vector3(0f,0f,0.01f*Time.deltaTime));
        } 
        if(transform.position.y <= groundY)
        {
            transform.position = new Vector2(transform.position.x, groundY);
            finishDown = false;
        }
    }
}
