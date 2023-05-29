using UnityEngine;

public class Timer : MonoBehaviour
{
    private float _currentTime;
    private string _formattedTime;
    private UIManager _uiManager;

    public string formattedTime { get { return _formattedTime; } }


    void Start()
    {
        _uiManager = GameObject.Find("UI_Manager").GetComponent<UIManager>();
        if (_uiManager == null )
        {
            Debug.LogError("The timer's UI Manager is NULL.");
        }
    }

    void Update()
    {
        // Takes the current time, formats it, and tells the UI Manager to update the time on screen.
        _currentTime += Time.deltaTime;
        _formattedTime = DisplayTime(_currentTime);
        _uiManager.UpdateTimer(_formattedTime);
    }

    private string DisplayTime(float timeToFormat)
    {
        float minutes = Mathf.FloorToInt(timeToFormat / 60);
        timeToFormat -= minutes * 60;
        var secondsDisplay = timeToFormat.ToString("00.00").Replace('.', ':');

        // Sets the time as a string 
        _formattedTime = $"{minutes:00}:{secondsDisplay}";
        return _formattedTime;
    }

}
