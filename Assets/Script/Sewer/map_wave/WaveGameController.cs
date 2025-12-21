using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class WaveGameController : MonoBehaviour
{
    public BuoyancyEffector2D[] water;
    public TilemapCollider2D[] map;
    public GameObject player;
    public Vector3 maxSize = new Vector3(1.5f, 1.5f, 1f);
    public Vector3 minSize = new Vector3(0.5f, 0.5f, 1f);
    public PlayerInput playerInput;

    private int index = 1; //0近,1,2遠
    private bool turnLine = false;
    private string[] waterLayer = {"WaterLayer0", "WaterLayer1", "WaterLayer2"};
    private int[] srOrder = {8, 5, 2};

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput.SwitchCurrentActionMap("Player_waveGame"); 
        int playerLayer = LayerMask.NameToLayer("Default");
        int waterl = LayerMask.NameToLayer(waterLayer[0]);
        Physics2D.IgnoreLayerCollision(playerLayer, waterl, true);
        waterl = LayerMask.NameToLayer(waterLayer[2]);
        Physics2D.IgnoreLayerCollision(playerLayer, waterl, true);
    }

    // Update is called once per frame
    void Update()
    {
        if (turnLine)
        {
            TurnLine();
        }
    }

    public void WControl(InputAction.CallbackContext context)
    {
        if(player.GetComponent<PlayerController>().isSwinging){
            player.GetComponent<PlayerController>().InputY = context.ReadValue<Vector2>().y;
            return;
        }
        player.GetComponent<PlayerController>().InputY = 0;

        if(context.ReadValue<Vector2>().y == 0 || turnLine || player.GetComponent<PlayerController>().isHurt) return;

        else
        {
            float input = context.ReadValue<Vector2>().y;
            if(index == 2 && input>0) return;
            else if(index == 0 && input<0) return;

            map[index].enabled = false;
            water[index].colliderMask &= ~(1 << 0); //去掉default layer
            
            //碰撞項設定
            int playerLayer = LayerMask.NameToLayer("Default");
            int waterl = LayerMask.NameToLayer(waterLayer[index]);
            Physics2D.IgnoreLayerCollision(playerLayer, waterl, true);

            index += input > 0? 1: -1; //更改層

            map[index].enabled = true;
             water[index].colliderMask |= 1 << 0;
            waterl = LayerMask.NameToLayer(waterLayer[index]);
            Physics2D.IgnoreLayerCollision(playerLayer, waterl, false);
            SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
            sr.sortingOrder = srOrder[index];
            sr = player.transform.GetChild(0).GetComponent<SpriteRenderer>();
            sr.sortingOrder = srOrder[index];   
            sr = player.transform.GetChild(3).GetComponent<SpriteRenderer>();
            sr.sortingOrder = srOrder[index]+1;   
            
            turnLine = true;
        }
    }

    void TurnLine()
    {
        Vector3 targetScale = new Vector3(1f,1f,1f);
        switch (index)
        {
            case 0:
                targetScale = maxSize;
                break;
            case 2:
                targetScale = minSize;
                break;
            default:
                break;
        }
        player.transform.localScale = Vector3.MoveTowards(player.transform.localScale, targetScale, Time.deltaTime*20f);

        if (Vector3.Distance(player.transform.localScale, targetScale) < 0.01f)
        {
            player.transform.localScale = targetScale;
            turnLine = false;
        }
    }
}
