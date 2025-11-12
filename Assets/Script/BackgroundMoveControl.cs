using UnityEngine;

public class BackgroundMoveControl : MonoBehaviour
{
    public Transform camera;
    public Transform player;
    public Transform farBg;
    public Transform frontBg;

    private float lastMove;

    void Start()
    {
        lastMove = camera.position.x;
    }

    void FixedUpdate()
    {
        float amountOfMove = camera.position.x - lastMove;
        farBg.position = farBg.position + new Vector3(amountOfMove*0.5f, 0, 0);
        frontBg.position = frontBg.position + new Vector3(amountOfMove * -0.2f, 0, 0);
        lastMove = camera.position.x;
    }
}
