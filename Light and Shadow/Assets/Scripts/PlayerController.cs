using System.Collections.Specialized;
using UnityEngine;

public class PlayerController: MonoBehaviour
{

    private Rigidbody2D _rb;
    private float _horizontalInput, _verticalInput;
    [SerializeField] private float _moveSpeed, _dashSpeed;
    private float _activeMoveSpeed;
    [SerializeField] private float _dashLength = 0.5f, _dashCooldown = 1.0f;
    private float _dashCounter, _dashCooldownCounter;
    [SerializeField] private GameObject _spellPrefab;
    [SerializeField] private float _spellSpeed = 30f;
    private Camera _camera;
    private Vector2 _target;
   


    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _activeMoveSpeed = _moveSpeed;
        _camera = Camera.main;
    }

    void Update()
    {
        MovePlayer();

        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            Vector3 difference = mousePos - transform.position;
            float distance = difference.magnitude;
            Vector2 direction = difference / distance;
            direction.Normalize();

            CreateSpell(direction, _spellSpeed);
        }
    }

    private void MovePlayer()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2 (_horizontalInput, _verticalInput) * _activeMoveSpeed;
        _rb.velocity = movement;

        if (Input.GetButtonDown("Dash"))
        {
            if (_dashCooldownCounter <= 0 && _dashCounter <=0 )
            {
                _activeMoveSpeed = _dashSpeed;
                _dashCounter = _dashLength;
            }
        }

        if (_dashCounter > 0)
        {
            _dashCounter -= Time.deltaTime;

            if ( _dashCounter <= 0)
            {
                _activeMoveSpeed = _moveSpeed;
                _dashCooldownCounter = _dashCooldown;
            }
        }

        if (_dashCooldownCounter > 0)
        {
            _dashCooldownCounter -= Time.deltaTime;
        }

    }

    private void CreateSpell(Vector3 target, float speed)
    {
        GameObject spell = Instantiate(_spellPrefab, transform.position, Quaternion.identity);
        spell.GetComponent<Rigidbody2D>().velocity = target * speed;
    }

}
