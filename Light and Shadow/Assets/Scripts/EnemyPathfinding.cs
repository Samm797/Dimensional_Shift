using UnityEngine;

public class EnemyPathfinding : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;

    private Rigidbody2D _rb;
    private Vector3 _moveDirection;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        _rb.MovePosition((Vector3)_rb.position + (Time.fixedDeltaTime * _moveSpeed * _moveDirection));
    }

    public void MoveTo(Vector3 targetPosition)
    {
        _moveDirection = targetPosition;
        Debug.Log(targetPosition);
    }

}
