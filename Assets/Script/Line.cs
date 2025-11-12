using UnityEngine;

public class Line : MonoBehaviour
{
    [SerializeField] GameObject target;
    private LineRenderer line;
    private Color white = new Color(1.0f, 1.0f, 1.0f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        line = this.gameObject.AddComponent<LineRenderer>();
        line.startWidth = 0.5f;
        line.endWidth = 0.5f;

        line.positionCount = 2;

        line.startColor = white;
        line.endColor = white;

        line.material = new Material(Shader.Find("Sprites/Default"));
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {
            line.SetPosition(0, target.transform.position);
            line.SetPosition(1, transform.position);
        }
    }
}
