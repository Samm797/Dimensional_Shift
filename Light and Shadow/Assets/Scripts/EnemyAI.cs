using System.Collections;
using UnityEngine;
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
    private EnemyPathfinding _enemyPathFinding;
    private bool _isRoutineActive;
    private PlayerController _player;
    [SerializeField] private float _attackDistance;
    private int _rayLengthForRandomMovement = 10, _rayLengthForCollision = 10;
    private Rigidbody2D _rb;
    private int _randomNumberCounter;

    // Health and damage
    private HealthSystem _healthSystem;
    [SerializeField] private int _baseDamage;
    [SerializeField] private int _darkDamage;
    private int _currentDamage;
    private ColorManager _colorManager;
    [SerializeField] private int _monsterID;


    


    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<PlayerController>();
        if (_player == null )
        {
            Debug.LogError("The Monster's target is NULL.");
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
        // Initialize the pathfinding script and set the state to roaming at the start
        _enemyPathFinding = GetComponent<EnemyPathfinding>();
        _state = State.Roaming;
    }

    private void FixedUpdate()
    {
        // Checks the current state as a switch
        switch (_state)
        {
            default:
            case State.Roaming:
                //Debug.Log("Roaming");

                // If this is not checked, the Coroutine is called on every frame and the enemy will not mov
                if (_isRoutineActive == false)
                {
                    StartCoroutine(RoamingRoutine());
                }
                // Ensures that the enemy is looking for the player while randomly roaming
                FindTarget();
                break;
            case State.Chasing:
                //Debug.Log("Chasing");

                // Get the players position on each physics update and move towards the player 
                // The function implements the pathfinding to avoid obstacles
                Vector3 movePosition = GetPlayerPosition();
                _enemyPathFinding.MoveToLocation(movePosition);
                // Using this as a placeholder for the moment, just stops the enemies
                StopNearPlayer();
                break;
            case State.Attacking:
                //Debug.Log("Attacking");
                break;
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

        if (_healthSystem.CurrentHealth <= 0)
        {
            Destroy(this.gameObject);
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
            _enemyPathFinding.MoveToLocation(roamPosition);
            yield return new WaitForSeconds(2f);
        }
        _isRoutineActive = false;
    }

   
    private Vector3 GetPlayerPosition()
    {
        Vector3 direction = (_player.GetPosition() - transform.position).normalized;
        //Debug.DrawRay(transform.position, direction * _rayLengthForCollision, Color.red, 0f);
        // If there is nothing in front the monster
        if (!DidRaycastHit(_rayLengthForCollision, direction))
        {
            // Move toward the player
            return direction;
        }

        // Shift the direction vector by 45 degrees
        Vector3 left45 = ShiftVector45Degrees(direction, true);
        Vector3 right45 = ShiftVector45Degrees(direction, false);
        
        // Fire out 2 rays, left and right and compare distances between the objects
        RaycastHit2D hitInfoLeft = Physics2D.Raycast(transform.position, left45, _rayLengthForCollision, LayerMask.GetMask("Obstacle"));
        RaycastHit2D hitInfoRight = Physics2D.Raycast(transform.position, right45, _rayLengthForCollision, LayerMask.GetMask("Obstacle"));
        //Debug.DrawRay(transform.position, left45 * _rayLengthForCollision, Color.green, 0f);
        //Debug.DrawRay(transform.position, right45 * _rayLengthForCollision, Color.blue, 0f);

        // Compare the distance between hits
        float leftDistance = hitInfoLeft.distance;
        float rightDistance = hitInfoRight.distance;
        float furthestFromObstacle = Mathf.Max(leftDistance, rightDistance);

        // If the left is furthest 
        if (furthestFromObstacle == leftDistance)
        {
            // Go right
            direction = right45;
            return direction;
        }
        else
        {
            direction = left45;
            return direction;
        }
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

    private void FindTarget()
    {
        // If the player is within range, change the state to Chasing
        float targetRange = 10f;
        if (Vector2.Distance(transform.position, _player.GetPosition()) < targetRange)
        {
            _state = State.Chasing;
        }
    }

    private void StopNearPlayer()
    {
        // If the player is within attack distance, change the state to Attacking
        if (Vector2.Distance(transform.position, _player.GetPosition()) <= _attackDistance)
        { 
            //_enemyPathFinding.StopMoving();
            //_state = State.Attacking;
        }
    }

    /// <summary>
    /// Method for the Brute's melee attack
    /// </summary>
    private void MeleeAttack()
    {

    }
    /// <summary>
    /// Method for the Wraith's ranged attack
    /// </summary>
    private void RangedAttack()
    {

    }

    /// <summary>
    /// Shifts a Vector3 by a 45 degree angle. Use true for left, false for right. 
    /// </summary>
    /// <param name="direction">Initial direction you are shifting</param>
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
}

