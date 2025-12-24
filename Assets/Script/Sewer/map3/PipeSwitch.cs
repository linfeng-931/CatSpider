using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PipeSwitch : MonoBehaviour
{
    public GameObject[] waterB, waterY, waterG;
    public Light2D lightB, lightY, lightG;
    public PlayerController playerController;
    public Follower camera;
    public bool type_b, type_y, type_g;
    public bool active_b, active_y, active_g;

    private WaterSquirtController[] waterBController, waterYController, waterGController;
    private bool canAct;
    private float rotateSpeed;
    private bool isRotate;
    private int rdir;
    private float changeTimer;
    private bool changeWater;

    static bool sb,sy,sg;
    static int allchange;
    static int amount = 4;

    void Start()
    {
        sb = false;
        sy = false;
        sg = false;

        canAct = false;
        rotateSpeed = 100f;
        isRotate = false;
        rdir = 1;

        waterBController = new WaterSquirtController[waterB.Length];
        waterYController = new WaterSquirtController[waterY.Length];
        waterGController = new WaterSquirtController[waterG.Length];

        if(waterB!=null && lightB != null){
            for(int i = 0; i<waterB.Length; i++)
            {
                waterBController[i] = waterB[i].GetComponent<WaterSquirtController>();
                waterBController[i].Active = active_b;
            }
            lightB.enabled = active_b;
        }
        if(waterY!=null && lightY != null){
            for(int i = 0; i<waterY.Length; i++)
            {
                waterYController[i] = waterY[i].GetComponent<WaterSquirtController>();
                waterYController[i].Active = active_y;
            }
            lightY.enabled = active_y;
        }
        if(waterG!=null && lightG != null){
            for(int i = 0; i<waterG.Length; i++)
            {
                waterGController[i] = waterG[i].GetComponent<WaterSquirtController>();
                waterGController[i].Active = active_g;
            }
            lightG.enabled = active_g;
        }
        
        changeTimer = 0;
        changeWater = false;
        allchange = 0;
    }


    void Update()
    {
        if (canAct && playerController.isInteract)
        {
            isRotate = true;
            playerController.isInteract = false;
        }

        if(allchange!=0 && allchange<amount && !changeWater){
            changeWater = true;
            allchange++;
        }
        if(allchange == amount) allchange = 0;

        objRotate();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            canAct = true;
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            canAct = false;
        }
    }

    void objRotate()
    {
        if (changeWater)
        {
            if(changeTimer == 0)
            {
                if(sb && type_b) lightB.enabled = !lightB.enabled;
                if(sy && type_y) lightY.enabled = !lightY.enabled;
                if(sg && type_g) lightG.enabled = !lightG.enabled;
            }
            changeTimer+=Time.deltaTime;
            if(changeTimer > 1f)
            {
                if(sb && waterB!=null){
                    active_b = !active_b;
                    for(int i=0; i<waterB.Length; i++)
                    {
                        waterBController[i].Active = active_b;
                    }
                }
                if(sy && waterY != null)
                {
                    active_y = !active_y;
                    for(int i=0; i<waterY.Length; i++)
                    {
                        waterYController[i].Active = active_y;
                    }
                }
                if(sg && waterG != null)
                {
                    active_g = !active_g;
                    for(int i=0; i<waterG.Length; i++)
                    {
                        waterGController[i].Active = active_g;
                    }
                }

                changeTimer = 0;
                changeWater = false;
                return;
            }
        }
        if(!isRotate) return;
        if (rdir < 0)
        {
            if(transform.eulerAngles.z > 10f)
            {
                transform.Rotate(0, 0, rdir*rotateSpeed*Time.deltaTime);
            }
            else
            {
                if(camera.puzzlesIndex == -1) camera.puzzlesIndex = 0;
                rdir*=-1;
                isRotate = false;
                changeWater = true;
                if(type_b) sb = true;
                else sb = false;
                if(type_y) sy = true;
                else sy = false;
                if(type_g) sg = true;
                else sg = false;
                allchange=1;
                return;
            }
        }
        else
        {
            if(transform.eulerAngles.z < 180f)
            {
                transform.Rotate(0, 0, rdir*rotateSpeed*Time.deltaTime);
            }
            else
            {
                if(camera.puzzlesIndex == -1) camera.puzzlesIndex = 0;
                rdir*=-1;
                isRotate = false;
                changeWater = true;
                if(type_b) sb = true;
                else sb = false;
                if(type_y) sy = true;
                else sy = false;
                if(type_g) sg = true;
                else sg = false;
                allchange=1;
                return;
            }
        } 
    }
}
