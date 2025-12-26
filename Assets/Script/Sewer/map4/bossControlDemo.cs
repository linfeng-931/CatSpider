using System;
using UnityEngine;

public class bossControlDemo : MonoBehaviour
{
    public Vector3 actPoint;
    public float speed;
    public Transform player;
    public GameObject camera;
    public int type; // 0normal, 1boss

    private bool isAct;
    private Vector3 startPoint;
    private float Timer;
    private bool type2StartRun;
    private float rePoint = 94.7f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        Timer = 0;
        type2StartRun = false;
        startPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(player.position, actPoint) < 3.0f && !isAct)
        {
            isAct = true;
            GetComponent<SpriteRenderer>().enabled = true;
            if(type == 1) camera.GetComponent<Follower>().isShake = true;
        }
        Action();

        if(Math.Abs(transform.position.x - rePoint) < 0.5f)
        {
            Timer = 0;
            type2StartRun = false;
            GetComponent<SpriteRenderer>().enabled = false;
            transform.position = startPoint;
            camera.GetComponent<Follower>().isShake = false;
            camera.GetComponent<Follower>().puzzlesIndex = -1;
            isAct = false;
        }
    }
    void Action()
    {
        if(!isAct) return;
        
        if(type == 0)
        {
            transform.Translate(Vector3.right*speed*Time.deltaTime);
            if(transform.position.x > 100) isAct = false;
        }
        else
        {
            if (!type2StartRun)
            {
                Timer+= Time.deltaTime;
                if (Timer > 2.0f)
                {
                    camera.GetComponent<Follower>().puzzlesIndex = 0;
                    camera.GetComponent<Follower>().puzzlesTime = 1f;
                    type2StartRun = true;
                }
            }
            else
            {
                transform.Translate(Vector3.right*speed*Time.deltaTime);
            }
        }  
    }
}
