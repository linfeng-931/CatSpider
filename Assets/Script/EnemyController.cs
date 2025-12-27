using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Animator animator;
    public Animator feetAni;
    public Transform frontPoint;
    public Transform frontGroundPoint;
    public Transform centerPoint;
    public float MoveSpeed;
    public float Attack;
    public float LookAroundTime;
    public float WalkTime;
    public float IdleTime;
    public LayerMask groundMask;
    public LayerMask spiderWebMask;
    public Vector3 frontPointOffset;
    public Vector3 frontGroundPointOffset;
    public Transform player;

    private float idleTimer;
    private float walkTimer;
    private float lookAroundTimer;
    private int action; //1-idle, 2-walk, 3-lookAround
    private bool flip;
    private float reMoveSpeed;
    private bool webDebuff;
    private bool isTrace;

    void Start()
    {
        action = 1;
        flip = false;
        webDebuff = false;
    }

    void Update()
    {
        bool standGround = Physics2D.OverlapCircle(frontGroundPoint.position, 0.1f, groundMask);
        bool colWall = Physics2D.OverlapCircle(frontPoint.position, 0.1f, groundMask);
        bool isWeb = Physics2D.OverlapCircle(frontPoint.position, 0.5f, spiderWebMask);

        if (isWeb && !webDebuff)
        {
            reMoveSpeed = MoveSpeed;
            MoveSpeed *= 0.1f;
            webDebuff = true;
        }
        else if(!isWeb && webDebuff)
        {
            MoveSpeed = reMoveSpeed;
            webDebuff = false;
        }

        transform.GetComponent<SpriteRenderer>().flipX = flip;
        int dir = flip ? 1 : -1;

        if(standGround && !colWall) Trace();
        else isTrace = false;

        if(!isTrace)
        {
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

                if (!isTrace)
                {
                    walkTimer += Time.deltaTime;
                    transform.position = transform.position + new Vector3(MoveSpeed * Time.deltaTime * dir, 0f, 0f);
                    if (walkTimer > WalkTime || colWall || !standGround)
                    {
                        walkTimer = 0f;
                        action = 3;
                        animator.SetBool("isWalk", false);
                        feetAni.SetBool("isWalk", false);
                    }
                }
                break;
            case 3:
                if (!Array.Exists(animator.parameters, p => p.name == "isLookAround"))
                {
                    action = 1;
                    return;
                }
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
        Gizmos.DrawWireSphere(centerPoint.position, 0.5f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SpiderWeb"))
        {
            webDebuff = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("SpiderWeb"))
        {
            reMoveSpeed = MoveSpeed;
            if(!webDebuff){
                MoveSpeed = reMoveSpeed;
                webDebuff = false;
            }
        }
    }

    private void Trace()
    {
        if(Vector3.Distance(player.transform.position, transform.position) < 15f && Math.Abs(player.transform.position.y-transform.position.y)<5f)
        {
            isTrace = true;
            animator.SetBool("isLookAround", false);
            feetAni.SetBool("isLookAround", false);
            action = 2;
            if(player.position.x-transform.position.x <= 0) flip = false;
            else flip = true;

            transform.position = Vector3.MoveTowards(transform.position, new Vector3(player.position.x, transform.position.y, transform.position.z), Time.deltaTime*MoveSpeed);
        }
        else
        {
            isTrace = false;
            return;
        }
    }
}
