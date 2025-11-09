using Mono.Cecil.Cil;
using UnityEngine;

public class ScannerController : MonoBehaviour
{
    public Transform[] CollectionPoint;
    public Transform[] NpcPoint;
    public Transform PlayerPos;

    private float range;
    private GameObject collection;
    private GameObject npc;
    private GameObject player;
    private Vector3 pointScale = new Vector3(0.02f, 0.02f, 1f);

    void Start()
    {
        range = 45f;
        collection = transform.GetChild(2).gameObject;
        npc = transform.GetChild(1).gameObject;
        player = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < CollectionPoint.Length; i++)
        {
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
        
        for(int i = 0; i<NpcPoint.Length; i++)
        {
            RectTransform npcTransform = npc.transform.GetChild(i).GetComponent<RectTransform>();
            //點位置
            var dir = (NpcPoint[i].position - PlayerPos.position).normalized;
            var distance = Vector2.Distance(PlayerPos.position, NpcPoint[i].position);
            var distancePercent = Mathf.Clamp01(distance / range);
            distance = distancePercent * range;
            Vector2 offset = new Vector2((distance * dir).x, (distance * dir).y);
            npcTransform.anchoredPosition = player.GetComponent<RectTransform>().anchoredPosition + offset;

            //點大小
            var scale = 1f + ((1-distancePercent) * 2);
            npcTransform.localScale =  new Vector3(pointScale.x*scale, pointScale.y*scale, pointScale.z);
        }
    }
}
