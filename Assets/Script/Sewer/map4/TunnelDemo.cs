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
        if(Mathf.Abs(player.position.x - transform.position.x) < 5.0f)
        {
            if(changeSceneTimer == 0f)
            {
                fade.SetTrigger("changeScene");
            }
            changeSceneTimer += Time.deltaTime;

            if(changeSceneTimer >= 1.5f)
            {
                SceneManager.LoadScene(5, LoadSceneMode.Single);
                //目前無法切換！！！！！！！！！！！！！！！！！
            }
        }

    }
}
