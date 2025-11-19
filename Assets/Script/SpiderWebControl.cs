using UnityEngine;
using UnityEngine.InputSystem;

public class SpiderWebControl : MonoBehaviour
{
    public Transform player;
    public Transform anchor;
    public GameObject objPrefab;
    public PlayerStatus playerStatus;
    public int startShoot = 0;

    private float delayTime;
    private float Timer;
    private bool isSpwan;
    private GameObject newWeb;
    private Vector3 target;
    private Vector3 targetScale;

    void Start()
    {
        delayTime = 1.0f;
        Timer = 0f;
        isSpwan = false;
        targetScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    void Update()
    {
        if (isSpwan)
        {
            Timer += Time.deltaTime;
            if(Timer >= delayTime)
            {
                isSpwan = false;
                Timer = 0f;
            }
        }
        if(newWeb != null)
        {
            newWeb.transform.position = Vector3.MoveTowards(newWeb.transform.position, target, 80.0f*Time.deltaTime);
            newWeb.transform.localScale = Vector3.MoveTowards(newWeb.transform.localScale, targetScale, 10.0f*Time.deltaTime);
        }
    }
        

    public void Attack(InputAction.CallbackContext context)
    {
        if(playerStatus.Energy >= 2.0f && !isSpwan)
        {
            newWeb = Instantiate(objPrefab, player.position, Quaternion.identity);
            newWeb.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            target = anchor.position;
            isSpwan = true;
            Destroy(newWeb, 10f);
            startShoot = 1;
        }
    }
}
