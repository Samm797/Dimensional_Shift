using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class EnemyAI : MonoBehaviour
{
    // State and Pathfinding
    private enum State
    {
        Roaming,
        Chasing,
        Attacking
    }
    private State _state;
    private bool _isRoutineActive;
    private PlayerController _player;
    private int _rayLengthForRandomMovement = 10, _rayLengthForCollision = 5, _avoidanceThreshold = 4;
    private Rigidbody2D _rb;
    private int _randomNumberCounter;
    [SerializeField] private float _moveSpeed;
    private Vector3 _moveDirection;
    private bool _isMoving;
    private Vector3 _targetPosition;
    private float _minDistanceToTargetPosition = 0.5f;
    [SerializeField] private Animator _animator;

    public Vector3 MoveDirection { get { return _moveDirection; } }


    // Combat
    private HealthSystem _healthSystem;
    [SerializeField] private int _baseDamage;
    [SerializeField] private int _darkDamage;
    private ColorManager _colorManager;
    /// <summary>
    /// 0 = mage; 1 = brute;
    /// </summary>
    [SerializeField] private int _monsterID;
    [SerializeField] private List<SpriteRenderer> _spriteRenderers;
    [SerializeField] private Color _startingColor;
    [SerializeField] private Color _hitColor;

    // Attacking
    private int _currentDamage;
    [SerializeField] private GameObject _spellPrefab;
    private GameObject _spellContainer;
    private Vector3 _spellOffset, _spellOffset2, _spellOffset3;
    public Transform attackPoint;
    public LayerMask playerLayer;
    [SerializeField] private float _attackDistance;
    private HealthSystem _playerHealth;
    private float _rangedCooldown = 0.5f;
    // private float _meleeCooldown = 0.5f;
    private float _canCast = -1.5f;
    // private float _canMelee = -0.5f;
    public FlipEnemy _flip;

    // Communicating with Managers
    private WaveManager _waveManager;
    private UIManager _uiManager;
    private AudioManager _audioManager;

    // Audio
    private AudioSource _enemyBanished;
    private AudioSource _enemyHit;


    void Start()
    {
        _playerHealth = _player.GetComponent<HealthSystem>();
        if ( _playerHealth == null )
        {
            Debug.LogError("The Monster's PlayerHealth is NULL.");
        }
        _healthSystem = GetComponent<HealthSystem>();
        if (_healthSystem == null )
        {
            Debug.LogError("The Monster's health system is NULL.");
        }

        _colorManager = GameObject.Find("Color_Manager").GetComponent<ColorManager>();
        if (_colorManager == null )
        {
            Debug.LogError("The Monster's color manager is NULL.");
        }

        _rb = GetComponent<Rigidbody2D>();
        if (_rb == null)
        {
            Debug.LogError("The Monster's Rigidbody2D is NULL.");
        }

        _waveManager = GameObject.Find("Wave_Manager").GetComponent<WaveManager>();
        if (_waveManager == null)
        {
            Debug.LogError("Enemies' Game Manager is NULL.");
        }

        _uiManager = GameObject.Find("UI_Manager").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("Enemies' UI Manager is NULL.");
        }


        // Set current damage to base damage as long as the dark is inactive
        if (_colorManager.IsDarkActive == false)
        {
            _currentDamage = _baseDamage;
        } 
        // Else, set current damage to dark damage
        else
        {
            _currentDamage = _darkDamage;
        }

        // No routine is running at the start
        _isRoutineActive = false;

        // Set sounds
        _enemyBanished = _audioManager.EnemyBanished;
        _enemyHit = _audioManager.EnemyHit;
    }

    private void Awake()
    {
        // Initialize the Rigidbody2D and set the state to roaming at the start
        _rb = GetComponent<Rigidbody2D>();
        _state = State.Roaming;

        if (_animator == null)
        {
            Debug.LogError("The Monster's Animator is NULL.");
        }


        _player = GameObject.Find("Player").GetComponent<PlayerController>();
        if (_player == null)
        {
            Debug.LogError("The Monster's target is NULL.");
        }

        _spellContainer = GameObject.Find("Spell_Container");

        _audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
        if (_audioManager == null)
        {
            Debug.LogError("The Monster's audio manager is NULL.");
        }

        if (_targetPosition == null)
        {
            StopMoving();
            _animator.SetBool("isMoving", false);
        }
    }

    private void FixedUpdate()
    {

        // Checks the current state as a switch
        switch (_state)
        {
            default:
            case State.Roaming:
                // If this is not checked, the Coroutine is called on every frame and the enemy will not move
                if (_isRoutineActive == false)
                {
                    StartCoroutine(RoamingRoutine());
                }
                // If the enemy gets close enough to the player, the State changes to "Chasing"
                // TODO: Set this to automatically find the player after a certain Time.FixedDeltaTime
                FindTarget();
                break;
            case State.Chasing:
                // The function implements the pathfinding to avoid obstacles
                _targetPosition = GetPlayerPosition();
                MoveToLocation(_targetPosition);
                // If the enemy is close enough to the player, the State changes to "Attacking"
                IsPlayerInAttackRange();
                break;
            case State.Attacking:
                RangedAttack(); 
                _targetPosition = GetAttackPosition();
                MoveToLocation(_targetPosition);
                break;
        }
        if (_isMoving)
        {
            _animator.SetBool("isMoving", true);
            // This is manually set and shouldn't work like this, but to get the enemy to follow the player properly, we have to take one extra step in the GetAttackPosition() method
            // Setting the _targetPosition to this value
            _targetPosition = (_player.GetPosition() - ((_player.GetPosition() - transform.position).normalized) * _attackDistance);
            float distanceToTarget = Vector3.Distance(transform.position, _targetPosition);
            if ( distanceToTarget <= _minDistanceToTargetPosition)
            {
                StopMoving();
                return;
            }

            _rb.MovePosition((Vector3)_rb.position + (Time.fixedDeltaTime * _moveSpeed * _moveDirection));

        }
        else
        {
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = 0f;
            _isMoving = false;
            _animator.SetBool("isMoving", false);
            return;
        }


    }
    // Update is called once per frame
    void Update()
    {
        if (_colorManager.IsDarkActive == false)
        {
            _currentDamage = _baseDamage;
        }
        else
        {
            _currentDamage = _darkDamage;
        }

        if (_healthSystem.CurrentHealth == 0)
        {
            // Tells the gameManager the enemy was destroyed and then destroys itself
            _waveManager.EnemyDestroyed();
            _uiManager.EnemyTypeDestroyed(_monsterID);
            _audioManager.PlaySound(_enemyBanished);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Spell"))
        {
            _audioManager.PlaySound(_enemyHit);

            ShowDamaged();

            // If dark is not active, do 1 damage to enemies
            if (_colorManager.IsDarkActive == false)
            {
                _healthSystem.Damage(1);
            }
            // Else, double damage
            else
            {
                _healthSystem.Damage(2);
            }
        }


        // Only called while the player is dashing
        if (other.CompareTag("Player"))
        {
            _audioManager.PlaySound(_enemyHit);

            ShowDamaged();

            // If dark is not active, do 2 damage to enemies
            if (_colorManager.IsDarkActive == false)
            {
                _healthSystem.Damage(2);
            }
            // Else, 3 damage
            else
            {
                _healthSystem.Damage(3);
            }
        }
    }

    private void ShowDamaged()
    {
        StartCoroutine(ShowDamageRoutine());
    }

    private IEnumerator ShowDamageRoutine()
    {
        // If there's nothing in the list, return
        if (_spriteRenderers.Count <= 0)
        {
            yield break;
        }

        // Turn every sprite renderers' color to the appropriate hit color
        for (int i = 0; i < _spriteRenderers.Count; i++)
        {
            _spriteRenderers[i].color = _hitColor;
        }

        // Wait some time 
        yield return new WaitForSeconds(0.3f);
        
        // Turn every sprite renderers' color to the starting color
        for (int i = 0; i < _spriteRenderers.Count; i++)
        {
            _spriteRenderers[i].color = _startingColor;
        }
    }

    IEnumerator RoamingRoutine()
    {
        _isRoutineActive = true;
        int loopsThrough = 0;
        while (_state == State.Roaming)
        {
            loopsThrough++;
            Vector3 roamPosition = GetRoamingPosition();
            MoveToLocation(roamPosition);
            yield return new WaitForSeconds(2f);

            // If the monsters run through the roaming routine more than 3 times, they will change their state to chasing
            if (loopsThrough >= 4)
            {
                _state = State.Chasing;
            }
        }
        _isRoutineActive = false;
    }

    private Vector3 GetPlayerPosition()
    {
        // The direction to the player
        Vector3 direction = (_player.GetPosition() - transform.position).normalized;

        Vector3 left45 = ShiftVector45Degrees(direction, true);
        Vector3 right45 = ShiftVector45Degrees(direction, false);

        bool obstacleInFront = DidRaycastHit(_rayLengthForCollision, direction);
        bool obstacleOnLeft = DidRaycastHit(_avoidanceThreshold, left45);
        bool obstacleOnRight = DidRaycastHit(_avoidanceThreshold, right45);

        // If there's nothing in front of the monster
        if (!obstacleInFront && !obstacleOnLeft && !obstacleOnRight)
        {
            return direction;
        }
        else if (obstacleOnLeft && !obstacleOnRight)
        {
            direction = right45;
            return direction;
        } 
        else if (!obstacleOnLeft && obstacleOnRight)
        {
            direction = left45;
            return direction;
        }
        else if (obstacleOnLeft && obstacleOnRight)
        {
            direction = ShiftVector45Degrees(left45, true); 
            return direction;   
        }
        return direction;
    }

    private Vector3 GetRoamingPosition()
    {
        // Returns a random direction
        Vector3 roamingPosition = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        
        // If there is no obstacle in front of the roamingPosition
        if (!DidRaycastHit(_rayLengthForRandomMovement, roamingPosition))
        {
            return roamingPosition;
        }
        // If there's an obstacle in front of the player, but not one 45 degrees to the left, use the left 45 degrees
        Vector3 leftShiftRoam = ShiftVector45Degrees(roamingPosition, true);
        if (!DidRaycastHit(_rayLengthForRandomMovement, leftShiftRoam))
        {
            return leftShiftRoam;
        }
        // If there's an obstacle to the left, use the right 45 degrees
        Vector3 rightShiftRoam = ShiftVector45Degrees(roamingPosition, false);
        if (!DidRaycastHit(_rayLengthForRandomMovement, rightShiftRoam))
        {
            return rightShiftRoam;
        }
        // Get a new random direction
        else
        {
            // Ensures that there is no stack overflow from repeated recursive calls 
            if (_randomNumberCounter >= 3)
            {
                // Set back to 0 so we can recursively call 
                _randomNumberCounter = 0;
                return roamingPosition;
            }

            // Increment the random counter, then call the function again
            _randomNumberCounter++;
            Vector3 newRoam = GetRoamingPosition().normalized;
            return newRoam;
        }
    }

    private Vector3 GetAttackPosition()
    {
        Vector3 direction = (_player.GetPosition() - transform.position).normalized;
        Vector3 attackPosition = _player.GetPosition() - direction * _attackDistance;

        Vector3 moveDirection = (attackPosition - transform.position).normalized;
        return moveDirection;
    }

    private void FindTarget()
    {
        // If the player is within range, change the state to Chasing
        float targetRange = 10f;

        if (Vector2.Distance(transform.position, _player.GetPosition()) < targetRange)
        {
            _state = State.Chasing;
        }
    }

    private void IsPlayerInAttackRange()
    {
        // If the player is within attack distance, change the state to Attacking
        if (Vector2.Distance(transform.position, _player.GetPosition()) <= _attackDistance)
        {
            _state = State.Attacking;
        }
    }

    /// <summary>
    /// Method for a melee attack
    /// </summary>
    private void MeleeAttack()
    {
        _animator.SetTrigger("attack");
        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, _attackDistance, playerLayer);
        if (hitPlayer)
        {
            _playerHealth.Damage(_currentDamage);
        }
    }


    /// <summary>
    /// Method for a ranged attack
    /// </summary>
    private void RangedAttack()
    {

        switch (_monsterID)
        {
            default:
                Debug.LogError("Default case reached in 'RangedAttack()' method.");
                break;
            case 0:
                if (_flip.facingRight)
                {
                    _spellOffset = new Vector3(0.2f, 2f, 0f);
                }
                else
                {
                    _spellOffset = new Vector3(-0.2f, 2f, 0f);
                }
                break;
            case 1:
                if (_flip.facingRight)
                {
                    _spellOffset = new Vector3(-0.5f, 0.8f, 0f);
                } 
                else
                {
                    _spellOffset = new Vector3(0.5f, 0.8f, 0f);
                }
                break; 
        }

        if (Time.time > _canCast)
        {
            _canCast = Time.time + _rangedCooldown;
            StartCoroutine(RangedAttackRoutine(_monsterID));
        }
    }

    /// <summary>
    /// Used so the monsters can manipulate the spells before firing if possible
    /// </summary>
    /// <param name="monsterID">0 = mage; 1 = bruiser</param>
    /// <returns></returns>
    private IEnumerator RangedAttackRoutine(int monsterID)
    {
        _isRoutineActive = true;

        while (_isRoutineActive)
        {
            switch (monsterID)
            {
                default:
                    Debug.LogError("Default case reached in 'RangedAttackRoutine()' method.");
                    yield break;
                case 0:
                    _animator.SetTrigger("attack");
                    yield return new WaitForSeconds(1.2f);
                    GameObject newSpellMage = Instantiate(_spellPrefab, transform.position + _spellOffset, Quaternion.identity);
                    newSpellMage.transform.parent = _spellContainer.transform;
                    newSpellMage.GetComponent<EnemySpellController>().damage = _currentDamage;
                    _isRoutineActive = false;
                    yield break;
                case 1:
                    _animator.SetTrigger("attack");
                    yield return new WaitForSeconds(0.5f);
                    GameObject newSpellBruiser = Instantiate(_spellPrefab, transform.position + _spellOffset, Quaternion.identity);
                    newSpellBruiser.transform.parent = _spellContainer.transform;
                    newSpellBruiser.GetComponent<EnemySpellController>().damage = _currentDamage;
                    _isRoutineActive = false;
                    yield break;
            }
        }
    }


    /// <summary>
    /// Shifts a Vector3 by a 45 degree angle. Use true for left, false for right. 
    /// </summary>
    /// <param name="direction">Initial direction you are shifting from</param>
    /// <param name="isLeft">true for shifting left, false for shifting right</param>
    /// <returns>A new Vector 3 with a 45 deg shift from the initial Vector3.</returns>
    private Vector3 ShiftVector45Degrees(Vector3 direction, bool isLeft)
    {
        if (isLeft == true)
        {
            Vector3 shiftedDirection = Quaternion.Euler(0f, 0f, 45f) * direction;
            shiftedDirection.Normalize();
            return shiftedDirection;
        }
        else
        {
            Vector3 shiftedDirection = Quaternion.Euler(0f, 0f, -45f) * direction;
            shiftedDirection.Normalize();
            return shiftedDirection;
        }
    }

    private bool DidRaycastHit(int distance, Vector3 direction)
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, direction, distance, LayerMask.GetMask("Obstacle"));
        // If the ray hit something
        if (hitInfo.collider != null)
        {
            return true;
        }
        return false;
    }

    private void MoveToLocation(Vector3 targetPosition)
    {
        // Ensure the monster is moving and then move toward the target position
        _isMoving = true;
        _moveDirection = targetPosition.normalized;
    }

    private void StopMoving()
    {
        // Stop movement
        _isMoving = false;
        _animator.SetBool("isMoving", false);
        RangedAttack();
    }
}

