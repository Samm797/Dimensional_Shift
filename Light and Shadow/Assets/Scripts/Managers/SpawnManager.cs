using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // Spawning
    [SerializeField] private GameObject[] _monsterPrefabs;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private GameObject _monsterContainer;
    private int _numberToSpawn;

    // Communication with other Managers
    [SerializeField] private WaveManager _waveManager;

    // Start is called before the first frame update
    void Start()
    {
        if (_waveManager == null)
        {
            Debug.LogError("SpawnManager's Game Manager is NULL.");
        }
        
        //Instantiate(_monsterPrefabs[1], _spawnPoints[0]);
        //GameObject dave = Instantiate(_monsterPrefabs[0], _spawnPoints[0]);
        //dave.transform.parent = _monsterContainer.transform;
    }

    public void StartSpawning(int wave)
    {
        // Spawns different numbers based on the waveNumber
        switch (wave)
        {
            case 0:
                break;
            case 1:
                _numberToSpawn = 3;
                Debug.Log($"The spawn number should be 3. The current number is {_numberToSpawn}.");
                break;
            case 2:
                _numberToSpawn = 4;
                Debug.Log($"The spawn number should be 4. The current number is {_numberToSpawn}.");
                break;
            case 3: 
                _numberToSpawn = 5;
                Debug.Log($"The spawn number should be 5. The current number is {_numberToSpawn}.");
                break;
            default:
                Debug.LogError("Default case reached.");
                break;
        }

        StartCoroutine(SpawnMonsterRoutine());
    }

    private IEnumerator SpawnMonsterRoutine()
    {
        // As long as there is something to spawn
        while (_numberToSpawn > 0)
        {
            // Get a random monster type and spawn it at a random location
            int randomMonster = Random.Range(0, 2);
            int randomLocation = Random.Range(0, 4);

            // Spawning and setting in the hierarchy 
            GameObject newMonster = Instantiate(_monsterPrefabs[randomMonster], _spawnPoints[randomLocation].position, Quaternion.identity);
            newMonster.transform.parent = _monsterContainer.transform;

            // Tell the gameManager to log the monster
            _waveManager.EnemySpawned(1);

            // Decrement numberToSpawn and then wait 0.5 seconds.
            _numberToSpawn--;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
