using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerMainMenu : MonoBehaviour
{
    // Music
    [Header("Music")]
    [SerializeField] private AudioSource _mainMenuIntro;
    [SerializeField] private AudioSource _mainMenuLoop;

    private void Awake()
    {
        StopAllMusic();
    }

    private void Start()
    {
        PlaySound(_mainMenuIntro);
    }

    private void Update()
    {
        if (!_mainMenuIntro.isPlaying && !_mainMenuLoop.isPlaying)
        {
            PlaySound(_mainMenuLoop);
            _mainMenuLoop.loop = true;
        }

        if (_mainMenuLoop.isPlaying)
        {
            if (_mainMenuIntro.isPlaying)
            {
                _mainMenuIntro.Stop();
            }
        }
    }

    private void OnDisable()
    {
        StopAllMusic();
    }

    private void PlaySound(AudioSource sound)
    {
        sound.Play();
    }

    public void StopAllMusic()
    {
        _mainMenuIntro.Stop();
        _mainMenuLoop.Stop();
    }
}
