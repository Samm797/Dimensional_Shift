using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int _waveNumber;
    private List<EnemyAI> _enemies;

    public int WaveNumber { get {return _waveNumber; } } 

    // Start is called before the first frame update
    void Start()
    {
        // Originally will be set to 0 at the start for the tutorial
        _waveNumber = 1;
    }





}
