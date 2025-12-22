using UnityEngine;
using UnityEngine.PlayerLoop;

public class SpawnController : MonoBehaviour
{
    public GameObject[] gameObjects;
    public float[] spawnTime;
    public GameObject player;

    private float[] objTimer = {0f, 0f, 0f};

    void Update()
    {
        for(int i = 0; i<spawnTime.Length; i++)
        {
            objTimer[i]+= Time.deltaTime;
            if (objTimer[i] > spawnTime[i])
            {
                sinkGround sinkg = Instantiate(gameObjects[i]).transform.GetChild(0).GetComponent<sinkGround>();
                sinkg.player = this.player;
                objTimer[i] = 0f;
            }
        }
    }
}
