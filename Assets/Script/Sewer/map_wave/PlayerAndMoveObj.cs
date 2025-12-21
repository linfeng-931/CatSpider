using UnityEngine;

public class PlayerAndMoveObj : MonoBehaviour
{
    moveGround currentGround;
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate(){
        if(currentGround != null)
        {
            transform.position += Vector3.left * currentGround.speed * Time.fixedDeltaTime;
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingObj"))
        {
            currentGround = collision.transform.parent.gameObject.GetComponent<moveGround>();
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingObj"))
        {
            currentGround = null;
        }
    }
}
