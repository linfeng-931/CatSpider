using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartBtn : MonoBehaviour
{
    public GameObject NewGameHint;
    public void NewGameBtn()
    {
        if (PlayerPrefs.HasKey("LastScene"))
        {
            NewGameHint.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene("map1");
        }
    }
    public void LoadGameBtn()
    {
        if (!PlayerPrefs.HasKey("LastScene")) return;

        string lastScene = PlayerPrefs.GetString("LastScene");
        SceneManager.LoadScene(lastScene);
    }
}
