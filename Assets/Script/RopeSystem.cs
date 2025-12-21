using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using Mono.Cecil.Cil;
using Unity.VisualScripting;

public class RopeSystem : MonoBehaviour
{
    [SerializeField] GameObject ropeHingeAnchor;
    [SerializeField] GameObject circleMask;
    [SerializeField] DistanceJoint2D ropeJoint;
    [SerializeField] Transform crosshair;
    [SerializeField] GameObject crosshairFaceObj;
    [SerializeField] Transform crosshairFace;
    [SerializeField] SpriteRenderer crosshairSprite;
    [SerializeField] PlayerController playerMovement;
    [SerializeField] LineRenderer ropeRenderer;
    [SerializeField] LayerMask ropeLayerMask;
    [SerializeField] float maxDirection;
    [SerializeField] float minDirection;
    [SerializeField] float climbSpeed = 30f;
    [SerializeField] LayerMask canMoveMask;
   
    public bool readyShoot;
    public float aimAngle;
    public PlayerStatus playerStatus;
    public int startShoot = 0;
    public float pullForce = 10f;

    private bool ropeAttached;
    private Vector2 playerPosition;
    private Rigidbody2D ropeHingeAnchorRb;
    private List<Vector2> ropePositions = new List<Vector2>();
    private bool distanceSet;
    private bool isColliding;
    private bool shrinkComplete;
    private float shootTimer = 3f;
    private bool overDistance = false;
    private bool udFlag = false;
    private Vector2 normalScale = new Vector2(0.05f, 0.05f);
    private Vector2 hitScale = new Vector2(0.15f, 0.15f);
    private UnityEngine.Color normalColor = new Color(1f,1f,1f,1f);
    private UnityEngine.Color canMoveHitColor = new Color(1f, 0.741f, 0.231f, 1f);
    private bool isPull = false;
    private RaycastHit2D moveObj;
    private bool moveOverDistance = false;

