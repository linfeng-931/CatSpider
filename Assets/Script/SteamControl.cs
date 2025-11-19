using UnityEngine;

public class SteamControl : MonoBehaviour
{
    public float ActiveTime;
    public float DisActiveTime;

    private float Timer;
    private bool isActive;
    private Animator ani;
    void Start()
    {
        Timer = 0f;
        isActive = false;
        ani = transform.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;
        if(Timer >= DisActiveTime && !isActive)
        {
            isActive = true;
            Timer = 0f;
            transform.GetComponent<BoxCollider2D>().enabled = true;
            ani.SetBool("isActive", true);
        }
        if(Timer >= ActiveTime && isActive)
        {
            isActive = false;
            Timer = 0f;
            transform.GetComponent<BoxCollider2D>().enabled = false;
            ani.SetBool("isActive", false);
        }

    }
}
