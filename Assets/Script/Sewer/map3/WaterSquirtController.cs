using UnityEngine;

public class WaterSquirtController : MonoBehaviour
{
    public bool Active;
    public bool canHurt = true;
    
    private Animator ani;
    private AudioSource audioSource;
    void Start()
    {
        ani = transform.GetComponent<Animator>();
        audioSource = transform.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Active)
        {
            if(canHurt)transform.GetComponent<BoxCollider2D>().enabled = true;
            ani.SetBool("isActive", true); 
            audioSource.enabled = true;
        }
        else
        {
            if(canHurt)transform.GetComponent<BoxCollider2D>().enabled = false;
            ani.SetBool("isActive", false);
            audioSource.enabled = false;
        }
    }
}