    void Awake()
    {
        ropeJoint.enabled = false;
        ropeAttached = false;
        readyShoot = false;
        shrinkComplete = false;
        playerPosition = transform.position;
        ropeHingeAnchorRb = ropeHingeAnchor.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //獲取滑鼠座標
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));

        //取得玩家與鼠標的距離
        Vector3 facingDirection = worldMousePosition - transform.position;

        //取得玩家與鼠標的角度並維持為正
        aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);
        if (aimAngle < 0.0f)
        {
            aimAngle = Mathf.PI * 2 + aimAngle; //如果是負值加2拍
        }

        //計算瞄準方向並記在四元數中
        var aimDirection = Quaternion.Euler(0, 0, aimAngle * Mathf.Rad2Deg) * Vector2.right;

        playerPosition = transform.position;
        SetCrosshairPosition(aimAngle, worldMousePosition);
        Handle(aimDirection, crosshair.transform.position);

        if (Input.GetMouseButton(0) && !readyShoot && shootTimer > 0.5f && playerStatus.Energy >= 3f && !playerMovement.isHurt)
        {
            readyShoot = true;
            playerMovement.isShoot = true;
            playerMovement.Head.SetActive(true);
        }
        if(Input.GetMouseButtonUp(0))
        {
            playerMovement.isShoot = false;
            playerMovement.Head.SetActive(false);
        }

        if (readyShoot)
        {
            if (!Input.GetMouseButton(0))
            {
                readyShoot = false;
                HandleInput(aimDirection, crosshair.transform.position);
            }
        }

        UpdateRopePositions();
        HandleRopeLength();
        MoveObject();

        if ((Input.GetMouseButton(0) && playerMovement.isSwinging) || playerMovement.isHurt || (Input.GetMouseButton(0) && isPull) || Vector2.Distance(ropeRenderer.GetPosition(0), ropeRenderer.GetPosition(1)) > 20f)
        {
            shootTimer = 0f;
            startShoot = 0;
            isPull = false;
            ResetRope();
        }
        if (!readyShoot && shootTimer < 2.0f)
        {
            shootTimer += Time.deltaTime;
        }
    }

    private void SetCrosshairPosition(float aimAngle, Vector3 mousePosition)
    {
        float dir = Mathf.Sqrt(Mathf.Pow(mousePosition.x - transform.position.x, 2) + Mathf.Pow(mousePosition.y - transform.position.y, 2));

        float x, y;
        if (dir < maxDirection && dir > minDirection)
        {
            x = mousePosition.x;
            y = mousePosition.y;
        }
        else if (dir >= 7.0f)
        {
            x = transform.position.x + maxDirection * Mathf.Cos(aimAngle);
            y = transform.position.y + maxDirection * Mathf.Sin(aimAngle);
        }
        else
        {
            x = transform.position.x + minDirection * Mathf.Cos(aimAngle);
            y = transform.position.y + minDirection * Mathf.Sin(aimAngle);
        }

        var crosshairPosition = new Vector3(x, y, 0);
        crosshair.position = crosshairPosition;
        crosshairFace.position = crosshairPosition;
        circleMask.transform.position = crosshairPosition;
    }

    //判斷線是否射到某點
    private void HandleInput(Vector2 aimDirection, Vector3 aimPosition)
    {
        if (ropeAttached) return;

        ropeRenderer.enabled = true;
        float dir = Mathf.Sqrt(Mathf.Pow(aimPosition.x - transform.position.x, 2) + Mathf.Pow(aimPosition.y - transform.position.y, 2));
        var hit = Physics2D.Raycast(playerPosition, aimDirection, dir, ropeLayerMask);
        var moveObjHit = Physics2D.Raycast(playerPosition, aimDirection, dir, canMoveMask);

        if (hit.collider != null && !hit.collider.CompareTag("disAttached") && !hit.collider.CompareTag("Leakage"))
        {
            if (startShoot == 0) startShoot = 1;
            playerMovement.isSwinging = true;
            ropeAttached = true;
            if (!ropePositions.Contains(hit.point))
            {
                ropePositions.Add(hit.point);
                ropeJoint.distance = Vector2.Distance(playerPosition, hit.point);
                ropeJoint.enabled = true;
            }
        }
        else if(moveObjHit.collider != null)
        {
            ropeAttached = true;
            isPull = true;
            if (!ropePositions.Contains(moveObjHit.point))
            {
                ropePositions.Add(moveObjHit.point);
                ropeJoint.distance = Vector2.Distance(playerPosition, moveObjHit.point);
                ropeJoint.enabled = true;
                moveObj = moveObjHit;
            }
        }
        else
        {
            ropeRenderer.enabled = false;
            ropeAttached = false;
            ropeJoint.enabled = false;
        }
    }
    
    private void Handle(Vector2 aimDirection, Vector3 aimPosition)
    {
        if (playerStatus.Energy < 3.0f)
        {
            crosshairFaceObj.SetActive(false);
            return;
        }
        else
        {
            crosshairFaceObj.SetActive(true);
        }

        float dir = Mathf.Sqrt(Mathf.Pow(aimPosition.x - transform.position.x, 2) + Mathf.Pow(aimPosition.y - transform.position.y, 2));
        var hit = Physics2D.Raycast(playerPosition, aimDirection, dir, ropeLayerMask);
        var moveObjHit = Physics2D.Raycast(playerPosition, aimDirection, dir, canMoveMask);
         
        if (hit.collider != null && !hit.collider.CompareTag("disAttached") && !hit.collider.CompareTag("Leakage"))
        {
            crosshairFace.localScale = hitScale;
            crosshairFace.position = hit.point;
            crosshairSprite.color = normalColor;
            circleMask.SetActive(false);
        }
        else if(moveObjHit.collider != null)
        {
            crosshairFace.localScale = hitScale;
            crosshairFace.position = moveObjHit.point;
            crosshairSprite.color = canMoveHitColor;
            circleMask.SetActive(false);
        }
        else
        {
            circleMask.SetActive(true);
            crosshairSprite.color = normalColor;
            crosshairFace.localScale = normalScale;
        }
    }

    public void ResetRope()
    {
        ropeJoint.enabled = false;
        ropeAttached = false;
        playerMovement.isSwinging = false;
        ropeRenderer.positionCount = 2;
        ropeRenderer.SetPosition(0, transform.position);
        ropeRenderer.SetPosition(1, transform.position);
        ropePositions.Clear();
        playerMovement.releaseSwing = true;
        shrinkComplete = false;
        moveOverDistance = false;
        overDistance = false;
    }

    //更新線的狀態
    private void UpdateRopePositions()
    {
        if (!ropeAttached)
        {
            return;
        }

        ropeRenderer.positionCount = ropePositions.Count + 1;
        for (var i = ropeRenderer.positionCount - 1; i >= 0; i--)
        {
            if (i != ropeRenderer.positionCount - 1)
            {
                ropeRenderer.SetPosition(i, ropePositions[i]);
                if (i == ropePositions.Count - 1 || ropePositions.Count == 1)
                {
                    var ropePosition = ropePositions[ropePositions.Count - 1];
                    ropeHingeAnchorRb.transform.position = ropePosition;
                    if (!distanceSet)
                    {
                        ropeJoint.distance = Vector2.Distance(transform.position, ropePosition);
                        distanceSet = true;
                    }
                }
                else if (i - 1 == ropePositions.IndexOf(ropePositions.Last()))
                {
                    var ropePosition = ropePositions.Last();
                    ropeHingeAnchorRb.transform.position = ropePosition;
                    if (!distanceSet)
                    {
                        ropeJoint.distance = Vector2.Distance(transform.position, ropePosition);
                        distanceSet = true;
                    }
                }
            }
            else
            {
                ropeRenderer.SetPosition(i, transform.position);
            }
        }
    }

    private void HandleRopeLength()
    {
        if (isPull) return;

        if (playerMovement.shrinkLine)
        {
            if (!shrinkComplete)
            {
                ropeJoint.distance = Mathf.MoveTowards(ropeJoint.distance, 0f, Time.deltaTime * 50f);
                float distance = Vector2.Distance(transform.position, crosshair.position);
                if (distance < 1.0f)
                {
                    shrinkComplete = true;
                    ResetRope();
                }
            }
        }

        Vector2 pos = playerMovement.groundpoint.position;
        if (playerMovement.InputY > 0f && ropeAttached && !playerMovement.colUp)
        {
            
            if (!udFlag)
            {
                playerMovement.groundpoint.position = new Vector2(playerPosition.x, playerPosition.y);
                udFlag = true;
            }
            if (playerMovement.standGround)
            {
                GetComponent<DistanceJoint2D>().enabled = true;
                playerMovement.isSwinging = true;
                ropeJoint.distance -= Time.deltaTime * climbSpeed * 30f;
                playerMovement.twiceColGround = true;
            }
            else
            {
                ropeJoint.distance -= Time.deltaTime * climbSpeed;
            } 
        }
        else if (playerMovement.InputY < 0f && ropeAttached)
        {
            overDistance = Vector2.Distance(ropeRenderer.GetPosition(0), ropeRenderer.GetPosition(1)) > 10f ? true : false;
            if (!overDistance)
            {
                ropeJoint.distance += Time.deltaTime * climbSpeed;
                if (udFlag)
                {
                    playerMovement.groundpoint.position = new Vector2(playerPosition.x, playerPosition.y - 1.7f);
                    udFlag = false;
                }
            }
        }
        else
        {
            if (udFlag)
            {
                playerMovement.groundpoint.position = new Vector2(playerPosition.x, playerPosition.y-1.7f);
                udFlag = false;
            }  
        }
    }

    void MoveObject()
    {
        if (!isPull) return;

        Rigidbody2D hitRb = moveObj.collider.attachedRigidbody;
        if (hitRb != null)
        {
            Vector2 ropeEnd = hitRb.position;
            ropeJoint.distance = Vector2.Distance(playerPosition, ropeEnd);
            crosshair.position = ropeEnd;
            ropeRenderer.SetPosition(0, ropeEnd);
            Debug.DrawLine(transform.position, ropeEnd, UnityEngine.Color.white);
            moveOverDistance = Vector2.Distance(ropeRenderer.GetPosition(0), ropeRenderer.GetPosition(1)) > 10f ? true : false;

            if (playerMovement.InputY > 0f || moveOverDistance)
            {
                Vector2 pullDir = (playerPosition - ropeEnd).normalized;
                hitRb.AddForce(pullDir * pullForce, ForceMode2D.Force);
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        isColliding = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        isColliding = false;
    }
}
