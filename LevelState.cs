using UnityEngine;
using UnityEngine.Events;

// This file defines the LevelState MonoBehaviour, which manages the current level's state, progress, and transitions.
// It tracks level progress, handles level completion and defeat, and interacts with other systems like the UI, coin management, and stone spawning.
// The static CurrentLevel property persists the player's level using PlayerPrefs, and events are used to notify other systems of level outcomes.


public class LevelState : MonoBehaviour
{
    private StoneSpawner spawner;
    private Cart cart;
    private UIManager uiManager;

    // Events for level completion and failure
    public UnityEvent Passed = new UnityEvent();
    public UnityEvent Defeat = new UnityEvent();

    private bool gameStarted = false;

    // Progress tracking for level completion
    private int totalProgressUnits;
    private int destroyedProgressUnits;

    // Progress properties for UI display
    public float Progress01 => totalProgressUnits == 0 ? 0f : (float)destroyedProgressUnits / totalProgressUnits;
    public float ProgressPercent => Progress01 * 100f;

    private const string LevelKey = "CurrentLevel";
    public static int CurrentLevel
    {
        get => PlayerPrefs.GetInt(LevelKey, 1);
        set { PlayerPrefs.SetInt(LevelKey, value); PlayerPrefs.Save(); }
    }
    
    public static void ResetLevel()
    {
        // Reset to level 1 and clear coins
        PlayerPrefs.SetInt(LevelKey, 1);
        PlayerPrefs.Save();

        CoinManager coinManager = FindFirstObjectByType<CoinManager>();
        if (coinManager != null)
        {
            coinManager.ResetCoins();
        }
        
        // Reset upgrades
        UpgradeManager upgradeManager = FindFirstObjectByType<UpgradeManager>();
        if (upgradeManager != null)
        {
            upgradeManager.ResetUpgrades();
        }
    }
    
    public void RestartCurrentLevel()
    {
        // Destroy all existing stones
        Stone[] allStones = FindObjectsByType<Stone>(FindObjectsSortMode.None);
        foreach (Stone stone in allStones)
        {
            if (stone != null)
            {
                Destroy(stone.gameObject);
            }
        }
        
        spawner.ResetSpawner();
        // Reset progress tracking
        destroyedProgressUnits = 0;
        totalProgressUnits = 0;
        
        // Restore saved coin amount
        CoinManager coinManager = FindFirstObjectByType<CoinManager>();
        if (coinManager != null)
        {
            coinManager.LoadCoins();
        }

        EnableGameLogic();
    }

    private void SubscribeEvents()
    {
        // Subscribe to level events
        Defeat.AddListener(DisableGameLogic);
        Passed.AddListener(DisableGameLogic);
        if (cart != null)
            cart.CollisionStone.AddListener(OnCartCollisionStone);
    }

    private void UnsubscribeEvents()
    {
        // Unsubscribe to prevent memory leaks
        Defeat.RemoveListener(DisableGameLogic);
        Passed.RemoveListener(DisableGameLogic);
        if (cart != null)
            cart.CollisionStone.RemoveListener(OnCartCollisionStone);
    }

    private void Awake()
    {
        // Clear all PlayerPrefs for testing
        PlayerPrefs.DeleteAll();

        // Find and cache references to game objects
        spawner = FindFirstObjectByType<StoneSpawner>();
        if (spawner == null)
        {
            Debug.LogWarning("StoneSpawner not found in the scene.");
        }

        cart = FindFirstObjectByType<Cart>();
        if (cart == null)
        {
            Debug.LogWarning("Cart not found in the scene.");
        }

        uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogWarning("UIManager not found in the scene.");
        }

        if (spawner != null)
        {
            spawner.Completed.AddListener(OnSpawnCompleted);
        }

        SubscribeEvents();
        totalProgressUnits = 0;
        destroyedProgressUnits = 0;
    }

    private void DisableGameLogic()
    {
        Debug.Log("Disabling Game Logic");
        // Disable all game systems
        if (spawner != null) spawner.enabled = false;
        if (cart != null) cart.enabled = false;
        
        CartInputControl inputControl = cart.GetComponent<CartInputControl>();
        inputControl.enabled = false;
    }

    public void EnableGameLogic()
    {
        Debug.Log("Enabling Game Logic");
        // Enable all game systems
        if (spawner != null) spawner.enabled = true;
        if (cart != null) cart.enabled = true;
        
        CartInputControl inputControl = cart.GetComponent<CartInputControl>();
        inputControl.enabled = true;

        gameStarted = true;
        

    }

    private void OnCartCollisionStone()
    {
        // Stop all stones from moving on defeat
        StopAllStones();
        
        Defeat.Invoke();
        DisableGameLogic();
    }

    private void StopAllStones()
    {
        // Disable movement for all stones in the scene
        Stone[] allStones = FindObjectsByType<Stone>(FindObjectsSortMode.None);
        foreach (Stone stone in allStones)
        {
            if (stone != null)
            {
                StoneMovement movement = stone.GetComponent<StoneMovement>();
                if (movement != null)
                {
                    movement.enabled = false;
                }
            }
        }
    }

    private void Update()
    {
        if (!gameStarted) return;

        // Check for level completion when no stones remain
        if (!spawner.enabled && FindObjectsByType<Stone>(FindObjectsSortMode.None).Length == 0)
        {
            Passed.Invoke();
            DisableGameLogic();
            gameStarted = false;
        }
    }

    private void OnSpawnCompleted()
    {
        // Reset progress and calculate total when spawning is complete
        destroyedProgressUnits = 0;
        CalculateTotalProgressUnits();
        UpdateProgressUI();
    }

    /// Calculates the total number of stones (including all splits) that will ever be destroyed in this level.
    private void CalculateTotalProgressUnits()
    {
        totalProgressUnits = 0;
        int[] sizeCounts = spawner.GetInitialSizeCounts();
        for (int i = 0; i < sizeCounts.Length; i++)
        {
            totalProgressUnits += GetTotalStonesIncludingSplits((Stone.Size)i) * sizeCounts[i];
        }
    }

    /// Recursively counts all stones (including splits) for a given size.
    private int GetTotalStonesIncludingSplits(Stone.Size size)
    {
        if (size == Stone.Size.Small) return 1;
        return 1 + 2 * GetTotalStonesIncludingSplits(size - 1);
    }

    public void AddDestroyedStone(Stone.Size size)
    {
        destroyedProgressUnits++;
        UpdateProgressUI();
    }

    private void UpdateProgressUI()
    {
        if (uiManager != null)
        {
            uiManager.SendMessage("UpdateProgressText", ProgressPercent, SendMessageOptions.DontRequireReceiver);
        }
    }
}