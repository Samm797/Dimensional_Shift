using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Music 
    [Header("Music")]
    [SerializeField] private AudioSource _endlessIntro;
    [SerializeField] private AudioSource _endlessLoop;
    [SerializeField] private AudioSource _waveLoop;

    // Audio clips 
    [Header("Audio Clips")]
    [SerializeField] private AudioSource _enemyBanished;
    [SerializeField] private AudioSource _enemyHit;
    [SerializeField] private AudioSource _playerHit;
    [SerializeField] private AudioSource _playerSpell;
    [SerializeField] private AudioSource _playerWarp;
    [SerializeField] private AudioSource _playerDash;

    // Accessible by other classes
    public AudioSource EnemyBanished { get { return _enemyBanished; } }
    public AudioSource EnemyHit { get { return _enemyHit; } }
    public AudioSource PlayerHit { get { return _playerHit; } }
    public AudioSource PlayerSpell { get { return _playerSpell; } }
    public AudioSource PlayerWarp { get { return _playerWarp; } }
    public AudioSource PlayerDash { get { return _playerDash; } }


    // Endless mode
    private bool _endlessOn;


    // Communication with other managers


    private void Start()
    {
        GameStart();
    }

    private void Awake()
    {
        _endlessOn = MainMenu.EndlessModeOn;
        StopAllMusic();
    }

    private void Update()
    {
        // If endless mode is on
        if( _endlessOn)
        {
            // As long as both the intro and the loop are NOT playing
            // This is to ensure the intro completes and ensures that loop track does not only play for a single fram
            if (!_endlessIntro.isPlaying && !_endlessLoop.isPlaying)
            {
                // Loop the endlessLoop until another method is called
                PlaySound(_endlessLoop);
                _endlessLoop.loop = true;
            }
        }
    }

    public void PlaySound(AudioSource sound)
    {
        sound.Play();
    }

    public void StopAllMusic()
    {
        _endlessIntro.Stop();
        _endlessLoop.Stop();
        _waveLoop.Stop();
    }

    public void GameStart()
    {
        if (_endlessOn)
        {
            PlaySound(_endlessIntro);
        }
        else
        {
            PlaySound(_waveLoop);
        }
    }

    private void OnDisable()
    {
        StopAllMusic();
    }
}
