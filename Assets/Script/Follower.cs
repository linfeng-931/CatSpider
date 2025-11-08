using UnityEngine;

public class Follower : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Vector3 offset = new Vector3(0, 5.0f, -10.0f);
    void Start()
    {
        transform.position = player.transform.position + offset;
    }

    void LateUpdate()
    {
        transform.position = player.transform.position + offset;
    }
}
