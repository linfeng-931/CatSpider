using UnityEngine;

public class mouseController : MonoBehaviour
{
    public Animator ani;
    public float speed;

    private float Timer;
    private bool dir;
    private SpriteRenderer img;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dir = false;
        img = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;

        if (dir)
        {
            if(Timer > 3f)
            {
                img.flipX = true;
                transform.Translate(Vector3.right * Time.deltaTime*speed);
                ani.SetBool("isWalk", true);
                if(Timer > 10f)
                {
                    ani.SetBool("isWalk", false);
                    Timer = 0;
                    dir = false;
                }
            }
        }
        else{
            if(Timer > 3f)
            {
                img.flipX = false;
                transform.Translate(Vector3.left * Time.deltaTime*speed);
                ani.SetBool("isWalk", true);
                if(Timer > 10f)
                {
                    ani.SetBool("isWalk", false);
                    Timer = 0;
                    dir = true;
                }
            }
        }
    }
}
