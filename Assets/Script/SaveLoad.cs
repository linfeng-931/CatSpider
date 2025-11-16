using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoad : MonoBehaviour
{
    public GameObject player;
    public PlayerStatus playerStatus;

    public void Awake()
    {
        PlayerPrefs.SetFloat("PlayerPosX", player.transform.position.x);
        PlayerPrefs.SetFloat("PlayerPosY", player.transform.position.y);
    }

    public void Save()
    {
        int blood = playerStatus.Blood;
        PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.SetFloat("PlayerPosX", player.transform.position.x);
        PlayerPrefs.SetFloat("PlayerPosY", player.transform.position.y);
        PlayerPrefs.SetInt("Blood", blood);

        PlayerPrefs.SetFloat("GameTime", playerStatus.gameTimer);

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
    }
}
