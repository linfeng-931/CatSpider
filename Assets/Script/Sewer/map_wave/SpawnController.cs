using UnityEngine;
using UnityEngine.PlayerLoop;

public class SpawnController : MonoBehaviour
{
    public GameObject[] gameObjects;
    public float[] spawnTime;

    private float[] objTimer = {0f, 0f, 0f};

    void Update()
    {
        for(int i = 0; i<spawnTime.Length; i++)
        {
            objTimer[i]+= Time.deltaTime;
            if (objTimer[i] > spawnTime[i])
            {
                Instantiate(gameObjects[i]);
                objTimer[i] = 0f;
            }
        }
    }
}
