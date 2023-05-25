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

    // Communication with Managers
    private WaveManager _waveManager;

    // Start is called before the first frame update
    void Start()
    {
        _waveCountdown.SetActive(false);
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

    }

    public void PlayerWon()
    {

    }
}
