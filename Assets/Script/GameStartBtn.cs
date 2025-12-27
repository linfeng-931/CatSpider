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
            SceneManager.LoadScene(1);  
        }
    }
    public void LoadGameBtn()
    {
        if (!PlayerPrefs.HasKey("LastScene")) return;

        string lastScene = PlayerPrefs.GetString("LastScene");
        SceneManager.LoadScene(lastScene);
    }

    public void CloseWarning()
    {
        NewGameHint.SetActive(false);
    }
    public void ContinueWarning()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(1);  
    }
    public void Exit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
