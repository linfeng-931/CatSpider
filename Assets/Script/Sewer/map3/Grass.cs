using UnityEngine;

public class Grass : MonoBehaviour
{
    public GameObject bottleCap;
    public achivementControl achivementFlag;
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
            if(!ani.GetBool("isActive")){
                achivementFlag.ActIndex = 0;
                achivementFlag.actAchivement[0] = 1;
            }
            ani.SetBool("isActive", true);
            bottleCap.GetComponent<BottleCap>().end = true;
        }
    }
}
