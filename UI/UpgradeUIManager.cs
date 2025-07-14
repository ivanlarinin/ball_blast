using UnityEngine;
using UnityEngine.UI;

// This script manages the UI for upgrading turret stats, handling button clicks, displaying upgrade levels and costs, and updating interactability based on the player's coins.


public class UpgradeUIManager : MonoBehaviour
{
    [Header("Upgrade UI")]
    [SerializeField] private Button upgradeFireRateButton;
    [SerializeField] private Button upgradeDamageButton;
    [SerializeField] private Button upgradeProjectileAmountButton;
    [SerializeField] private Text fireRateLevelText;
    [SerializeField] private Text damageLevelText;
    [SerializeField] private Text projectileAmountLevelText;
    [SerializeField] private Text fireRateCostText;
    [SerializeField] private Text damageCostText;
    [SerializeField] private Text projectileAmountCostText;

    private UpgradeManager upgradeManager;
    private CoinManager coinManager;

    private void Awake()
    {
        // Find and cache references to managers
        upgradeManager = FindFirstObjectByType<UpgradeManager>();
        coinManager = FindFirstObjectByType<CoinManager>();
        SetupButtonListeners();
    }

    private void OnEnable()
    {
        // Subscribe to events for UI updates
        if (upgradeManager != null)
            upgradeManager.OnUpgradesChanged += UpdateUpgradeUI;
        if (coinManager != null)
            coinManager.OnCoinsChanged += UpdateUpgradeUI;
        UpdateUpgradeUI();
    }

    private void OnDisable()
    {
        // Unsubscribe from events 
        if (upgradeManager != null)
            upgradeManager.OnUpgradesChanged -= UpdateUpgradeUI;
        if (coinManager != null)
            coinManager.OnCoinsChanged -= UpdateUpgradeUI;
    }

    private void SetupButtonListeners()
    {
        // Connect button clicks to upgrade methods
        upgradeFireRateButton.onClick.AddListener(UpgradeFireRate);
        upgradeDamageButton.onClick.AddListener(UpgradeDamage);
        upgradeProjectileAmountButton.onClick.AddListener(UpgradeProjectileAmount);
    }

    private void UpgradeFireRate()
    {
        TryUpgrade(() => upgradeManager.UpgradeFireRate());
    }

    private void UpgradeDamage()
    {
        TryUpgrade(() => upgradeManager.UpgradeDamage());
    }

    private void UpgradeProjectileAmount()
    {
        TryUpgrade(() => upgradeManager.UpgradeProjectileAmount());
    }

    private void TryUpgrade(System.Func<bool> upgradeAction)
    {
        // Attempt upgrade and refresh UI if successful
        if (upgradeAction())
        {
            UpdateUpgradeUI();
        }
    }

    public void UpdateUpgradeUI()
    {
        if (upgradeManager == null) return;
        
        // Update level display texts
        fireRateLevelText.text = $"Level {upgradeManager.FireRateLevel}";
        damageLevelText.text = $"Level {upgradeManager.DamageLevel}";
        projectileAmountLevelText.text = $"Level {upgradeManager.ProjectileAmountLevel}";
        
        // Update cost display texts
        fireRateCostText.text = upgradeManager.GetUpgradeCost(upgradeManager.FireRateLevel).ToString();
        damageCostText.text = upgradeManager.GetUpgradeCost(upgradeManager.DamageLevel).ToString();
        projectileAmountCostText.text = upgradeManager.GetUpgradeCost(upgradeManager.ProjectileAmountLevel).ToString();
        
        // Update button interactability based on available coins
        upgradeFireRateButton.interactable = upgradeManager.CanUpgradeFireRate();
        upgradeDamageButton.interactable = upgradeManager.CanUpgradeDamage();
        upgradeProjectileAmountButton.interactable = upgradeManager.CanUpgradeProjectileAmount();
    }
} 