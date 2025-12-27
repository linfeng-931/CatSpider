using UnityEngine;

public class achivementControl : MonoBehaviour
{
    public int ActIndex;
    public GameObject[] hint;
    public GameObject[] menuList;
    public GameObject[] disActMenuList;
    public Vector3 targetRect;
    public Vector3 originRect;
    public float moveSpeed;
    public int[] actAchivement;
    public SaveLoad saveLoad;

    private float Timer;
    private RectTransform point;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ActIndex = -1;
        Timer = 0;
        actAchivement = new int[menuList.Length];
        
        

        if (PlayerPrefs.HasKey("PlayerPosX"))
        {
            for(int i = 0; i<actAchivement.Length; i++)
            {
                actAchivement[i] = PlayerPrefs.GetInt("Achivement"+(i+'0'));
                if (actAchivement[i] == 1)
                {
                    disActMenuList[i].SetActive(false);
                    menuList[i].SetActive(true);
                }
            }
        }
        else
        {
            for(int i = 0; i<actAchivement.Length; i++)
            {
                actAchivement[i] = 0;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        ActAchivement();
    }

    void ActAchivement()
    {
        if(ActIndex == -1) return;
        if (menuList[ActIndex].activeSelf)
        {
            ActIndex = -1;
            return;
        }

        RectTransform rect = hint[ActIndex].GetComponent<RectTransform>();
        Timer += Time.deltaTime;
        if(Timer > 5.5f)
        {
            hint[ActIndex].SetActive(false);
            menuList[ActIndex].SetActive(true);
            disActMenuList[ActIndex].SetActive(false);
            saveLoad.Save();
            ActIndex = -1;
        }
        if(Timer > 5.0f)
        {
            rect.anchoredPosition = Vector3.MoveTowards(
                rect.anchoredPosition, 
                originRect, 
                moveSpeed * Time.deltaTime
            );
        }
        else
        {
            hint[ActIndex].SetActive(true);
            rect.anchoredPosition = Vector3.MoveTowards(
                rect.anchoredPosition, 
                targetRect, 
                moveSpeed * Time.deltaTime
            );
        }
    }
}
