using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipEnemy : MonoBehaviour
{
    public GameObject rotationPoint;
    public EnemyAI enemy;
    private bool _facingRight;

    private void Awake()
    {
        _facingRight = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (enemy.MoveDirection.x >= 0.01f && _facingRight)
        {
            FlipMonster();
        }
        else if (enemy.MoveDirection.x <= -0.01f && !_facingRight)
        {
            FlipMonster();
        }
    }

    private void FlipMonster()
    {
        rotationPoint.transform.Rotate(0f, 180f, 0f);
        _facingRight = !_facingRight;
    }

}
