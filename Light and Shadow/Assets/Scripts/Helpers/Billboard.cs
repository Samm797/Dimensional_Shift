using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform cam;

    private void Awake()
    {
        cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }
}
