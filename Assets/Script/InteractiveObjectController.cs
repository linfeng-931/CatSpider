using Microsoft.Win32.SafeHandles;
using UnityEngine;
using UnityEngine.Playables;

public class InteractiveObjectController : MonoBehaviour
{
    public GameObject Player;
    public PlayerController playerController;
    public int Type; //0存檔點、1對話、2...
    public SaveLoad saveLoad;

    private GameObject hint;
    private bool hintAct;

    void Start()
    {
        hint = transform.GetChild(0).gameObject;
        hintAct = false;
    }

    void Update()
    {
        //E鍵hint控制
        if (Vector2.Distance(Player.transform.position, transform.position) < 4.0f)
        {
            hintAct = true;
            hint.SetActive(true);
        }
        else
        {
            hintAct = false;
            hint.SetActive(false);
        }

        if (hintAct)
        {
            var hintPos = hint.transform.position;
            var target = new Vector2(hintPos.x, transform.position.y + 4.0f);
            hint.transform.position = Vector2.MoveTowards(hintPos, target, 0.3f);
        }
        else
        {
            var hintPos = hint.transform.position;
            var target = new Vector2(hintPos.x, transform.position.y);
            hint.transform.position = Vector2.MoveTowards(hintPos, target, 0.3f);
        }

        //使用者是否互動
        if (playerController.isInteract)
        {
            playerController.isInteract = false;
            if (hintAct)
            {
                switch (Type)
                {
                    case 0:
                        SavePoint();
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                    default:
                        break;
                }
            }
        }
    }
    
    void SavePoint()
    {
        Player.GetComponent<PlayerStatus>().Blood = 5;
        saveLoad.Save();
    }
}
