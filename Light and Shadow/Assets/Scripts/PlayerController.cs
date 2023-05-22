using UnityEditorInternal;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Movement
    private Rigidbody2D _rb;
    private float _horizontalInput, _verticalInput;
    [SerializeField] private float _moveSpeed, _dashSpeed;
    private float _activeMoveSpeed;
    private float _dashLength = 0.5f, _dashCooldown = 1.0f;
    private float _dashCounter, _dashCooldownCounter;

    // Combat
    [SerializeField] private GameObject _spellPrefab;
    [SerializeField] private GameObject _spellContainer;
    private float _spellSpeed = 30f;
    private HealthSystem _healthSystem;

    // Spell direction
    private Camera _camera;

    // Dimension shifting
    private ColorManager _colorManager;
    private float _canShift = -0.5f;
    private float _shiftCooldown = 2f;



    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (_rb == null)
        {
            Debug.LogError("The player Rigidbody2D is NULL.");
        }

        _activeMoveSpeed = _moveSpeed;
        _camera = Camera.main;

        _colorManager = GameObject.Find("Color_Manager").GetComponent<ColorManager>();
        if (_colorManager == null)
        {
            Debug.LogError("The ColorManager for the player is NULL.");
        }

        _healthSystem = GetComponent<HealthSystem>();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void Update()
    {

        // Mouse 0, making this so I can utilize different inputs easier later if wanted
        if (Input.GetButtonDown("Fire1"))
        {
            // Find the mouse position and set the z coordinate to 0
            Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            
            // Sets the direction vector to the mouse position for aiming purposes
            Vector3 difference = mousePos - transform.position;
            float distance = difference.magnitude;
            Vector2 direction = difference / distance;
            direction.Normalize();

            // Fires a spell
            CreateSpell(direction, _spellSpeed);
        }

        if (Input.GetButtonDown("Shift") && Time.time > _canShift)
        {
            DimensionShift();
            Debug.Log(_colorManager.IsDarkActive());
        }
    }

    private void MovePlayer()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(_horizontalInput, _verticalInput) * _activeMoveSpeed;
        _rb.velocity = movement;

        // Spacebar 
        if (Input.GetButtonDown("Dash"))
        {
            // If the cooldown is up and we are not currently dashing
            if (_dashCooldownCounter <= 0 && _dashCounter <= 0)
            {
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

    private void CreateSpell(Vector3 target, float speed)
    {
        // Instantiates a spell with a direction and speel, sets the velocity, and puts it in the Spawn_Manager in the hierarchy
        GameObject spell = Instantiate(_spellPrefab, transform.position, Quaternion.identity);
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
}
