using System.Drawing;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public float jumpForce = 30f;
    public Transform groundpoint;
    public LayerMask groundMask;
    public bool isSwinging = false;
    public bool releaseSwing = false;
    public Vector2 lastSwingDirection;
    public float releaseForce;
    public Vector2 ropeHook;
    public float swingForce = 4.0f;
    public float InputX;
    public float InputY;
    public bool shrinkLine;
    public bool standGround;
    public GameObject groundPos;

    private Rigidbody2D rig;
    private Animator ani;
    private Animator feetAni;
    private BoxCollider2D col;
    private bool endSwing = true;
    private bool twiceColGround = false;

    //子物件
    private GameObject feet;
    private GameObject hat;
    private GameObject feet_HungUp;
    bool directionFlag = true;
    bool jumpFlag = false;

    void Start()
    {
        releaseSwing = false;
        rig = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        col = GetComponent<BoxCollider2D>();
        feet = transform.GetChild(0).gameObject;
        feetAni = feet.GetComponent<Animator>();
        hat = transform.GetChild(3).gameObject;
        feet_HungUp = transform.GetChild(4).gameObject;
        shrinkLine = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //InputKey = Input.GetKey()
        standGround = Physics2D.OverlapCircle(groundpoint.position, .2f, groundMask);
        if (isSwinging)
        {
            //Swinging動畫與碰撞設定
            if (!standGround)
            {
                ani.SetBool("isSwing", true);
                ani.SetBool("isJump", false);
                ani.SetBool("isWalk", false);
                ani.SetBool("isSquat", false);
                feet.SetActive(false);
                feet_HungUp.SetActive(true);

                col.size = new Vector2(2f, 2.5f);
            }
            else
            {
                ani.SetBool("isSwing", false);
                feet.SetActive(true);
                feet_HungUp.SetActive(false);
                transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
                
                col.size = new Vector2(2.5f, 2f);
            }

            Vector3 dir = transform.GetChild(2).position - transform.position;
            float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
            transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
            
            for(int i = 0; i<4; i++)
            {
                feet_HungUp.transform.GetChild(i).transform.rotation = Quaternion.Euler(0f, 0f, (angle*(-1f)+90f)*0.2f);
            }

            twiceColGround = true;
            endSwing = false;

            //玩家到鉤子的單位方向向量
            Vector2 playerToHookDirection = (ropeHook - (Vector2)transform.position).normalized;
    
            //計算垂直向量
            Vector2 perpendicularDirection = new Vector2(0f, 0f);
            if (InputX < 0)
            {
                GetComponent<SpriteRenderer>().flipX = true;
                feet_HungUp.transform.localScale = new Vector3(-1f, 1f, 1f);

                perpendicularDirection = new Vector2(-playerToHookDirection.y, playerToHookDirection.x);
                Vector2 leftPerpPos = (Vector2)transform.position + perpendicularDirection * -2f;
                if (perpendicularDirection.x > 0) perpendicularDirection.x *= -1;
                Debug.DrawLine(transform.position, leftPerpPos, UnityEngine.Color.white, 0f);
            }
            else if (InputX > 0) //&& !shrinkLine 待修
            {
                GetComponent<SpriteRenderer>().flipX = false;
                feet_HungUp.transform.localScale = new Vector3(1f, 1f, 1f);

                perpendicularDirection = new Vector2(playerToHookDirection.y, -playerToHookDirection.x);
                Vector2 rightPerpPos = (Vector2)transform.position + perpendicularDirection * -2f;
                if (perpendicularDirection.x < 0) perpendicularDirection.x *= -1;
                Debug.DrawLine(transform.position, rightPerpPos, UnityEngine.Color.white, 0f);
            }

            if (InputX != 0)
            {
                Vector2 force = perpendicularDirection * swingForce;
                rig.AddForce(force, ForceMode2D.Force);
            }
            lastSwingDirection = perpendicularDirection;
        }
        else if (releaseSwing)
        {
            if (!standGround)
            {
                ani.SetBool("isSwing", false);
                ani.SetBool("isJump", true);
                feet.SetActive(true);
                feet_HungUp.SetActive(false);
                transform.rotation = Quaternion.LookRotation(new Vector3(0f, 0f, 0f));
            }

            rig.AddForce(lastSwingDirection * releaseForce, ForceMode2D.Impulse);
            releaseSwing = false;
        }
        else if (!endSwing)
        {
            rig.linearVelocity = new Vector2(rig.linearVelocityX + (moveSpeed * InputX * 0.08f), rig.linearVelocityY);
            if (standGround)
            {
                endSwing = true;
            }
        }


        if (endSwing || standGround)
        {
            if (twiceColGround) ResetSwing();
            else
            {
                GetComponent<DistanceJoint2D>().enabled = false;
            }
            Action();
        }
    }

    //角色控制
    public void Move(InputAction.CallbackContext context)
    {
        InputX = context.ReadValue<Vector2>().x;
    }
    public void Squat(InputAction.CallbackContext context)
    {
        InputY = context.ReadValue<Vector2>().y;
    }
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started && jumpFlag)
        {
            rig.linearVelocity = new Vector2(rig.linearVelocity.x, jumpForce);
        }
        if (context.canceled)
        {
            if (rig.linearVelocityY > 0)
            {
                rig.linearVelocity = new Vector2(rig.linearVelocityX, rig.linearVelocityY * 0.25f);
            }
        }
    }
    public void ShrinkLine(InputAction.CallbackContext context)
    {
        if (context.started && isSwinging)
        {
            shrinkLine = true;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundpoint.position, 0.2f);
    }

    //角色基礎動作
    private void Action()
    {
        //左右移動控制
        rig.linearVelocity = new Vector2(moveSpeed * InputX, rig.linearVelocityY);

        ani.SetBool("isSwing", false);
        feet.SetActive(true);
        feet_HungUp.SetActive(false);
        //Walk
        if (InputX > 0)
        {
            if (!directionFlag)
            {
                GetComponent<SpriteRenderer>().flipX = false;
                feet.GetComponent<SpriteRenderer>().flipX = false;
                hat.GetComponent<SpriteRenderer>().flipX = false;
                feet_HungUp.transform.localScale = new Vector3(1f, 1f, 1f);
                directionFlag = true;
            }
            if (jumpFlag)
            {
                ani.SetBool("isWalk", true);
                feetAni.SetBool("isWalk", true);
                feet.SetActive(true);
            }
        }
        else if (InputX < 0)
        {
            if (directionFlag)
            {
                GetComponent<SpriteRenderer>().flipX = true;
                feet.GetComponent<SpriteRenderer>().flipX = true;
                hat.GetComponent<SpriteRenderer>().flipX = true;
                feet_HungUp.transform.localScale = new Vector3(-1f, 1f, 1f);
                directionFlag = false;
            }
            if (jumpFlag)
            {
                ani.SetBool("isWalk", true);
                feetAni.SetBool("isWalk", true);
                feet.SetActive(true);
            }
        }
        else if (jumpFlag)
        {
            ani.SetBool("isWalk", false);
            feetAni.SetBool("isWalk", false);
            feet.SetActive(false);
        }

        //Squat
        if (InputY < 0 && jumpFlag)
        {
            ani.SetBool("isSquat", true);
            col.size = new Vector2(2.5f, 1.0f);
            col.offset = new Vector2(0, -0.6f);
        }
        else if (jumpFlag)
        {
            ani.SetBool("isSquat", false);
            col.size = new Vector2(2.5f, 2.0f);
            col.offset = new Vector2(0, -0.166f);
        }


        //Jump
        jumpFlag = Physics2D.OverlapCircle(groundpoint.position, .2f, groundMask);
        if (!jumpFlag)
        {
            ani.SetBool("isJump", true);
            ani.SetBool("isWalk", false);
            feet.SetActive(true);
            feetAni.SetBool("isJump", true);
            feetAni.SetBool("isWalk", false);
        }
        else
        {
            ani.SetBool("isJump", false);
            feetAni.SetBool("isJump", false);
        }
    }

    void ResetSwing()
    {
        isSwinging = false;
        releaseSwing = false;
        endSwing = true;
        twiceColGround = false;
        shrinkLine = false;
    }
}
