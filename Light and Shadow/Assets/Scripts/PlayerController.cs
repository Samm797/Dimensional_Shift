using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Movement
    private Rigidbody2D _rb;
    private Collider2D _collider;
    private float _horizontalInput, _verticalInput;
    [SerializeField] private float _moveSpeed, _dashSpeed;
    private float _activeMoveSpeed;
    private float _dashLength = 0.5f, _dashCooldown = 1.0f;
    private float _dashCounter, _dashCooldownCounter;
    private Vector3 _min = new Vector3(-23f, -21.75f, 0), _max = new Vector3(23f, 23f, 0);

    // Combat
    [SerializeField] private GameObject _spellPrefab;
    [SerializeField] private GameObject _spellContainer;
    private Vector3 _spellOffset = new Vector3(-0.2f, 1.5f, 0f);
    private float _spellSpeed = 30f;
    private float _canCast = -0.5f;
    private float _castCooldown = 1f;
    private HealthSystem _healthSystem;

    // Spell and Player direction
    private Camera _camera;
    public GameObject rotationPoint;
    public SpriteRenderer leftArm;
    public SpriteRenderer leftHand;
    public SpriteRenderer rightArm;
    public SpriteRenderer leftLeg;
    public SpriteRenderer rightLeg;
    private bool _facingRight;

    // Animation
    private Animator _animator;

    // Dimension shifting
    private ColorManager _colorManager;
    private float _canShift = -0.5f;
    private float _shiftCooldown = 2f;

    // Communicating with Managers
    private GameManager _gameManager;



    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (_rb == null)
        {
            Debug.LogError("The player Rigidbody2D is NULL.");
        }

        _collider = GetComponent<Collider2D>();
        if (_collider == null)
        {
            Debug.LogError("The player Collider2D is NULL.");
        }

        _healthSystem = GetComponent<HealthSystem>();
        if (_healthSystem == null)
        {
            Debug.LogError("The player HealthSystem is NULL.");
        }

        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("The player Animator is NULL.");
        }

        _colorManager = GameObject.Find("Color_Manager").GetComponent<ColorManager>();
        if (_colorManager == null)
        {
            Debug.LogError("The ColorManager for the player is NULL.");
        }

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_colorManager == null)
        {
            Debug.LogError("The GameManager for the player is NULL.");
        }
        
        _activeMoveSpeed = _moveSpeed;
        _camera = Camera.main;

        rotationPoint.transform.rotation = Quaternion.Euler(0, 180f, 0f);

        _facingRight = true;
}

    private void FixedUpdate()
    {
        MovePlayer();

        ClampXandY();
    }

    void Update()
    {
        Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        // Flip the player based on the mousePos
        if (mousePos.x > transform.position.x && !_facingRight)
        {
            FlipPlayer();
        }
        else if (mousePos.x < transform.position.x &&  _facingRight)
        {
            FlipPlayer();
        }

        if (_healthSystem.CurrentHealth <= 0)
        {
            _gameManager.isPlayerDead = true;
            _gameManager.GameOverSequence();
        }

        


        Dash();

        // Mouse 0, making this "Fire1" so I can utilize different inputs easier later if wanted
        if (Input.GetButtonDown("Fire1") && Time.time > _canCast)
        {
            // Find the mouse position and set the z coordinate to 0
            
            
            // Sets the direction vector to the mouse position for aiming purposes
            Vector3 difference = mousePos - transform.position;
            float distance = difference.magnitude;
            Vector2 direction = difference / distance;
            direction.Normalize();

            // Fires a spell
            CreateSpell(direction, _spellSpeed);
        }

        // Spacebar
        if (Input.GetButtonDown("Shift") && Time.time > _canShift)
        {
            DimensionShift();
        }

        
    }

    private void MovePlayer()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(_horizontalInput, _verticalInput) * _activeMoveSpeed;
        _rb.velocity = movement;
    }

    private void CreateSpell(Vector3 target, float speed)
    {
        StartCoroutine(CreateSpellRoutine(target, speed));
    }

    private IEnumerator CreateSpellRoutine(Vector3 target, float speed)
    {
        _animator.SetTrigger("attack");
        _canCast = Time.time + _castCooldown;
        yield return new WaitForSeconds(0.18f);
        // Instantiates a spell with a direction and speel, sets the velocity, and puts it in the Spawn_Manager in the hierarchy
        GameObject spell = Instantiate(_spellPrefab, transform.position + _spellOffset, Quaternion.identity);
        spell.GetComponent<Rigidbody2D>().velocity = target * speed;
        spell.transform.parent = _spellContainer.transform;
    }

    private void DimensionShift()
    {
        // Updates the cooldown variable to make sure the player can only shift once the cooldown is up
        _canShift = Time.time + _shiftCooldown;
        // Calls the colorManager function to shift all shift-able objects
        _colorManager.ChangeDimension();

        // TODO: Add audio and camera effects to DimensionShift
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
    /// <summary>
    /// Damages any enemy you hit while this is active. The damage is handled on the EnemyAI script
    /// </summary>
    private void Dash()
    {
        // L Shift 
        if (Input.GetButtonDown("Dash"))
        {
            // If the cooldown is up and we are not currently dashing
            if (_dashCooldownCounter <= 0 && _dashCounter <= 0)
            {
                // Make our collider a trigger and spin
                _collider.isTrigger = true;
                _animator.SetTrigger("attack02");

                // Dash, then set the counter to the length of the dash
                _activeMoveSpeed = _dashSpeed;
                _dashCounter = _dashLength;
            }
        }

        // As long as there is time left on the _dashCounter
        if (_dashCounter > 0)
        {
            // Decrease it each frame
            _dashCounter -= Time.deltaTime;

            // Once we have reached the _dashLength from earlier
            if (_dashCounter <= 0)
            {
                // Set us back to normal
                _collider.isTrigger = false;

                // Active speed is now our normal move speed, and our cooldowncounter begins
                _activeMoveSpeed = _moveSpeed;
                _dashCooldownCounter = _dashCooldown;
            }
        }

        // If our cooldownCounter is active 
        if (_dashCooldownCounter > 0)
        {
            // Decrease every frame
            _dashCooldownCounter -= Time.deltaTime;
        }
    }

    private void ClampXandY()
    {
        // At the end of the next step, predict where the player will be by using velocity
        Vector3 positionAtEndOfStep = _rb.position + _rb.velocity * Time.deltaTime;

        // Clamp the position by the borders on screen
        positionAtEndOfStep.x = Mathf.Clamp(positionAtEndOfStep.x, _min.x, _max.x);
        positionAtEndOfStep.y = Mathf.Clamp(positionAtEndOfStep.y, _min.y, _max.y);

        // Calculate a velocity to get the clamped position
        Vector3 neededVelocity = (positionAtEndOfStep - (Vector3)_rb.position) / Time.deltaTime;

        // Set the velocity to this amount instead of whatever would have been done
        _rb.velocity = neededVelocity;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("MonsterSpell") && _dashCounter <= 0)
        {
            _healthSystem.Damage(other.GetComponent<EnemySpellController>().damage);
            _animator.SetTrigger("damage");
        }
    }


    private void FlipPlayer()
    {
        rotationPoint.transform.Rotate(0f, 180f, 0f);
        _facingRight = !_facingRight;
    }
}
