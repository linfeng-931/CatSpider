using UnityEngine;

public class BottleCap : MonoBehaviour
{
    public bool end;
    private Animator ani;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ani = GetComponent<Animator>();
        end = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (end)
        {
            ani.SetBool("disActive", true);
        }   
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("DashObject"))
        {
            ani.SetBool("catchWater", true);
        }
    }
}
