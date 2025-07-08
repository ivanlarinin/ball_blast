using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject startButtonPanel; 
    [SerializeField] private Button startButton;

    [SerializeField] private GameObject defeatPanel; 
    [SerializeField] private Button restartButtonDefeat; 

    [SerializeField] private GameObject passedPanel; 
    [SerializeField] private Button restartButtonPassed; 
    [SerializeField] private Button nextLevelButton; // Add this line
    [SerializeField] private Text currentLevelText; // Add this line
    [SerializeField] private Text nextLevelText;    // Add this line
    [SerializeField] private Text progressText; // Add this line

    public UnityEvent OnGameStart;

    private LevelState levelState;

    void Awake()
    {
        startButton.onClick.AddListener(StartGame);
        restartButtonDefeat.onClick.AddListener(RestartLevel);
        restartButtonPassed.onClick.AddListener(RestartLevel);
        nextLevelButton.onClick.AddListener(NextLevel); // Add this line

        startButtonPanel.SetActive(true);
        defeatPanel.SetActive(false);
        passedPanel.SetActive(false);
    }

    void Start()
    {
        // Subscribe to LevelState events in Start to ensure LevelState exists
        levelState = FindFirstObjectByType<LevelState>();
        if (levelState != null)
        {
            levelState.Passed.AddListener(ShowPassedScreen);
            levelState.Defeat.AddListener(ShowDefeatScreen);
        }
    }

    void OnDestroy()
    {
        startButton.onClick.RemoveListener(StartGame);
        restartButtonDefeat.onClick.RemoveListener(RestartLevel);
        restartButtonPassed.onClick.RemoveListener(RestartLevel);

        // Unsubscribe from LevelState events
        if (levelState != null)
        {
            levelState.Passed.RemoveListener(ShowPassedScreen);
            levelState.Defeat.RemoveListener(ShowDefeatScreen);
        }
    }

    public void StartGame()
    {
        startButtonPanel.SetActive(false);
        OnGameStart.Invoke();
        Debug.Log("GameStart botton logic invoked");
        UpdateLevelTexts(); // Add this line
    }

    public void ShowDefeatScreen()
    {
        defeatPanel.SetActive(true);
        startButtonPanel.SetActive(false);
        passedPanel.SetActive(false);
        UpdateLevelTexts(); // Add this line
    }

    public void ShowPassedScreen()
    {
        passedPanel.SetActive(true);
        startButtonPanel.SetActive(false);
        defeatPanel.SetActive(false);
        nextLevelButton.gameObject.SetActive(true);
        UpdateLevelTexts(); // Add this line
    }

    public void RestartLevel()
    {
        LevelState.ResetLevel(); // Add this line to reset level on restart
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        LevelState.CurrentLevel++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void UpdateLevelTexts()
    {
        if (currentLevelText != null)
            currentLevelText.text = $"Level: {LevelState.CurrentLevel}";
        if (nextLevelText != null)
            nextLevelText.text = $"Next: {LevelState.CurrentLevel + 1}";
    }

    private void UpdateProgressText(float percent)
    {
        if (progressText != null)
            progressText.text = $"Progress: {percent:0}%";
    }
}