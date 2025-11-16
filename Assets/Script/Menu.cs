using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Menu : MonoBehaviour
{
    public PlayerStatus playerStatus;
    public TextMeshProUGUI Time;

    public void ShowDetail()
    {
        int min = (int)playerStatus.gameTimer / 60;
        int hour = min / 60;
        int sec = (int)playerStatus.gameTimer - min;
        min = min - hour * 60;

        string sHour = $"{hour:00}";
        string sMin = $"{min:00}";
        string sSec = $"{sec:00}";

        Time.SetText(sHour + " : " + sMin + " : " + sSec);
    }
}
