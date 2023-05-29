using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Text Holders
    [SerializeField] private GameObject _waveCountdown;
    [SerializeField] private GameObject _playerLost;
    [SerializeField] private GameObject _playerWon;
    [SerializeField] private GameObject _restartGame;
    [SerializeField] private GameObject _startGame;
    [SerializeField] private GameObject _warpExplanation;
    [SerializeField] private GameObject _pause;

    // Text 
    [SerializeField] private TMP_Text _countdownText;
    [SerializeField] private TMP_Text _waveNumberText;
    [SerializeField] private TMP_Text _brutesText;
    [SerializeField] private TMP_Text _magesText;
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private TMP_Text _playerLostText;
    [SerializeField] private TMP_Text _playerWonText;

    // Keeping track of monsters for the UI
    private int _numberOfBrutes;
    private int _numberOfMages;
    private int _totalMonstersDestroyed;

    // Communication with Managers
    private WaveManager _waveManager;
    private Timer _timer;

    // Start is called before the first frame update
    void Start()
    {
        // UI Elements set to inactive
        _waveCountdown.SetActive(false);
        _playerLost.SetActive(false);
        _restartGame.SetActive(false);
        _playerWon.SetActive(false);
        _warpExplanation.SetActive(false);
        _pause.SetActive(false);
        

        // The only element that needs to be active (that would go inactive at some point) at the start is the start game element 
        _startGame.SetActive(true);

        
        _numberOfBrutes = 0;
        _numberOfMages = 0;
        _magesText.text = $"Mages: {_numberOfMages}";
        _brutesText.text = $"Brutes: {_numberOfBrutes}";
        }

    private void Awake()
    {
        _waveManager = GameObject.Find("Wave_Manager").GetComponent<WaveManager>();
        if (_waveManager == null)
        {
            Debug.LogError("The Wave Manager on the UI Manager is NULL");
        }

        _timer = GameObject.Find("Timer").GetComponent<Timer>();
        if (_timer == null)
        {
            Debug.LogError("The Timer on the UI Manager is NULL.");
        }
    }


    public void StartGame()
    {
        StartCoroutine(StartGameRoutine());
    }

    private IEnumerator StartGameRoutine()
    {
        _startGame.SetActive(false);
        _warpExplanation.SetActive(true);
        yield return new WaitForSeconds(6);
        _warpExplanation.SetActive(false);
    }

    public void DisplayCurrentWave(int waveNumber)
    {
        _waveNumberText.text = $"Wave #{waveNumber}";
    }

    public void StartPlayingSequence(int waveNumber)
    {
        StartCoroutine(Countdown(waveNumber));
    }

    public void WavePreparation(int waveNumber)
    {
        _numberOfBrutes = 0;
        _numberOfMages = 0;
        StartCoroutine(Countdown(waveNumber));
    }

    private IEnumerator Countdown(int waveNumber)
    {
        // 3...2...1...Wave Start! 
        _waveCountdown.SetActive(true);
        _countdownText.text = "3!";
        yield return new WaitForSeconds(1);
        _countdownText.text = "2!";
        yield return new WaitForSeconds(1);
        _countdownText.text = "1!";
        yield return new WaitForSeconds(1);
        _countdownText.text = "Wave Start!";
        yield return new WaitForSeconds(1);
        _waveCountdown.SetActive(false);
    }

    public void PlayerLost()
    {
        _waveCountdown.SetActive(false);
        _playerLost.SetActive(true);
        _playerLostText.text = $"You lasted {_timer.formattedTime} and banished {_totalMonstersDestroyed} enemies!";
        _restartGame.SetActive(true);
    }

    public void PlayerWon()
    {
        _waveCountdown.SetActive(false);
        _playerWonText.text = $"You saved the world in {_timer.formattedTime}!";
        _playerWon.SetActive(true);
    }

    /// <param name="monsterID">0 = mage; 1 = brute</param>
    public void EnemyTypeSpawned(int monsterID)
    {
        switch (monsterID)
        {
            default:
                Debug.LogError("Default case reached in 'EnemyTypeSpawned()' in UI Manager.");
                break;
            case 0:
                _magesText.text = $"Mages: {++_numberOfMages}";
                break;
            case 1:
                _brutesText.text = $"Brutes: {++_numberOfBrutes}";
                break;
        }
    }

    /// <param name="monsterID">0 = mage; 1 = brute</param>
    public void EnemyTypeDestroyed(int monsterID)
    {
        // If total monsters destroyed is below 0, set it to 0
        if (_totalMonstersDestroyed < 0)
        {
            _totalMonstersDestroyed = 0;
        } 
        // Increment it 
        _totalMonstersDestroyed++;



        _totalMonstersDestroyed++;
        switch (monsterID)
        {
            default:
                Debug.LogError("Default case reached in 'EnemyTypeDestroyed()' in UI Manager.");
                break;
            case 0:
                _numberOfMages--;
                if (_numberOfMages < 0)
                {
                    _numberOfMages = 0;
                }
                _magesText.text = $"Mages: {_numberOfMages}";
                break;
            case 1:
                _numberOfBrutes--;
                if (_numberOfBrutes < 0)
                {
                    _numberOfBrutes = 0;
                }
                _brutesText.text = $"Brutes: {_numberOfBrutes}";
                break;
        }
    }

    public void UpdateTimer(string time)
    {
        _timerText.text = time;
    }

    public void PauseGame()
    {
        _pause.SetActive(true);
    }

    public void UnPauseGame()
    {
        _pause.SetActive(false);
    }

}
