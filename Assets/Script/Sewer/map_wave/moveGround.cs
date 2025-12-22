using UnityEngine;

public class moveGround : MonoBehaviour
{
    public float speed;

    private GameObject box;
    void Start()
    {
        box = transform.GetChild(0).gameObject;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(Vector3.left*speed*Time.deltaTime);
        if(transform.position.x < -80) Destroy(gameObject);
    }
}
