using UnityEngine;

public class autoSavePoint : MonoBehaviour
{
    public GameObject player;
    public SaveLoad save;
    private bool isSave;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isSave = false; 
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(player.transform.position, transform.position) < 3.0f && !isSave)
        {
            save.Save();
            isSave = true;
        }
    }
}
