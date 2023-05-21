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
        // Moves the target reticle to wherever the mouse is on screen
        _mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);

        transform.position = _mousePos;
    }

}





