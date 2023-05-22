using UnityEngine;

public class EnemyPathfinding : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;

    private Rigidbody2D _rb;
    private Vector3 _moveDirection;
    private bool _isMoving;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // If the monster shouldn't be moving, set the velocity and angular velocity to zero and then return
        if (_isMoving == false)
        {
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = 0f;
            return;
        }
        // else move to the MoveDirection
        _rb.MovePosition((Vector3)_rb.position + (Time.fixedDeltaTime * _moveSpeed * _moveDirection));
    }

    public void MoveToRandom(Vector3 targetPosition)
    {
        // Ensure the monster is moving and then move toward the target position
        _isMoving = true;
        _moveDirection = targetPosition.normalized;
    }

    public void MoveToPlayer(Vector3 targetPosition)
    {
        // Ensure the monster is moving and then move toward the direction of the player 
        _isMoving = true;
        _moveDirection = (targetPosition - transform.position).normalized;
    }

    public void StopMoving()
    {
        // Stop movement
        _isMoving = false;
    }

}
