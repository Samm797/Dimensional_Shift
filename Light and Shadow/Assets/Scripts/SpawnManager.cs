using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _monsterPrefabs;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private GameObject _monsterContainer;
    private int numberToSpawn;

    // Start is called before the first frame update
    void Start()
    {
        numberToSpawn = 10;

        StartSpawning();
    }

    public void StartSpawning()
    {
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
            Debug.Log($"Spawning monster [{randomMonster}] at location [{randomLocation + 1}]");

            // Spawning and setting in the hierarchy 
            GameObject newMonster = Instantiate(_monsterPrefabs[randomMonster], _spawnPoints[randomLocation].position, Quaternion.identity);
            newMonster.transform.parent = _monsterContainer.transform;
            
            // Decrement numberToSpawn and then wait 0.5 seconds.
            numberToSpawn--;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
