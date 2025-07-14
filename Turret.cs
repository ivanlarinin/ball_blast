using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] private float baseFireRate = 1f;
    [SerializeField] private int baseDamage = 10;
    [SerializeField] private int baseProjectileAmount = 1;
    [SerializeField] private float projectileInterval;

    [Header("References")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform shootPoint;

    private float timer;
    private UpgradeManager upgradeManager;

    // Current stats with upgrades applied
    public float FireRate => baseFireRate * (1f - (upgradeManager.FireRateLevel * 0.1f)); // 10% faster per level
    public int Damage => baseDamage + (upgradeManager.DamageLevel * 5); // +5 damage per level
    public int ProjectileAmount => baseProjectileAmount + upgradeManager.ProjectileAmountLevel; // +1 projectile per level

    private void Awake()
    {
        upgradeManager = FindFirstObjectByType<UpgradeManager>();
        if (upgradeManager == null)
        {
            Debug.LogWarning("UpgradeManager not found. Creating one.");
            upgradeManager = new GameObject("UpgradeManager").AddComponent<UpgradeManager>();
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
    }

    private void SpawnProjectile()
    {
        float startPosX = shootPoint.position.x - projectileInterval * (ProjectileAmount - 1) * 0.5f;

        for (int i = 0; i < ProjectileAmount; i++)
        {
            Projectile projectile = Instantiate(projectilePrefab, new Vector3(startPosX + i * projectileInterval, shootPoint.position.y, shootPoint.position.z), transform.rotation);
            projectile.SetDamage(Damage);
        }
    }

    public void Fire()
    {
        if (timer >= FireRate)
        {
            SpawnProjectile();
            timer = 0;
        }
    }
}
