using UnityEngine;

public class sinkGround : MonoBehaviour
{
    public GameObject player;
    private bool isSink;
    private float speed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        speed = transform.parent.GetComponent<moveGround>().speed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(isSink){
            transform.Translate(Vector3.down*0.2f*Time.deltaTime);
            player.transform.Translate(Vector3.down*0.2f*Time.deltaTime);
        }
    }

    void OnCollisionEnter2D(Collision2D other){
        if (other.gameObject.CompareTag("Player"))
        {
            isSink = true;
            //停止物理模擬，讓程式碼完全接管移動
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.linearVelocity = Vector2.zero;
            }
        }
    }
}
