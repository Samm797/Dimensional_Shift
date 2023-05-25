using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool _isPlayerDead = false, _didPlayerWin = false, _hasGameStarted = false;

    // Communication with other Managers
    private UIManager _uiManager;
    private WaveManager _waveManager;

    public bool isPlayerDead { set { _isPlayerDead = value; } }
    public bool didPlayerWin { set { _didPlayerWin = value; } }

    private void Start()
    {
        
    }

    private void Awake()
    {
        _uiManager = GameObject.Find("UI_Manager").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager on the GameManager is NULL");
        }
        
        _waveManager = GameObject.Find("Wave_Manager").GetComponent<WaveManager>();
        if (_waveManager == null)
        {
            Debug.LogError("The Wave Manager on the GameManager is NULL");
        }
    }

    private void Update()
    {
        // only works if the game hasn't started yet
        if (!_hasGameStarted)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _hasGameStarted = true;
                _waveManager.NextWavePublic();
                _uiManager.StartPlayingSequence(_waveManager.WaveNumber);
            }
        }

        if (_isPlayerDead)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {

            }
        }

    }

    private void StartGame()
    {

    }



    /// <summary>
    /// Called by either the player when killed or by the waveManager when all waves have been cleared 
    /// </summary>
    public void GameOverSequence()
    {
        if (_didPlayerWin == true)
        {
            // This portion returns because if the player would die the same frame as winning, the player counts as winning
            Debug.Log("You have saved us all!");
            return;
        }

        if (_isPlayerDead == true)
        {
            Debug.Log("Ded");
        }
    }
}
