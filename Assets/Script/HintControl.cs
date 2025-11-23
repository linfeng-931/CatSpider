using TMPro;
using UnityEngine;

public class HintControl : MonoBehaviour
{
    public Transform player;

    private TextMeshPro tmp;
    private RectTransform hintTransform;
    private float target = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tmp = gameObject.GetComponent<TextMeshPro>();
        tmp.alpha = 0f;
        hintTransform = gameObject.GetComponent<RectTransform>();
        gameObject.GetComponent<MeshRenderer>().sortingOrder = 3;
    }

    // Update is called once per frame
    void Update()
    {
        if(Mathf.Abs(player.position.x - hintTransform.position.x) < 10.0f)
        {
            if(target < 1f){
                target = target + 2f*Time.deltaTime;
                tmp.alpha = target;
            }
            else
            {
                tmp.alpha = 1f;
            }
        }
        else
        {
            if(target > 0f){
                target = target - 2f*Time.deltaTime;
                tmp.alpha = target;
            }
            else
            {
                tmp.alpha = 0f;
            }
        }
    }
}
