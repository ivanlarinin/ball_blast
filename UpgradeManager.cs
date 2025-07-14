using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    // PlayerPrefs keys for persistent upgrade data
    private const string FireRateKey = "FireRateLevel";
    private const string DamageKey = "DamageLevel";
    private const string ProjectileAmountKey = "ProjectileAmountLevel";

    // Upgrade cost calculation parameters
    private const int BaseUpgradeCost = 5;
    private const int CostIncreasePerLevel = 6;

    // Current upgrade levels with public getters
    public int FireRateLevel { get; private set; }
    public int DamageLevel { get; private set; }
    public int ProjectileAmountLevel { get; private set; }

    // Event triggered when upgrades change
    public System.Action OnUpgradesChanged;

    private void Awake()
    {
        LoadUpgrades();
    }

    private void LoadUpgrades()
    {
        // Load saved upgrade levels from PlayerPrefs
        FireRateLevel = PlayerPrefs.GetInt(FireRateKey, 0);
        DamageLevel = PlayerPrefs.GetInt(DamageKey, 0);
        ProjectileAmountLevel = PlayerPrefs.GetInt(ProjectileAmountKey, 0);
    }

    private void SaveUpgrades()
    {
        // Save current upgrade levels to PlayerPrefs
        PlayerPrefs.SetInt(FireRateKey, FireRateLevel);
        PlayerPrefs.SetInt(DamageKey, DamageLevel);
        PlayerPrefs.SetInt(ProjectileAmountKey, ProjectileAmountLevel);
        PlayerPrefs.Save();
    }

    public int GetUpgradeCost(int currentLevel)
    {
        // Calculate cost based on current level
        return BaseUpgradeCost + (currentLevel * CostIncreasePerLevel);
    }

    public bool CanUpgradeFireRate()
    {
        CoinManager coinManager = FindFirstObjectByType<CoinManager>();
        if (coinManager == null) return false;
        
        int cost = GetUpgradeCost(FireRateLevel);
        return coinManager.GetCurrentCoins() >= cost;
    }

    public bool CanUpgradeDamage()
    {
        CoinManager coinManager = FindFirstObjectByType<CoinManager>();
        if (coinManager == null) return false;
        
        int cost = GetUpgradeCost(DamageLevel);
        return coinManager.GetCurrentCoins() >= cost;
    }

    public bool CanUpgradeProjectileAmount()
    {
        CoinManager coinManager = FindFirstObjectByType<CoinManager>();
        if (coinManager == null) return false;
        
        int cost = GetUpgradeCost(ProjectileAmountLevel);
        return coinManager.GetCurrentCoins() >= cost;
    }

    public bool UpgradeFireRate()
    {
        if (!CanUpgradeFireRate()) return false;

        CoinManager coinManager = FindFirstObjectByType<CoinManager>();
        int cost = GetUpgradeCost(FireRateLevel);
        coinManager.AddCoins(-cost);
        
        FireRateLevel++;
        SaveUpgrades();
        OnUpgradesChanged?.Invoke();
        return true;
    }

    public bool UpgradeDamage()
    {
        if (!CanUpgradeDamage()) return false;

        CoinManager coinManager = FindFirstObjectByType<CoinManager>();
        int cost = GetUpgradeCost(DamageLevel);
        coinManager.AddCoins(-cost);
        
        DamageLevel++;
        SaveUpgrades();
        OnUpgradesChanged?.Invoke();
        return true;
    }

    public bool UpgradeProjectileAmount()
    {
        if (!CanUpgradeProjectileAmount()) return false;

        CoinManager coinManager = FindFirstObjectByType<CoinManager>();
        int cost = GetUpgradeCost(ProjectileAmountLevel);
        coinManager.AddCoins(-cost);
        
        ProjectileAmountLevel++;
        SaveUpgrades();
        OnUpgradesChanged?.Invoke();
        return true;
    }

    public void ResetUpgrades()
    {
        // Reset all upgrade levels to 0
        FireRateLevel = 0;
        DamageLevel = 0;
        ProjectileAmountLevel = 0;
        SaveUpgrades();
        OnUpgradesChanged?.Invoke();
    }
} 