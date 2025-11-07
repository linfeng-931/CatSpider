using System;
using System.Drawing;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public float jumpForce;
    public float dashForce;
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
    public GameObject Head;

    private Rigidbody2D rig;
    private Animator ani;
    private Animator feetAni;
    private Animator hatAni;
    private BoxCollider2D col;
    private bool endSwing = true;
    private bool twiceColGround = false;
    private bool isDash = false;

    //子物件
    private GameObject feet;
    private GameObject hat;
    private GameObject feet_HungUp;
    private GameObject dashObj;
    bool directionFlag = true; //true右、false左
    bool canJump = true; //true為可跳躍狀態
    float jumpDelayTime = 0f;
    float dashDistance = 0f;
    Vector2 dashOriginalPoint;


    void Start()
    {
        releaseSwing = false;
        rig = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        col = GetComponent<BoxCollider2D>();
        feet = transform.GetChild(0).gameObject;
        feetAni = feet.GetComponent<Animator>();
        hat = transform.GetChild(3).gameObject;
        hatAni = hat.GetComponent<Animator>();
        feet_HungUp = transform.GetChild(4).gameObject;
        shrinkLine = false;
        dashObj = transform.GetChild(6).gameObject;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //InputKey = Input.GetKey()
        standGround = Physics2D.OverlapCircle(groundpoint.position, .2f, groundMask);
        //canJump = standGround;

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

        if (endSwing)
        {
            if (twiceColGround) ResetSwing();
            else
            {
                GetComponent<DistanceJoint2D>().enabled = false;
            }
            HeadControl();
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
        if (context.started && canJump)
        {
            rig.linearVelocity = new Vector2(rig.linearVelocity.x, jumpForce);
            canJump = false;
            feetAni.SetBool("isJump", false);
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
    public void Dash(InputAction.CallbackContext context)
    {
        if (context.started && !isDash)
        {
            isDash = true;
            int dir = directionFlag ? 1 : -1;
            rig.gravityScale = 0f;
            rig.linearVelocity = new Vector2(dir * dashForce, 0);
            dashOriginalPoint = transform.position;
            dashObj.SetActive(true);
            DisAni();
            BodyAniActive("isDash");
            feet.SetActive(true);
            feetAni.SetBool("isDash", true);
            col.size = new Vector2(2.5f, 1.0f);
            col.offset = new Vector2(0, -0.6f);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundpoint.position, 0.2f);
    }

    //角色基礎動作
    private void Action()
    {
        ani.SetBool("isSwing", false);
        feet.SetActive(true);
        feet_HungUp.SetActive(false);
        
        SquatAction();
        JumpAction();
        DashAction();
        MoveAction();
    }
    private void MoveAction()
    {
        if (isDash) return;
        //左右移動控制
        rig.linearVelocity = new Vector2(moveSpeed * InputX, rig.linearVelocityY);
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
            if (standGround)
            {
                BodyAniActive("isWalk");
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
            if (standGround)
            {
                BodyAniActive("isWalk");
                feetAni.SetBool("isWalk", true);
                feet.SetActive(true);
            }
        }
        else if(standGround)
        {
            ani.SetBool("isWalk", false);
            hatAni.SetBool("isWalk", false);
            feetAni.SetBool("isWalk", false);
            feet.SetActive(false);
        }
    }
    private void JumpAction()
    {
        if (!standGround)
        {
            jumpDelayTime += Time.deltaTime;
            if(!isDash) BodyAniActive("isJump");
            feet.SetActive(true);
            feetAni.SetBool("isJump", true);
            feetAni.SetBool("isWalk", false);
        }
        else
        {
            ani.SetBool("isJump", false);
            hatAni.SetBool("isJump", false);
            feetAni.SetBool("isJump", false);
            if (jumpDelayTime > 0.5f)
            {
                jumpDelayTime = 0f;
                canJump = true;
            }
        }
    }
    private void SquatAction()
    {
        if (isDash) return;
        if (InputY < 0 && standGround)
        {
            BodyAniActive("isSquat");
            col.size = new Vector2(2.5f, 1.0f);
            col.offset = new Vector2(0, -0.6f);
        }
        else if (standGround)
        {
            ani.SetBool("isSquat", false);
            hatAni.SetBool("isSquat", false);
            col.size = new Vector2(2.5f, 2.0f);
            col.offset = new Vector2(0, -0.166f);
        }
    }
    private void SwingAction(bool flag)
    {
        if (!flag) return;
    }
    private void DashAction()
    {
        if (!isDash) return;

        dashDistance = Vector2.Distance(transform.position, dashOriginalPoint);
        if (dashDistance > 10.0f)
        {
            isDash = false;
            rig.gravityScale = 5f;
            dashObj.SetActive(false);
            ani.SetBool("isDash", false);
            hatAni.SetBool("isDash", false);
            feetAni.SetBool("isDash", false);
            feet.SetActive(false);
            col.size = new Vector2(2.5f, 2.0f);
            col.offset = new Vector2(0, -0.166f);
        }
    }
    

    //重置Swing
    void ResetSwing()
    {
        isSwinging = false;
        releaseSwing = false;
        endSwing = true;
        twiceColGround = false;
        shrinkLine = false;
    }

    //Shoot角色頭部動作
    void HeadControl()
    {
        //取得玩家是否正在射擊狀態
        var ropeSystem = GetComponent<RopeSystem>();
        bool readyShoot = ropeSystem.readyShoot;
        var headSpriteRenderer = Head.GetComponent<SpriteRenderer>();

        //1
        if (!readyShoot || isSwinging)
        {
            headSpriteRenderer.enabled = false;
            Head.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            return;
        }

        //2
        float aimAngle = ropeSystem.aimAngle * Mathf.Rad2Deg;
        float range = 30.0f;

        if (aimAngle < range || aimAngle > 360f - range)
        {
            headSpriteRenderer.flipX = false;
            Head.transform.rotation = Quaternion.Euler(0f, 0f, aimAngle);
        }
        else if (aimAngle > 180f - range && aimAngle < 180f + range)
        {
            headSpriteRenderer.flipX = true;
            Head.transform.rotation = Quaternion.Euler(0f, 0f, aimAngle - 180.0f);
        }
        headSpriteRenderer.enabled = true;

    }

    //動畫控制
    private void DisAni()
    {
        ani.SetBool("isWalk", false);
        ani.SetBool("isSquat", false);
        ani.SetBool("isJump", false);
        ani.SetBool("isSwing", false);
        ani.SetBool("isShoot", false);
        ani.SetBool("isDash", false);

        hatAni.SetBool("isWalk", false);
        hatAni.SetBool("isSquat", false);
        hatAni.SetBool("isJump", false);
        hatAni.SetBool("isSwing", false);
        hatAni.SetBool("isShoot", false);
        hatAni.SetBool("isDash", false);
    }
    
    private void BodyAniActive(String act)
    {
        switch (act)
        {
            case "isWalk":
                ani.SetBool("isWalk", true);
                hatAni.SetBool("isWalk", true);
                break;
            case "isSquat":
                ani.SetBool("isSquat", true);
                hatAni.SetBool("isSquat", true);
                break;
            case "isJump":
                ani.SetBool("isJump", true);
                hatAni.SetBool("isJump", true);
                break;
            case "isSwing":
                ani.SetBool("isSwing", true);
                hatAni.SetBool("isSwing", true);
                break;
            case "isShoot":
                ani.SetBool("isShoot", true);
                hatAni.SetBool("isShoot", true);
                break;
            case "isDash":
                ani.SetBool("isDash", true);
                hatAni.SetBool("isDash", true);
                break;
            default:
                break;
        }  
    }
}
