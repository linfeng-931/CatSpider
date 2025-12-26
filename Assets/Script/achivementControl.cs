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

    private float Timer;
    private RectTransform point;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ActIndex = -1;
        Timer = 0;
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
