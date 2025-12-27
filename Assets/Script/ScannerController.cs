using UnityEngine;
using UnityEngine.InputSystem;

public class ScannerController : MonoBehaviour
{
    public Transform[] CollectionPoint;
    public Transform[] NpcPoint;
    public Transform PlayerPos;
    public float InputX;
    public float InputY;
    public float moveDistance;
    public RectTransform rotateObj;
    public float rotateSpeed;

    private float range;
    private GameObject collection;
    private GameObject npc;
    private GameObject player;
    private Vector3 pointScale = new Vector3(0.02f, 0.02f, 1f);

    void Start()
    {
        range = 35f;
        collection = transform.GetChild(2).gameObject;
        npc = transform.GetChild(1).gameObject;
        player = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        DrawPoint();
        if(InputX !=0 || InputY != 0)
        {
            var pos = GetComponent<RectTransform>().anchoredPosition;
            if (pos.x > moveDistance && InputX > 0) InputX = 0;
            else if (pos.x < -1*moveDistance && InputX < 0) InputX = 0;

            if (pos.y > moveDistance && InputY > 0) InputY = 0;
            else if (pos.y < -1*moveDistance && InputY < 0) InputY = 0;

            GetComponent<RectTransform>().anchoredPosition = new Vector2(pos.x+InputX, pos.y+InputY);
        }
        RotateObj();
    }

    public void Move(InputAction.CallbackContext context)
    {
        InputX = context.ReadValue<Vector2>().x;
        InputY = context.ReadValue<Vector2>().y;
    }

    private void DrawPoint()
    {
        for (int i = 0; i < CollectionPoint.Length; i++)
        {
            if(CollectionPoint[i] != null){
                RectTransform collectionTransform = collection.transform.GetChild(i).GetComponent<RectTransform>();
                //點位置
                var dir = (CollectionPoint[i].position - PlayerPos.position).normalized;
                var distance = Vector2.Distance(PlayerPos.position, CollectionPoint[i].position);
                var distancePercent = Mathf.Clamp01(distance / range);
                distance = distancePercent * range;
                Vector2 offset = new Vector2((distance * dir).x, (distance * dir).y);
                collectionTransform.anchoredPosition = player.GetComponent<RectTransform>().anchoredPosition + offset;

                //點大小
                var scale = 1f + ((1 - distancePercent) * 2);
                collectionTransform.localScale = new Vector3(pointScale.x * scale, pointScale.y * scale, pointScale.z);
            }
        }

        for (int i = 0; i < NpcPoint.Length; i++)
        {
            if(NpcPoint[i] != null){
                RectTransform npcTransform = npc.transform.GetChild(i).GetComponent<RectTransform>();
                //點位置
                var dir = (NpcPoint[i].position - PlayerPos.position).normalized;
                var distance = Vector2.Distance(PlayerPos.position, NpcPoint[i].position);
                var distancePercent = Mathf.Clamp01(distance / range);
                distance = distancePercent * range;
                Vector2 offset = new Vector2((distance * dir).x, (distance * dir).y);
                npcTransform.anchoredPosition = player.GetComponent<RectTransform>().anchoredPosition + offset;

                //點大小
                var scale = 1f + ((1 - distancePercent) * 2);
                npcTransform.localScale = new Vector3(pointScale.x * scale, pointScale.y * scale, pointScale.z);
            }
        }
    }

    void RotateObj()
    {
        rotateObj.Rotate(0, 0, rotateSpeed);
    }
}
