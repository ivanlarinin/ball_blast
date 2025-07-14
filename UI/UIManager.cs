using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections;

/*
    UIManager Explanation:

    The UIManager is responsible for managing all user interface elements and interactions in the game. Its main responsibilities include:

    1. Panel Management:
        - Controls the visibility of main UI panels such as the StartGamePanel and ResultPanel.
        - Shows or hides panels based on the current game state (e.g., starting the game, showing results after a level).

    2. Button Handling:
        - Manages button references for starting, restarting, progressing to the next level, and resetting the game.
        - Registers event listeners for button clicks and defines the corresponding actions (e.g., starting the game, restarting the level, advancing to the next level, or resetting progress).

    3. Level Information Display:
        - Updates and displays the current and next level numbers.
        - Shows progress information (such as percentage completion) to the player.

    4. Result Display:
        - Updates the result text to indicate whether the player has passed or failed a level.

    5. Integration with LevelState:
        - Listens to events from the LevelState (such as Passed and Defeat) to update the UI accordingly.
        - Sends messages to update progress text when the level state changes.

    6. Scene and Game Flow Control:
        - Handles scene reloading and level progression logic in response to UI events.

    In summary, the UIManager acts as the central hub for all UI-related logic, ensuring that the game's interface accurately reflects the current state and provides intuitive controls for the player.
*/


public class UIManager : MonoBehaviour
{
    [Header("Main Panels")]
    [SerializeField] private GameObject StartGamePanel;
    [SerializeField] private GameObject ResultPanel;

    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button resetButton;

    [Header("Level Info")]
    [SerializeField] private Text currentLevelText;
    [SerializeField] private Text nextLevelText;
    [SerializeField] private Text progressText;

    void Start()
    {
        // Only show start button and result panel on level 1
        Debug.Log("Current Level: " + LevelState.CurrentLevel);
        if (LevelState.CurrentLevel == 1)
        {
            StartGamePanel.SetActive(true);
            ResultPanel.SetActive(false);
        }
        else
        {
            StartGamePanel.SetActive(false);
            ResultPanel.SetActive(true);
        }

        startButton.onClick.AddListener(OnStartButtonClicked);
        restartButton.onClick.AddListener(OnRestartButtonClicked);
        nextLevelButton.onClick.AddListener(OnNextLevelButtonClicked);
        resetButton.onClick.AddListener(OnResetButtonClicked);

        LevelState levelState = FindFirstObjectByType<LevelState>();
        if (levelState != null)
        {
            levelState.Passed.AddListener(OnLevelPassed);
            levelState.Defeat.AddListener(OnLevelDefeated);
        }
        
        UpdateLevelText();
    }
    
    private void OnStartButtonClicked()
    {   
        StartGamePanel.SetActive(false);

        LevelState levelState = FindFirstObjectByType<LevelState>();
        if (levelState != null)
        {
            levelState.RestartCurrentLevel();
        }
    }

    private void OnRestartButtonClicked()
    {
        Debug.Log("Restart button clicked!");
        
        LevelState levelState = FindFirstObjectByType<LevelState>();
        if (levelState != null)
        {
            levelState.RestartCurrentLevel();
        }

        ResultPanel.SetActive(false);
    }

    private void OnNextLevelButtonClicked()
    {
        Debug.Log("Next level button clicked!");
        
        CoinManager coinManager = FindFirstObjectByType<CoinManager>();
        if (coinManager != null)
        {
            coinManager.SaveCoins();
        }
        
        LevelState.CurrentLevel++;
        
        LevelState levelState = FindFirstObjectByType<LevelState>();
        if (levelState != null)
        {
            levelState.RestartCurrentLevel();
        }
        
        ResultPanel.SetActive(false);
        UpdateLevelText();
    }

    private void OnResetButtonClicked()
    {
        Debug.Log("Reset button clicked!");

        LevelState.ResetLevel();

        ResultPanel.SetActive(false);
        StartGamePanel.SetActive(true);
        UpdateLevelText();
    }
    
    private void OnLevelPassed()
    {
        Debug.Log("Level passed! Showing result screen.");
        ResultPanel.SetActive(true);

        nextLevelButton.gameObject.SetActive(true);
    }

    private void OnLevelDefeated()
    {
        Debug.Log("Level defeated! Showing result screen.");
        ResultPanel.SetActive(true);

        nextLevelButton.gameObject.SetActive(false);
    }
    
    private void UpdateLevelText()
    {
        if (currentLevelText != null)
        {
            currentLevelText.text = "Level: " + LevelState.CurrentLevel;
        }
        
        if (nextLevelText != null)
        {
            nextLevelText.text = "Next: " + (LevelState.CurrentLevel + 1);
        }
    }
    
    public void UpdateProgressText(float progressPercent)
    {
        Debug.Log($"UpdateProgressText called with: {progressPercent:F1}%");
        if (progressText != null)
        {
            progressText.text = "Progress: " + progressPercent.ToString("F1") + "%";
        }
    }
}