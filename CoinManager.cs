using UnityEngine;
using UnityEngine.UI;

/*
    CoinManager Explanation:

    The CoinManager is responsible for managing the player's coin balance throughout the game. Its main responsibilities include:

    1. Tracking Coins:
        - Maintains the current number of coins the player has (`currentCoins`).
        - Provides methods to add coins (`AddCoins`), reset coins (`ResetCoins`), and retrieve the current coin count (`GetCurrentCoins`).

    2. Persistence:
        - Saves the coin count to persistent storage using Unity's PlayerPrefs (`SaveCoins`).
        - Loads the coin count from PlayerPrefs when the game starts (`LoadCoins`).

    3. UI Updates:
        - Updates the on-screen coin display (`UpdateCoinUI`) whenever the coin amount changes.
        - Uses a reference to a UI Text component to show the current coin count.

    4. Event Notification:
        - Exposes an event (`OnCoinsChanged`) that is triggered whenever the coin amount changes.
        - Allows other systems (such as UI or upgrade managers) to react to coin changes and update accordingly.

    5. Integration:
        - Coins are collected in-game (e.g., when the player picks up a coin object).
        - Other systems (like upgrades) can check the player's coin balance and deduct coins as needed.

    In summary, the CoinManager centralizes all logic related to coin collection, display, saving/loading, and event notification, ensuring a consistent and persistent coin experience for the player.
*/


public class CoinManager : MonoBehaviour
{
    [SerializeField] private Text coinText;
    private int currentCoins = 0;
    private const string CoinsKey = "Coins";

    // Event triggered when coin amount changes
    public System.Action OnCoinsChanged;

    private void Awake()
    {
        LoadCoins();
        UpdateCoinUI();
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        UpdateCoinUI();
        OnCoinsChanged?.Invoke();
    }

    public void ResetCoins()
    {
        currentCoins = 0;
        SaveCoins();
        UpdateCoinUI();
        OnCoinsChanged?.Invoke();
    }

    public void SaveCoins()
    {
        // Save current coin count to PlayerPrefs
        PlayerPrefs.SetInt(CoinsKey, currentCoins);
        PlayerPrefs.Save();
    }

    public void LoadCoins()
    {
        // Load saved coin count from PlayerPrefs
        currentCoins = PlayerPrefs.GetInt(CoinsKey, 0);
        UpdateCoinUI();
    }

    public int GetCurrentCoins()
    {
        return currentCoins;
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
            coinText.text = "Coins: " + currentCoins;
        else
            Debug.LogWarning("Coin Text UI element not assigned to CoinManager.");
    }
}