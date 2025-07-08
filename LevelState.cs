using UnityEngine;
using UnityEngine.Events;

public class LevelState : MonoBehaviour
{
    private StoneSpawner spawner;
    private Cart cart;
    private UIManager uiManager;

    public UnityEvent Passed = new UnityEvent();
    public UnityEvent Defeat = new UnityEvent();

    private float timer;
    private bool checkPassed;
    private bool gameStarted = false;

    // Progress tracking
    private int totalProgressUnits;
    private int destroyedProgressUnits;

    /// <summary>
    /// Returns progress as a value from 0 to 1.
    /// </summary>
    public float Progress01 => totalProgressUnits == 0 ? 0f : (float)destroyedProgressUnits / totalProgressUnits;

    /// <summary>
    /// Returns progress as a percentage (0-100).
    /// </summary>
    public float ProgressPercent => Progress01 * 100f;

    private const string LevelKey = "CurrentLevel";
    public static int CurrentLevel
    {
        get => PlayerPrefs.GetInt(LevelKey, 1);
        set { PlayerPrefs.SetInt(LevelKey, value); PlayerPrefs.Save(); }
    }
    public static void ResetLevel()
    {
        PlayerPrefs.SetInt(LevelKey, 1);
        PlayerPrefs.Save();
    }

    private void SubscribeEvents()
    {
        Defeat.AddListener(DisableGameLogic);
        Passed.AddListener(DisableGameLogic);
        if (cart != null)
            cart.CollisionStone.AddListener(OnCartCollisionStone);
    }

    private void UnsubscribeEvents()
    {
        Defeat.RemoveListener(DisableGameLogic);
        Passed.RemoveListener(DisableGameLogic);
        if (cart != null)
            cart.CollisionStone.RemoveListener(OnCartCollisionStone);
    }

    private void Awake()
    {
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
        else
        {
            uiManager.OnGameStart.AddListener(OnGameStart);
        }

        if (spawner != null)
        {
            spawner.Completed.AddListener(OnSpawnCompleted);
        }
        SubscribeEvents();
        totalProgressUnits = 0;
        destroyedProgressUnits = 0;
    }

    private void OnDestroy()
    {
        if (uiManager != null)
        {
            uiManager.OnGameStart.RemoveListener(OnGameStart);
        }
        if (spawner != null)
        {
            spawner.Completed.RemoveListener(OnSpawnCompleted);
        }
        UnsubscribeEvents();
    }

    private void OnGameStart()
    {
        Debug.Log("Game Started");
        gameStarted = true;
        EnableGameLogic();
    }

    private void DisableGameLogic()
    {
        Debug.Log("Disabling Game Logic");
        if (spawner != null) spawner.enabled = false;
        if (cart != null) cart.enabled = false;
        
        CartInputControl inputControl = cart.GetComponent<CartInputControl>();
        inputControl.enabled = false;
    }

    private void EnableGameLogic()
    {
        Debug.Log("Enabling Game Logic");
        if (spawner != null) spawner.enabled = true;
        if (cart != null) cart.enabled = true;
        
        CartInputControl inputControl = cart.GetComponent<CartInputControl>();
        inputControl.enabled = true;
    }

    private void OnCartCollisionStone()
    {
        if (!gameStarted) return;
        CoinManager coinManager = FindObjectOfType<CoinManager>();
        if (coinManager != null)
        {
            coinManager.ResetCoins();
        }
        Defeat.Invoke();
        DisableGameLogic();
        ResetLevel(); // Add this line to reset level on defeat
    }

    private void Update()
    {
        if (!gameStarted) return;
        timer += Time.deltaTime;

        if (timer > 0.5f)
        {
            if (checkPassed == true)
            {
                if (FindObjectsByType<Stone>(FindObjectsSortMode.None).Length == 0)
                {
                    Passed.Invoke();
                    CoinManager coinManager = FindObjectOfType<CoinManager>();
                    if (coinManager != null)
                    {
                        coinManager.SaveCoins();
                    }
                    DisableGameLogic();
                }
            }
            timer = 0;
        }
    }

    private void OnSpawnCompleted()
    {
        checkPassed = true;
        destroyedProgressUnits = 0;
        CalculateTotalProgressUnits();
        UpdateProgressUI();
    }

    /// <summary>
    /// Calculates the total number of stones (including all splits) that will ever be destroyed in this level.
    /// </summary>
    private void CalculateTotalProgressUnits()
    {
        totalProgressUnits = 0;
        if (spawner == null) return;
        int[] sizeCounts = spawner.GetInitialSizeCounts();
        for (int i = 0; i < sizeCounts.Length; i++)
        {
            totalProgressUnits += GetTotalStonesIncludingSplits((Stone.Size)i) * sizeCounts[i];
        }
    }

    /// <summary>
    /// Recursively counts all stones (including splits) for a given size.
    /// </summary>
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