using UnityEngine;
using UnityEngine.UI;

public class CoinManager : MonoBehaviour
{
    [SerializeField] private Text coinText;
    private int currentCoins = 0;
    private const string CoinsKey = "Coins";

    private void Awake()
    {
        LoadCoins();
        UpdateCoinUI();
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        UpdateCoinUI();
    }

    public void ResetCoins()
    {
        currentCoins = 0;
        SaveCoins();
        UpdateCoinUI();
    }

    public void SaveCoins()
    {
        PlayerPrefs.SetInt(CoinsKey, currentCoins);
        PlayerPrefs.Save();
    }

    public void LoadCoins()
    {
        currentCoins = PlayerPrefs.GetInt(CoinsKey, 0);
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