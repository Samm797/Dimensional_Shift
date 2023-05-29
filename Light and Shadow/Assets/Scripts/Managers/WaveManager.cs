using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    private int _waveNumber;
    private const int _FINALWAVE = 3;
    /// <summary>
    /// This variable should never be below 0. All methods that adjust this value keep it from going above or below 0. Do not set below 0.
    /// </summary>
    private int _activeEnemies;
    private bool _isRoutineActive;
    private bool _endlessModeOn;

    //Communication with other Managers
    private SpawnManager _spawnManager;
    private GameManager _gameManager;
    private UIManager _uiManager;

    // Would like to convert this to a getter/setter where you can decrement and do the job of the two functions below through this.
    // Will work on that later.
    public int ActiveEnemies { get { return _activeEnemies; } }
    
    public int WaveNumber { get { return _waveNumber; } }

    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("The Wave Manager's spawn manager is NULL.");
        }
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("The Wave Manager's game manager is NULL.");
        }
        _uiManager = GameObject.Find("UI_Manager").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("The Wave Manager's game manager is NULL.");
        }

        _waveNumber = 0;
        _activeEnemies = 0;
        _isRoutineActive = false;
        _endlessModeOn = MainMenu.EndlessModeOn;
    }
    

    public void EnemySpawned(int amount)
    {
        // Ensure that _activeEnemies is never negative
        if (_activeEnemies < 0)
        {
            _activeEnemies = 0;
        }

        _activeEnemies += amount;
    }

    public void EnemyDestroyed()
    {
        _activeEnemies--;

        // Ensure that _activeEnemies is never negative
        if (_activeEnemies < 0)
        {
            _activeEnemies = 0;
        }

        if (_activeEnemies == 0) 
        { 
            NextWave();
        }
    }
    /// <summary>
    /// Most likely this is not the correct way to do this, but I need to be able to call the next wave from another class and this is a solution.
    /// </summary>
    public void NextWavePublic()
    {
        NextWave();
    }

    private void NextWave()
    {
        // This should be 0 when called, but this ensures that there is not a new wave spawned too soon.
        _activeEnemies = 0;

        // Increment the wave number
        _waveNumber++;

        if (!_endlessModeOn)
        {
            // If wave number is greater than the final wave, this function will no longer be called as we do the GameOverSequence from the gameManager
            if (_waveNumber > _FINALWAVE)
            {
                _waveNumber = _FINALWAVE;
                // Implementation for a boss if time allows can go here
                _gameManager.DidPlayerWin = true;
                _gameManager.GameOverSequence();
                return;
            }
        }

        // Do something different on the first wave
        if (_waveNumber == 1)
        {
            StartCoroutine(InitialWaveRoutine());
            
            return;
        }
        else
        {
            StartCoroutine(NextWavePreparationRoutine());
        }
    }

    private IEnumerator InitialWaveRoutine()
    {
        _isRoutineActive = true;
        while (_isRoutineActive)
        {
            yield return new WaitForSeconds(3);
            _spawnManager.StartSpawning(_waveNumber);
            _isRoutineActive = false;
        }
    }

    private IEnumerator NextWavePreparationRoutine()
    {
        // UI Manager displays the countdown while resetting the number of monsters
        _uiManager.WavePreparation(_waveNumber);
        
        // Update the top left wave
        _uiManager.DisplayCurrentWave(_waveNumber);

        // Wait for the wave prep and the UI updates to start
        yield return new WaitForSeconds(3);
        
        // Start spawning the monsters
        _spawnManager.StartSpawning(_waveNumber);
    }
}
