using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour
{
    public Animator animator;
    public Animator feetAni;
    public Transform frontPoint;
    public Transform frontGroundPoint;
    public float MoveSpeed;
    public float Attack;
    public float LookAroundTime;
    public float WalkTime;
    public float IdleTime;
    public LayerMask groundMask;
    public Vector3 frontPointOffset;
    public Vector3 frontGroundPointOffset;

    private float idleTimer;
    private float walkTimer;
    private float lookAroundTimer;
    private int action; //1-idle, 2-walk, 3-lookAround
    private bool flip;

    void Start()
    {
        action = 1;
        flip = false;
    }

    void Update()
    {
        bool standGround = Physics2D.OverlapCircle(frontGroundPoint.position, 0.1f, groundMask);
        bool colWall = Physics2D.OverlapCircle(frontPoint.position, 0.1f, groundMask);

        transform.GetComponent<SpriteRenderer>().flipX = flip;
        int dir = flip ? 1 : -1;
        if (!flip)
        {
            frontPoint.position = transform.position + frontPointOffset;
            frontGroundPoint.position = transform.position + frontGroundPointOffset;
        }
        else
        {
            Vector3 fOffset = new Vector3(frontPointOffset.x * -1, frontPointOffset.y, 0);
            Vector3 fgOffset = new Vector3(frontGroundPointOffset.x * -1, frontGroundPointOffset.y, 0);
            frontPoint.position = transform.position + fOffset;
            frontGroundPoint.position = transform.position + fgOffset;
        }
        switch (action)
        {
            case 1:
                idleTimer += Time.deltaTime;
                if (idleTimer > IdleTime)
                {
                    idleTimer = 0f;
                    action = 2;
                }
                break;
            case 2:
                animator.SetBool("isWalk", true);
                feetAni.SetBool("isWalk", true);
                walkTimer += Time.deltaTime;
                transform.position = transform.position + new Vector3(MoveSpeed * Time.deltaTime * dir, 0f, 0f);
                if (walkTimer > WalkTime || colWall || !standGround)
                {
                    walkTimer = 0f;
                    action = 3;
                    animator.SetBool("isWalk", false);
                    feetAni.SetBool("isWalk", false);
                }
                break;
            case 3:
                animator.SetBool("isLookAround", true);
                feetAni.SetBool("isLookAround", true);
                lookAroundTimer += Time.deltaTime;
                if (lookAroundTimer > LookAroundTime)
                {
                    lookAroundTimer = 0f;
                    action = 1;
                    flip = !flip;
                    animator.SetBool("isLookAround", false);
                    feetAni.SetBool("isLookAround", false);
                }
                break;
            default:
                break;
        }
    }

     private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(frontGroundPoint.position, 0.1f);
        Gizmos.DrawWireSphere(frontPoint.position, 0.1f);
    }
}
