using UnityEngine;

public class cameraController_wave : MonoBehaviour
{
    public float moveSpeed = 10f;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);
    }
}
