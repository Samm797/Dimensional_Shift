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

    // Text 
    [SerializeField] private TMP_Text _countdownText;
    [SerializeField] private TMP_Text _waveNumberText;
    [SerializeField] private TMP_Text _brutesText;
    [SerializeField] private TMP_Text _magesText;

    // Keeping track of monsters for the UI
    private int _numberOfBrutes;
    private int _numberOfMages;

    // Communication with Managers
    private WaveManager _waveManager;

    // Start is called before the first frame update
    void Start()
    {
        _waveCountdown.SetActive(false);
        _playerLost.SetActive(false);
        _playerWon.SetActive(false);
        _restartGame.SetActive(false);
        _numberOfBrutes = 0;
        _numberOfMages = 0;
    }

    private void Awake()
    {
        _waveManager = GameObject.Find("Wave_Manager").GetComponent<WaveManager>();
        if (_waveManager == null)
        {
            Debug.LogError("The Wave Manager on the GameManager is NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {

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
        _restartGame.SetActive(true);
    }

    public void PlayerWon()
    {
        _waveCountdown.SetActive(false);
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
}
