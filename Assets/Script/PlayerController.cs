using System;
using System.Drawing;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using UnityEditor.Animations;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public float jumpForce;
    public float dashForce;
    public Transform groundpoint;
    public Transform leftCheck;
    public Transform rightCheck;
    public Transform upCheck;
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
    public GameObject upPos;
    public GameObject Head;
    public bool colUp;
    public PlayerStatus playerStatus;
    public bool isHart;
    public bool isInteract;

    private Rigidbody2D rig;
    private Animator ani;
    private Animator feetAni;
    private Animator hatAni;
    private CapsuleCollider2D col;
    public bool endSwing = true;
    private bool twiceColGround = false;
    private bool isDash = false;
    private Vector2 boxSize = new Vector2(0.1f, 2f);
    private Vector2 groundBoxSize = new Vector2(2.3f, 0.1f);
    private RopeSystem ropeSystem;
    private float twiceColGroundTimer = 0f;
    private float hartTimer;
    private bool hartMove = false;
    //private bool autoJump = false;

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
        //DontDestroyOnLoad(gameObject); 場景轉換不消失
        releaseSwing = false;
        rig = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        col = GetComponent<CapsuleCollider2D>();
        feet = transform.GetChild(0).gameObject;
        feetAni = feet.GetComponent<Animator>();
        hat = transform.GetChild(3).gameObject;
        hatAni = hat.GetComponent<Animator>();
        feet_HungUp = transform.GetChild(4).gameObject;
        shrinkLine = false;
        dashObj = transform.GetChild(6).gameObject;
        ropeSystem = GetComponent<RopeSystem>();
        isInteract = false;

        //資料讀取
        string lastScene = PlayerPrefs.GetString("LastScene");
        if (SceneManager.GetActiveScene().name == lastScene)
        {
            float x = PlayerPrefs.GetFloat("PlayerPosX");
            float y = PlayerPrefs.GetFloat("PlayerPosY");
            transform.position = new Vector2(x, y);
            playerStatus.Blood = PlayerPrefs.GetInt("Blood");
        }
    }

    void FixedUpdate()
    {
        standGround = Physics2D.OverlapBox(groundpoint.position, groundBoxSize, .2f, groundMask);
        colUp = Physics2D.OverlapBox(upCheck.position, groundBoxSize, .2f, groundMask);
        transform.GetChild(1).rotation = Quaternion.identity;

        //hart狀態，其他狀態皆抵銷
        if (isHart)
        {
            hartTimer += Time.deltaTime;
            if (hartTimer > 1.0f)
            {
                isHart = false;
            }
            if(!hartMove)
            rig.linearVelocity = new Vector2(0f, 0f);
        }
        else
        {
            if (isSwinging)
            {
                //Swinging動畫與碰撞設定
                if (!standGround)
                {
                    DisAni();
                    BodyAniActive("isSwing");
                    feet.SetActive(false);
                    feet_HungUp.SetActive(true);

                    col.direction = CapsuleDirection2D.Vertical;
                    groundBoxSize = new Vector2(1.0f, 0.5f);

                    //玩家角色角度控制
                    Vector3 dir = transform.GetChild(2).position - transform.position;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

                    for (int i = 0; i < 4; i++)
                    {
                        feet_HungUp.transform.GetChild(i).transform.rotation = Quaternion.Euler(0f, 0f, (angle * (-1f) + 90f) * 0.2f);
                    }

                    //是否為二次觸碰地板
                    if (twiceColGroundTimer < 1.0f) twiceColGroundTimer += Time.deltaTime;
                    if (twiceColGroundTimer > 0.1f)
                    {
                        twiceColGround = true;
                    }
                    endSwing = false;
                }
                else
                {
                    ani.SetBool("isSwing", false);
                    feet.SetActive(true);
                    feet_HungUp.SetActive(false);
                    float front = directionFlag ? 1 : -1;
                    transform.rotation = Quaternion.Euler(front, 0, 0);

                    col.direction = CapsuleDirection2D.Horizontal;
                    groundBoxSize = new Vector2(2.3f, 0.1f);
                    twiceColGroundTimer = 0f;
                }

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
                    ropeSystem.startShoot = 0;
                    float front = directionFlag ? 1 : -1;
                    transform.rotation = Quaternion.Euler(front, 0, 0);
                    twiceColGroundTimer = 0f;
                }

                rig.AddForce(lastSwingDirection * releaseForce, ForceMode2D.Impulse);
                releaseSwing = false;
            }
            else if (!endSwing)
            {
                rig.linearVelocity = new Vector2(rig.linearVelocityX + (moveSpeed * InputX * 0.08f), rig.linearVelocityY);
                canJump = true;
                if (standGround || !canJump)
                {
                    endSwing = true;
                }
            }

            if (standGround && twiceColGround)
            {
                ResetSwing();
                ropeSystem.ResetRope();
            }

            if (endSwing) // && !autoJump
            {
                GetComponent<DistanceJoint2D>().enabled = false;
                HeadControl();
                Action();
            }
        }
    }

    //碰撞偵測
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            isHart = true;
            playerStatus.Blood -= 1;
        }
        if (other.CompareTag("Leakage"))
        {
            isHart = true;
            playerStatus.Blood = 0;
        }
    }
    
    //玩家控制
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
            feetAni.SetBool("isJump", true);
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
        if (context.started && !isDash && !isSwinging)
        {
            isDash = true;
            int front = directionFlag ? 1 : -1;
            if (InputX > 0)
            {
                front = 1;
            }
            else if(InputX < 0)
            {
                front = -1;
            }
            rig.gravityScale = 0f;
            rig.linearVelocity = new Vector2(front * dashForce, 0);
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

    public void Interactive(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isInteract = true;
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(groundpoint.position, groundBoxSize);
        Gizmos.DrawWireCube(upCheck.position, new Vector2(1.5f, 0.2f));
        Gizmos.DrawWireCube(leftCheck.position, boxSize);
        Gizmos.DrawWireCube(rightCheck.position, boxSize);
    }

    //動作函式
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
        if (IsTouchingWallLeft() && !standGround)
        {
            rig.linearVelocity = new Vector2(0.0001f, rig.linearVelocityY);
        }
        else if (IsTouchingWallRight() && !standGround)
        {
            rig.linearVelocity = new Vector2(-0.0001f, rig.linearVelocityY);
        }
        else rig.linearVelocity = new Vector2(moveSpeed * InputX, rig.linearVelocityY);
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
        else if (standGround)
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
            if (!isDash) BodyAniActive("isJump");
            feet.SetActive(true);
            feetAni.SetBool("isJump", true);
            feetAni.SetBool("isWalk", false);
        }
        else
        {
            ani.SetBool("isJump", false);
            hatAni.SetBool("isJump", false);
            feetAni.SetBool("isJump", false);
            if (jumpDelayTime > 0.1f || standGround)
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

        /*if (IsTouchingWallLeft()) dash撞牆問題待修
        {
            rig.linearVelocity = new Vector2(0.0001f, rig.linearVelocityY);
            isDash = false;
        }
        else if (IsTouchingWallRight())
        {
            rig.linearVelocity = new Vector2(-0.0001f, rig.linearVelocityY);
            isDash = false;
        }*/

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
            rig.linearVelocity = new Vector2(0, 0);
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
        col.direction = CapsuleDirection2D.Horizontal;
        groundBoxSize = new Vector2(2.3f, 0.1f);
        canJump = true;
        twiceColGroundTimer = 0f;
        ropeSystem.startShoot = 0;
    }

    //Shoot角色頭部動作
    void HeadControl()
    {
        //取得玩家是否正在射擊狀態
        bool readyShoot = ropeSystem.readyShoot;
        var headSpriteRenderer = Head.GetComponent<SpriteRenderer>();
        var hatSpriteRenderer = Head.transform.GetChild(0).GetComponent<SpriteRenderer>();

        //1
        if (!readyShoot || isSwinging)
        {
            Head.SetActive(false);
            hat.SetActive(true);
            headSpriteRenderer.enabled = false;
            Head.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            return;
        }

        //2
        float aimAngle = ropeSystem.aimAngle * Mathf.Rad2Deg;
        float range = 30.0f;

        if (aimAngle < range || aimAngle > 360f - range)
        {
            Head.SetActive(true);
            headSpriteRenderer.flipX = false;
            hatSpriteRenderer.flipX = false;
            Head.transform.rotation = Quaternion.Euler(0f, 0f, aimAngle);
            hat.SetActive(false);
        }
        else if (aimAngle > 180f - range && aimAngle < 180f + range)
        {
            Head.SetActive(true);
            headSpriteRenderer.flipX = true;
            hatSpriteRenderer.flipX = true;
            Head.transform.rotation = Quaternion.Euler(0f, 0f, aimAngle - 180.0f);
            hat.SetActive(false);
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

    bool IsTouchingWallLeft()
    {
        return Physics2D.OverlapBox(leftCheck.position, boxSize, 0f, groundMask);
    }

    bool IsTouchingWallRight()
    {
        return Physics2D.OverlapBox(rightCheck.position, boxSize, 0f, groundMask);
    }
}
