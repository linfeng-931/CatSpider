using UnityEngine;

public class Follower : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Vector3 offset = new Vector3(0, 5.0f, -10.0f);
    [SerializeField] Transform[] puzzles;

    public int puzzlesIndex;
    public float puzzlesTime;
    public float puzzlesSpeed;
    public bool isShake;

    private float Timer;
    private float shakeTimer;
    private bool shakeDir;

    void Start()
    {
        transform.position = player.transform.position + offset;
        puzzlesIndex = -1;
        shakeTimer = 0;
        shakeDir = false;
    }

    void LateUpdate()
    {
        if(puzzlesIndex == -1 && !isShake) transform.position = player.transform.position + offset;
        else if(puzzlesIndex != -1)
        {
            Timer+=Time.deltaTime;
            if(Timer<puzzlesTime) transform.position = Vector3.MoveTowards(transform.position, new Vector3(puzzles[puzzlesIndex].position.x, puzzles[puzzlesIndex].position.y,transform.position.z), Time.deltaTime*puzzlesSpeed);
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position + offset,Time.deltaTime*puzzlesSpeed);
                if(Vector3.Distance(player.transform.position + offset, transform.position) < 0.1f)
                {
                    transform.position = player.transform.position + offset;
                    puzzlesIndex = -1;
                    Timer = 0;
                }
            }
        }

        if (isShake)
        {
            shakeTimer += Time.deltaTime;
            if (shakeTimer > 0.15f)
            {
                shakeDir = !shakeDir;
                shakeTimer = 0;
            }

            if(shakeDir) transform.Translate(Vector3.up * Time.deltaTime);
            else transform.Translate(Vector3.down * Time.deltaTime);
        }
    }
}
