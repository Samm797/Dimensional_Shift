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

    private SpawnManager _spawnManager;
    private GameManager _gameManager;

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

        _waveNumber = 0;
        _activeEnemies = 0;
        _isRoutineActive = false;

        //NextWave();
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



    private void NextWave()
    {
        // This should be 0 when called, but this ensures that there is not a new wave spawned too soon.
        _activeEnemies = 0;

        // Increment the wave number
        _waveNumber++;

        // If wave number is greater than the final wave, this function will no longer be called as we do the GameOverSequence from the gameManager
        if (_waveNumber > _FINALWAVE)
        {
            _waveNumber = _FINALWAVE;
            // Implementation for a boss if time allows can go here
            _gameManager.didPlayerWin = true;
            _gameManager.GameOverSequence();
            return;
        }

        // Do something different on the first wave
        if (_waveNumber == 1)
        {
            Debug.Log($"Wave #{_waveNumber} Start!");
            _spawnManager.StartSpawning(_waveNumber);
            return;
        }
        else
        {
            StartCoroutine(InBetweenWaves());
        }
        // Wait 3.5 seconds 
    }

    private IEnumerator InBetweenWaves()
    {
        _isRoutineActive = true;
        while (_isRoutineActive == true)
        {
        // Start a countdown
        Debug.Log("3");
        yield return new WaitForSeconds(1);        
        Debug.Log("2");
        yield return new WaitForSeconds(1);        
        Debug.Log("1");
        yield return new WaitForSeconds(1);
        Debug.Log($"Wave #{_waveNumber} Start!");
        _spawnManager.StartSpawning(_waveNumber);
        _isRoutineActive = false;
        }
    }
}
