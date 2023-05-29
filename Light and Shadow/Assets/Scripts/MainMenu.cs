using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private static bool _endlessModeOn = false;

    [Header("Buttons")]
    public Button newGame;
    public Button thanks;
    public Button endlessMode;
    public Button story;

    public Button backButton;

    [Header("Text")]
    public GameObject creditText;
    public GameObject instructionText;
    public GameObject spaceToContinue;
    public GameObject storyText;


    private bool _readyToContinue;
    private bool _isRoutineActive;

    public static bool EndlessModeOn { get { return _endlessModeOn; } }

    private void Awake()
    {
        BeginningOfGame();
    }

    private void Update()
    {
        if (_isRoutineActive && Input.GetKeyDown(KeyCode.Space))
        {
            _readyToContinue = true;
        }
    }

    private void BeginningOfGame()
    {
        newGame.gameObject.SetActive(true);
        thanks.gameObject.SetActive(true);
        endlessMode.gameObject.SetActive(true);
        story.gameObject.SetActive(true);

        backButton.gameObject.SetActive(false);
        creditText.SetActive(false);
        instructionText.SetActive(false);
        spaceToContinue.SetActive(false);
        storyText.SetActive(false);

    }

    public void LoadGame()
    {
        _endlessModeOn = false;
        ShowInstructions();
        StartCoroutine(StartGameRoutine());
    }

    public void EndlessMode()
    {
        _endlessModeOn = true;
        ShowInstructions();
        StartCoroutine(StartGameRoutine());
    }

    private IEnumerator StartGameRoutine()
    {
        while (_readyToContinue == false)
        {
            _isRoutineActive = true;
            yield return null;
        }

        _isRoutineActive = false;
        SceneManager.LoadScene(1);

    }

    private void ShowInstructions()
    {
        HideButtons();
        backButton.gameObject.SetActive(false);
        instructionText.SetActive(true);
        spaceToContinue.SetActive(true);
    }

    public void HighScore()
    {
        HideButtons();
        backButton.gameObject.SetActive(true);
        creditText.SetActive(false);
    }

    public void Credits()
    {
        HideButtons();
        creditText.SetActive(true);
    }

    public void Story()
    {
        HideButtons();
        storyText.SetActive(true);
    }

    private void HideButtons()
    {
        newGame.gameObject.SetActive(false);
        thanks.gameObject.SetActive(false);
        endlessMode.gameObject.SetActive(false);
        story.gameObject.SetActive(false);
        backButton.gameObject.SetActive(true);
    }


    public void Back()
    {
        creditText.SetActive(false);
        storyText.SetActive(false);
        backButton.gameObject.SetActive(false);
        ShowButtons();
    }

    private void ShowButtons()
    {
        newGame.gameObject.SetActive(true);
        thanks.gameObject.SetActive(true);
        endlessMode.gameObject.SetActive(true);
        story.gameObject.SetActive(true);
        backButton.gameObject.SetActive(false);
    }

}
