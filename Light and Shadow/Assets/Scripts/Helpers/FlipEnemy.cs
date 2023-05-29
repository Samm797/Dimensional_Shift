using UnityEngine;

/// <summary>
/// This class requires the sprite to be facing the right. If they aren't, the code below will not work without adjustment.
/// </summary>
public class FlipEnemy : MonoBehaviour
{
    public GameObject rotationPoint;
    public EnemyAI enemy;
    private bool _facingRight;

    public bool facingRight { get { return _facingRight; } }    


    private void Awake()
    {
        // Set to right at the start 
        _facingRight = true;
    }

    // Update is called once per frame
    void Update()
    {
        // If the enemy is moving right
        if (enemy.MoveDirection.x >= 0.01f && _facingRight)
        {
            FlipMonster();
        }
        // If they are moving left
        else if (enemy.MoveDirection.x <= -0.01f && !_facingRight)
        {
            FlipMonster();
        }
    }

    private void FlipMonster()
    {
        // Transform 180 degrees and flip the boolean
        rotationPoint.transform.Rotate(0f, 180f, 0f);
        _facingRight = !_facingRight;
    }

}
