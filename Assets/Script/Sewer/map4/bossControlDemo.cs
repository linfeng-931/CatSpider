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
    private bool isFinal;
    private Vector3 startPoint;
    private float Timer;
    private bool type2StartRun;
    private float rePoint = 94.7f;
    private CapsuleCollider2D capsuleCollider2D;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        Timer = 0;
        type2StartRun = false;
        startPoint = transform.position;
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isFinal) return;

        if(Vector3.Distance(player.position, actPoint) < 3.0f && !isAct)
        {
            isAct = true;
            GetComponent<AudioSource>().enabled = true;
            GetComponent<SpriteRenderer>().enabled = true;
            if(type == 1) camera.GetComponent<Follower>().isShake = true;
        }
        Action();

        if(Math.Abs(transform.position.x - rePoint) < 0.5f && !isFinal)
        {
            Timer = 0;
            type2StartRun = false;
            GetComponent<SpriteRenderer>().enabled = false;
            if(type == 1) transform.position = startPoint;
            if(camera!=null) camera.GetComponent<Follower>().isShake = false;
            if(camera!=null) camera.GetComponent<Follower>().puzzlesIndex = -1;
            GetComponent<AudioSource>().enabled = false;
            isAct = false;
        }
    }
    void Action()
    {
        if(!isAct || isFinal) return;
        
        if(type == 0)
        {
            transform.Translate(Vector3.right*speed*Time.deltaTime);
            if(transform.position.x >= 100f){
                isAct = false;
                GetComponent<AudioSource>().enabled = false;
                isFinal = true;
                return;
            }
        }
        else
        {
            if (!type2StartRun)
            {
                capsuleCollider2D.enabled = true;
                Timer+= Time.deltaTime;
                if (Timer > 2.0f)
                {
                    if(camera!=null) camera.GetComponent<Follower>().puzzlesIndex = 0;
                    if(camera!=null) camera.GetComponent<Follower>().puzzlesTime = 1f;
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
