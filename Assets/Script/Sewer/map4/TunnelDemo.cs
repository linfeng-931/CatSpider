using System;
using System.Collections;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class TunnelDemo : MonoBehaviour
{
    public Transform player;
    public string TargetScene;
    public Animator fade;

    private float changeSceneTimer;
    private bool canChange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canChange = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Mathf.Abs(player.position.x - transform.position.x) < 10.0f || canChange)
        {
            if(changeSceneTimer == 0f)
            {
                canChange = true;
                fade.SetTrigger("changeScene");
            }
            changeSceneTimer += Time.deltaTime;

            if(changeSceneTimer >= 1f)
            {
                SceneManager.LoadScene(0, LoadSceneMode.Single);
            }
        }

    }
}
