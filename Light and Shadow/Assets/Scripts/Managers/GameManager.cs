using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool _isPlayerDead = false, _didPlayerWin = false, _hasGameplayStarted = false, _isGameOver = false, _isGamePaused = false;

    // Communication with other Managers
    private UIManager _uiManager;
    private WaveManager _waveManager;

    public bool IsPlayerDead { set { _isPlayerDead = value; } }
    public bool DidPlayerWin { set { _didPlayerWin = value; } }

    private void Start()
    {
        Time.timeScale = 1.0f;
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
        if (!_hasGameplayStarted)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _hasGameplayStarted = true;
                _uiManager.StartGame();
                _waveManager.NextWavePublic();
                _uiManager.StartPlayingSequence(_waveManager.WaveNumber);
            }
        }

        if (_isPlayerDead || _didPlayerWin)
        {
            if (Input.GetKeyDown(KeyCode.R) && _isGameOver == true)
            {
                _isGameOver = false;
                _isPlayerDead = false;
                _isGameOver = false;
                SceneManager.LoadScene(1);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isGamePaused)
            {
                UnPauseGame();
            }
            else
            {
                PauseGame();
            }
        }

    }

    private void GameOver()
    {
        _isGameOver = true;
    }


    /// <summary>
    /// Called by either the player when killed or by the waveManager when all waves have been cleared 
    /// </summary>
    public void GameOverSequence()
    {
        if (_didPlayerWin)
        {
            _uiManager.PlayerWon();
            Time.timeScale = 0f;
            GameOver();
            return;
        }

        if (_isPlayerDead)
        {
            _uiManager.PlayerLost();
            Time.timeScale = 0f;
            GameOver();
        }
    }

    public void EndlessOn()
    {
        // Hope to be implemented
        Debug.Log("EndlessOn()");
    }

    private void PauseGame()
    {
        _uiManager.PauseGame();
        _isGamePaused = true;
        Time.timeScale = 0f;
    }

    private void UnPauseGame()
    {
        _uiManager.UnPauseGame();
        _isGamePaused = false;
        Time.timeScale = 1f;
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
