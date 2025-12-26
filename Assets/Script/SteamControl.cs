using UnityEngine;

public class SteamControl : MonoBehaviour
{
    public float ActiveTime;
    public float DisActiveTime;
    public bool canHurt = true;
    

    private float Timer;
    private bool isActive;
    private Animator ani;
    private AudioSource audioSource;

    void Start()
    {
        Timer = 0f;
        isActive = false;
        ani = transform.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        audioSource.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;
        if(Timer >= DisActiveTime && !isActive)
        {
            isActive = true;
            audioSource.enabled = true;
            Timer = 0f;
            if(canHurt)transform.GetComponent<BoxCollider2D>().enabled = true;
            ani.SetBool("isActive", true);
        }
        if(Timer >= ActiveTime && isActive)
        {
            isActive = false;
            audioSource.enabled = false;
            Timer = 0f;
            if(canHurt)transform.GetComponent<BoxCollider2D>().enabled = false;
            ani.SetBool("isActive", false);
        }

    }
}
