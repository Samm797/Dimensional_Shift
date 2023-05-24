using System.Collections;
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

    public Vector3 MoveDirection { get { return _moveDirection; } }


    // Combat
    private HealthSystem _healthSystem;
    [SerializeField] private int _baseDamage;
    [SerializeField] private int _darkDamage;
    private ColorManager _colorManager;
    /// <summary>
    /// 0 = mage; 1 = bruiser;
    /// </summary>
    [SerializeField] private int _monsterID;

    // Attacking
    private int _currentDamage;
    [SerializeField] private GameObject _spellPrefab;
    private GameObject _spellContainer;
    private Vector3 _spellOffset;
    public Transform attackPoint;
    public LayerMask playerLayer;
    [SerializeField] private float _attackDistance;
    private HealthSystem _playerHealth;
    private float _rangedCooldown = 0.5f, _meleeCooldown = 0.5f;
    private float _canCast = -0.5f, _canMelee = -0.5f;

    // Communicating with Managers
    private WaveManager _waveManager;

    // Start is called before the first frame update
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
    }

    private void Awake()
    {
        // Initialize the Rigidbody2D and set the state to roaming at the start
        _rb = GetComponent<Rigidbody2D>();
        _state = State.Roaming;

        
        _player = GameObject.Find("Player").GetComponent<PlayerController>();
        if (_player == null)
        {
            Debug.LogError("The Monster's target is NULL.");
        }

        _spellContainer = GameObject.Find("Spell_Container");
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
            // This is manuay set and shoudn't work like this, but to get the enemy to follow the player properly, we have to take one extra setp in the GetAttackPosition() method
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
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Spell"))
        {
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

    IEnumerator RoamingRoutine()
    {
        _isRoutineActive = true;
        while (_state == State.Roaming)
        {
            Vector3 roamPosition = GetRoamingPosition();
            MoveToLocation(roamPosition);
            yield return new WaitForSeconds(2f);
        }
        _isRoutineActive = false;
    }

    private Vector3 GetPlayerPosition()
    {
        // The direction to the player
        Vector3 direction = (_player.GetPosition() - transform.position).normalized;
        Debug.DrawRay(transform.position, direction * _rayLengthForCollision, Color.red, 0f);

        Vector3 left45 = ShiftVector45Degrees(direction, true);
        Vector3 right45 = ShiftVector45Degrees(direction, false);

        bool obstacleInFront = DidRaycastHit(_rayLengthForCollision, direction);
        bool obstacleOnLeft = DidRaycastHit(_avoidanceThreshold, left45);
        bool obstacleOnRight = DidRaycastHit(_avoidanceThreshold, right45);
        Debug.DrawRay(transform.position, left45 * _avoidanceThreshold, Color.green, 0f);
        Debug.DrawRay(transform.position, right45 * _avoidanceThreshold, Color.blue, 0f);

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
        //Debug.DrawRay(transform.position, roamingPosition, Color.white, 10f);
        
        // If there is no obstacle in front of the roamingPosition
        if (!DidRaycastHit(_rayLengthForRandomMovement, roamingPosition))
        {
            return roamingPosition;
        }
        // If there's an obstacle in front of the player, but not one 45 degrees to the left, use the left 45 degrees
        Vector3 leftShiftRoam = ShiftVector45Degrees(roamingPosition, true);
        //Debug.DrawRay(transform.position, leftShiftRoam, Color.green, 10f);
        if (!DidRaycastHit(_rayLengthForRandomMovement, leftShiftRoam))
        {
            return leftShiftRoam;
        }
        // If there's an obstacle to the left, use the right 45 degrees
        Vector3 rightShiftRoam = ShiftVector45Degrees(roamingPosition, false);
        //Debug.DrawRay(transform.position, rightShiftRoam, Color.blue, 10f);
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
            //Debug.DrawRay(transform.position, newRoam, Color.red, 10f);
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
        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, _attackDistance, playerLayer);
        if (hitPlayer)
        {
            _playerHealth.Damage(_currentDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, _attackDistance);
    }

    /// <summary>
    /// Method for a ranged attack
    /// </summary>
    private void RangedAttack()
    {
        Debug.Log("RangedAttack()");
        switch (_monsterID)
        {
            default:
                Debug.LogError("Default case reached in 'RangedAttack()' method.");
                break;
            case 0:
                _spellOffset = new Vector3(-0.5f, 2f, 0f);
                break;
            case 1:
                _spellOffset = new Vector3(0f, 0.8f, 0f);
                break; 
        }

        if (Time.time > _canCast)
        {
            _canCast = Time.time + _rangedCooldown;

            GameObject newSpell = Instantiate(_spellPrefab, transform.position + _spellOffset, Quaternion.identity);
            newSpell.transform.parent = _spellContainer.transform;
            newSpell.GetComponent<MoveSpellTowardsPlayer>().damage = _currentDamage;
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
        RangedAttack();
        Debug.Log("StopMoving()");
    }
}

