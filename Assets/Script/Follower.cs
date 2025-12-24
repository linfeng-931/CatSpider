using UnityEngine;

public class Follower : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Vector3 offset = new Vector3(0, 5.0f, -10.0f);
    [SerializeField] Transform[] puzzles;

    public int puzzlesIndex;
    public float puzzlesTime;
    public float puzzlesSpeed;
    private float Timer;

    void Start()
    {
        transform.position = player.transform.position + offset;
        puzzlesIndex = -1;
    }

    void LateUpdate()
    {
        if(puzzlesIndex == -1) transform.position = player.transform.position + offset;
        else
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
                    print(puzzlesIndex);
                    Timer = 0;
                }
            }
        }
    }
}
