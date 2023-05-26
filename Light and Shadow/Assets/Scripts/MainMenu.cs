using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private Scene scene;
    private GameManager _gameManager;
    private bool _endlessModeOn = false;
    public Button newGame, highScore, thanks, endlessMode;
    public Button backButton;
    public GameObject highScoreText;
    public GameObject creditText;

    public bool EndlessModeOn { get { return _endlessModeOn; } }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        scene = SceneManager.GetActiveScene();
    }

    private void Update()
    {

        if (scene.buildIndex == 1 && _endlessModeOn)
        {
            _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
            _gameManager.EndlessOn();
            Destroy(gameObject);
        }
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(1);
    }

    public void EndlessMode()
    {
        _endlessModeOn = true;
        SceneManager.LoadScene(1);
    }

    public void HighScore()
    {
        HideButtons();
        highScoreText.SetActive(true);
        creditText.SetActive(false);
    }

    public void Credits()
    {
        HideButtons();
        highScoreText.SetActive(false);
        creditText.SetActive(true);
    }

    private void HideButtons()
    {
        newGame.gameObject.SetActive(false);
        highScore.gameObject.SetActive(false);
        thanks.gameObject.SetActive(false);
        endlessMode.gameObject.SetActive(false);

        backButton.gameObject.SetActive(true);
    }

    public void Back()
    {
        highScoreText.SetActive(false);
        creditText.SetActive(false);
        backButton.gameObject.SetActive(false);
        ShowButtons();
    }

    private void ShowButtons()
    {
        newGame.gameObject.SetActive(true);
        highScore.gameObject.SetActive(true);
        thanks.gameObject.SetActive(true);
        endlessMode.gameObject.SetActive(true);

        backButton.gameObject.SetActive(false);
    }

}
