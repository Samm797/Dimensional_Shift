using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] monsterPrefabs;
    [SerializeField] private Transform[] spawnPoints;
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
        while (numberToSpawn > 0)
        {
            int randomMonster = Random.Range(0, 2);
            int randomLocation = Random.Range(0, 4);
            Debug.Log($"Spawning monster [{randomMonster}] at location [{randomLocation + 1}]");
            GameObject newMonster = Instantiate(monsterPrefabs[randomMonster], spawnPoints[randomLocation].position, Quaternion.identity);
            numberToSpawn--;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
