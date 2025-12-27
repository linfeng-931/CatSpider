using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoad : MonoBehaviour
{
    public GameObject player;
    public PlayerStatus playerStatus;
    public achivementControl achivementControl;

    public void Awake()
    {
        PlayerPrefs.SetFloat("PlayerPosX", player.transform.position.x);
        PlayerPrefs.SetFloat("PlayerPosY", player.transform.position.y);
    }

    public void Save()
    {
        PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.SetFloat("PlayerPosX", player.transform.position.x);
        PlayerPrefs.SetFloat("PlayerPosY", player.transform.position.y);
        PlayerPrefs.SetInt("Blood", playerStatus.Blood);
        PlayerPrefs.SetInt("DeadCount", playerStatus.deadCount);

        PlayerPrefs.SetFloat("GameTime", playerStatus.gameTimer);
        for(int i = 0; i<achivementControl.actAchivement.Length; i++)
        {
            PlayerPrefs.SetInt("Achivement"+(i+'0'), achivementControl.actAchivement[i]);
        }
        PlayerPrefs.Save();
    }

    public void Load()
    {
        string lastScene = PlayerPrefs.GetString("LastScene");
        float playerPosX = PlayerPrefs.GetFloat("PlayerPosX");
        float playerPosY = PlayerPrefs.GetFloat("PlayerPosY");
        int blood = PlayerPrefs.GetInt("Blood");
    
        player.transform.position = new Vector2(playerPosX, playerPosY);
        playerStatus.Blood = blood;
        playerStatus.gameTimer = PlayerPrefs.GetFloat("GameTime");
        playerStatus.deadCount = PlayerPrefs.GetInt("DeadCount");

        for(int i = 0; i<achivementControl.actAchivement.Length; i++)
        {
            achivementControl.actAchivement[i] = PlayerPrefs.GetInt("Achivement"+(i+'0'), achivementControl.actAchivement[i]);
        }
    }
}
