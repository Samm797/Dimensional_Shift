using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _monsterPrefabs;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private GameObject _monsterContainer;
    private int numberToSpawn, _waveNumber;
    private GameManager _gameManager;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        _waveNumber = _gameManager.GetWaveNumber();
        
        //StartSpawning();
        //Instantiate(_monsterPrefabs[1], _spawnPoints[0]);
        Instantiate(_monsterPrefabs[0], _spawnPoints[0]);
    }

    public void StartSpawning()
    {
        // Spawns different numbers based on the waveNumber
        switch (_waveNumber)
        {
            case 0:
                break;
            case 1:
                numberToSpawn = 10;
                break;
            case 2:
                numberToSpawn = 15;
                break;
            case 3: 
                numberToSpawn = 20;
                break;
            default:
                Debug.LogError("Default case reached.");
                break;
        }

        StartCoroutine(SpawnMonsterRoutine());
    }

    IEnumerator SpawnMonsterRoutine()
    {
        // As long as there is something to spawn
        while (numberToSpawn > 0)
        {
            // Get a random monster type and spawn it at a random location
            int randomMonster = Random.Range(0, 2);
            int randomLocation = Random.Range(0, 4);

            // Spawning and setting in the hierarchy 
            GameObject newMonster = Instantiate(_monsterPrefabs[randomMonster], _spawnPoints[randomLocation].position, Quaternion.identity);
            newMonster.transform.parent = _monsterContainer.transform;
            
            // Decrement numberToSpawn and then wait 0.5 seconds.
            numberToSpawn--;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
