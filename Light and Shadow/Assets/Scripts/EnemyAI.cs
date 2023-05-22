using System.Collections;
using UnityEngine;

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
    private Vector3 _startingPosition;
    private bool _isRoutineActive;
    private PlayerController _player;
    [SerializeField] private float _attackDistance;

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
        
        // Set current damage to base damage as long as the dark is inactive
        if (_colorManager.IsDarkActive() == false)
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
        switch(_state)
        {
            default:
            case State.Roaming:
                Debug.Log("Roaming");

                // If this is not checked, the Coroutine is called on every frame and the enemy will not mov
                if (_isRoutineActive == false)
                {
                    StartCoroutine(RoamingRoutine());
                }
                // Ensures that the enemy is looking for the player while randomly roaming
                FindTarget();
                break;
            case State.Chasing:
                Debug.Log("Chasing");

                // Get the players position on each physics update and move towards the player 
                Vector3 playerPosition = _player.GetPosition();
                _enemyPathFinding.MoveToPlayer(playerPosition);

                // Using this as a placeholder for the moment, just stops the enemies
                StopNearPlayer();
                break;
            case State.Attacking:
                Debug.Log("Attacking");
                break;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (_colorManager.IsDarkActive() == false)
        {
            _currentDamage = _baseDamage;
        }
        else
        {
            _currentDamage = _darkDamage;
        }

        if (_healthSystem.CurrentHealth() <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Spell"))
        {
            // If dark is not active, do 1 damage to enemies
            if (_colorManager.IsDarkActive() == false)
            {
                _healthSystem.Damage(1);
            }
            // Else, double damage
            else
            {
                _healthSystem.Damage(2);
            }
        }
    }

    IEnumerator RoamingRoutine()
    {
        _isRoutineActive = true;
        while (_state == State.Roaming)
        {
            Vector3 roamPosition = GetRoamingPosition();
            _enemyPathFinding.MoveToRandom(roamPosition);
            yield return new WaitForSeconds(2f);
        }
        _isRoutineActive = false;
    }


    private Vector3 GetRoamingPosition()
    {
        // Returns a random direction
        // TODO: Stop monsters from choosing directions with either other monsters or walls
        return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
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
            _enemyPathFinding.StopMoving();
            _state = State.Attacking;
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

}
