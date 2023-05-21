using UnityEngine;

public class TargetController : MonoBehaviour
{
    private Camera _camera;
    private Vector2 _mousePos;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        _mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);

        transform.position = _mousePos;
    }

}





