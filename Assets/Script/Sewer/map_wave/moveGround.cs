using UnityEngine;

public class moveGround : MonoBehaviour
{
    public float speed;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left*speed*Time.deltaTime);
        if(transform.position.x < -80) Destroy(gameObject);
    }
}
