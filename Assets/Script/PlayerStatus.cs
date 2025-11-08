using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public GameObject EnergyObj;
    public GameObject BloodObj;
    public PlayerController playerController; //減速改速度時用到
    public RopeSystem ropeSystem;

    //數值與設定
    public float Energy = 10.0f;
    public int Blood = 5;
    public UnityEngine.Color[] blood_color;

    private Transform wave;
    private bool changeWave = false;
    private Vector2 waveTargetPos;
    private float maxMoveRange = 3.0f;
    private float moveRange;
    private float reWaveTimer;

    void Start()
    {
        wave = EnergyObj.transform.GetChild(1);
        waveTargetPos = wave.position;
    }

    void Update()
    {
        WaveControl();
    }
    
    void WaveControl()
    {
        if (ropeSystem.startShoot == 0)
        {
            reWaveTimer += Time.deltaTime;
        }

        if (ropeSystem.startShoot == 1)
        {
            reWaveTimer = 0f;
            ropeSystem.startShoot++;
            Energy -= 3.0f;
            moveRange = (10.0f - Energy) * 0.3f;
        }
        if (reWaveTimer > 4.0f)
        {
            if (Energy < 10f) Energy += 0.005f;
        }

        float energyPercent = Energy / 10f; // 0 ~ 1
        float offsetY = (1f - energyPercent) * maxMoveRange;
        Vector2 targetPos = new Vector2(EnergyObj.transform.position.x, EnergyObj.transform.position.y - offsetY);

        if (Vector2.Distance(wave.position, targetPos) < 0.01) changeWave = false;
        wave.position = Vector2.MoveTowards(wave.position, targetPos, 0.01f);

        //顏色
        if (Energy < 3.0f)
        {
            SpriteRenderer src = EnergyObj.transform.GetChild(1).GetComponent<SpriteRenderer>();
            Color c = src.color;
            c.a = 0.4f;
            src.color = c;
        }
        else
        {
            SpriteRenderer src = EnergyObj.transform.GetChild(1).GetComponent<SpriteRenderer>();
            Color c = src.color;
            c.a = 1f;
            src.color = c;
        }
    }
}
