using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class TunnelInteract : MonoBehaviour
{
    public Transform player;
    public GameObject arrow;
    public Light2D light;
    public Light2D lightArrow;
    public PlayerController playerController;
    public string TargetScene;
    public Animator fade;
    public SaveLoad saveLoad;

    private float upDownMoveTimer;
    private int upDownMoveFlag;
    private float upDownMoveTime;
    private float target;
    private SpriteRenderer spr;
    private SpriteRenderer sprArrow;
    private UnityEngine.Color c;
    private float changeSceneTimer;
    private bool canChange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        upDownMoveFlag = 1;
        upDownMoveTimer = 0f;
        upDownMoveTime = 0.5f;
        target = 0f;
        spr = transform.GetComponent<SpriteRenderer>();
        sprArrow = arrow.transform.GetComponent<SpriteRenderer>();
        c = spr.color;
        canChange = false;
    }

    // Update is called once per frame
    void Update()
    {
        upDownMoveTimer += Time.deltaTime;
        if(upDownMoveTime <= upDownMoveTimer)
        {
            upDownMoveFlag *= -1;
            upDownMoveTimer = 0f;
        }
        arrow.transform.position = new Vector2(arrow.transform.position.x, arrow.transform.position.y+upDownMoveFlag * 0.2f*Time.deltaTime);

        if(Mathf.Abs(player.position.x - transform.position.x) < 10.0f)
        {
            canChange = true;
            if(target < 1f){
                target = target + 2f*Time.deltaTime;
                c.a = target;
                spr.color = c;
                sprArrow.color = c;
                light.intensity = target;
                lightArrow.intensity = target;
            }
            else
            {
                c.a = 1f;
                spr.color = c;
                sprArrow.color = c;
                light.intensity = 1f;
                lightArrow.intensity = 1f;
            }
        }
        else
        {
            canChange = false;
            if(target > 0f){
                target = target - 2f*Time.deltaTime;
                c.a = target;
                spr.color = c;
                sprArrow.color = c;
                light.intensity = target;
                lightArrow.intensity = target;
            }
            else
            {
                c.a = 0f;
                spr.color = c;
                sprArrow.color = c;
                light.intensity = 0f;
                lightArrow.intensity = 0f;
            }
        }

        if (canChange && playerController.isInteract)
        {
            if(changeSceneTimer == 0f)
            {
                fade.SetTrigger("changeScene");
            }
            changeSceneTimer += Time.deltaTime;

            if(changeSceneTimer >= 1.5f)
            {
                saveLoad.Save();
                SceneManager.LoadScene(TargetScene, LoadSceneMode.Single);
                playerController.isInteract = false;
            }
            
        }
    }
}
