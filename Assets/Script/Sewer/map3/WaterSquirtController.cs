using UnityEngine;

public class WaterSquirtController : MonoBehaviour
{
    public bool Active;
    public bool canHurt = true;
    
    private Animator ani;
    void Start()
    {
        ani = transform.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Active)
        {
            if(canHurt)transform.GetComponent<BoxCollider2D>().enabled = true;
            ani.SetBool("isActive", true); 
        }
        else
        {
            if(canHurt)transform.GetComponent<BoxCollider2D>().enabled = false;
            ani.SetBool("isActive", false);
        }
    }
}

