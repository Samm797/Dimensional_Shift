using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool _isPlayerDead, _didPlayerWin;

    public bool isPlayerDead { set { _isPlayerDead = value; } }
    public bool didPlayerWin { set { _didPlayerWin = value; } }

    private void Start()
    {
        _isPlayerDead = false;
        _didPlayerWin = false;
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
