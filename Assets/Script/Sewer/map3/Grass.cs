using UnityEngine;

public class Grass : MonoBehaviour
{
    public GameObject bottleCap;
    private Animator ani;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ani = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(bottleCap.GetComponent<Transform>().position, transform.position) < 4.0f && bottleCap.GetComponent<Animator>().GetBool("catchWater"))
        {
            ani.SetBool("isActive", true);
            bottleCap.GetComponent<BottleCap>().end = true;
        }
    }
}
