using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpellController : MonoBehaviour
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
        _initialDirection = Vector3.zero;
    }
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        // The individual enemies will call this function so we can manipulate the spells in fun ways first if there is time left
        _initialDirection = SetTarget(_player.position);
    }

    private void FixedUpdate()
    {
        if (_initialDirection != Vector3.zero)
        {
            _rb.AddForce(_initialDirection * speed);
        }
    }

    private void Update()
    {
        if (_player.position == null)
        {
            Destroy(this.gameObject);
            return;
        }
    }

    public Vector3 SetTarget(Vector3 position)
    {
        Vector3 direction = (position - transform.position).normalized;
        return direction;
    }

}
