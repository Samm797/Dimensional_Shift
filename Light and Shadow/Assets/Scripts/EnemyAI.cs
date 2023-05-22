using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
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
    private Vector3 _playerPosition;
    private HealthSystem _healthSystem;
    [SerializeField] private int _baseDamage;
    [SerializeField] private int _darkDamage;
    private int _currentDamage;
    private ColorManager _colorManager;

    


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
        

        if (_colorManager.IsDarkActive() == false)
        {
            _currentDamage = _baseDamage;
        } 
        else
        {
            _currentDamage = _darkDamage;
        }

        _startingPosition = transform.position;
        _isRoutineActive = false;
    }

    private void Awake()
    {
        _enemyPathFinding = GetComponent<EnemyPathfinding>();
        _state = State.Roaming;
    }

    private void FixedUpdate()
    {
        switch(_state)
        {
            default:
            case State.Roaming:
                Debug.Log("State.Roaming");
                if (_isRoutineActive == false)
                {
                    StartCoroutine(RoamingRoutine());
                }
                FindTarget();
                break;
            case State.Chasing:
                Debug.Log("State.Chasing");
                if ( _isRoutineActive == false)
                {
                    StartCoroutine(ChasingRoutine());
                }
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
            if (_colorManager.IsDarkActive() == false)
            {
                _healthSystem.Damage(1);
            }
            else
            {
                _healthSystem.Damage(2);
            }
        }

        // Damage the player 
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Damaged the player by {_currentDamage} HP!");
            other.GetComponent<HealthSystem>().Damage(_currentDamage);
        }
    }

    IEnumerator RoamingRoutine()
    {
        _isRoutineActive = true;
        while (_state == State.Roaming)
        {
            Vector3 roamPosition = GetRoamingPosition();
            _enemyPathFinding.MoveTo(roamPosition);
            yield return new WaitForSeconds(2f);
        }
        _isRoutineActive = false;
    }

    IEnumerator ChasingRoutine()
    {
        _isRoutineActive = true;
        while (_state == State.Chasing)
        {
            Vector3 playerPosition = GetPlayerPosition();
            _enemyPathFinding.MoveTo(playerPosition);
            yield return null;
        }
        _isRoutineActive = false;
    }

    private Vector3 GetRoamingPosition()
    {
        return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    private void FindTarget()
    {
        float targetRange = 20f;
        if (Vector2.Distance(transform.position, _player.GetPosition()) < targetRange)
        {
            _state = State.Chasing;
        }
    }

    private Vector3 GetPlayerPosition()
    {
        return _player.GetPosition();
    }
}
