using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSpellTowardsPlayer : MonoBehaviour
{
    private Transform _player;
    private Rigidbody2D _rb;
    public float speed = 30f;
    private Vector3 _initialDirection;
    private int _damage;


    public int damage
    {
        get { return _damage; }
        set
        {
            _damage = value;
        }
    }

    private void Awake()
    {
        _player = GameObject.Find("Player").GetComponent<Transform>();
    }
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _initialDirection = (_player.position - transform.position).normalized;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _rb.AddForce(_initialDirection * speed);
    }
}
