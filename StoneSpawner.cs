using UnityEngine;
using UnityEngine.Events;

public class StoneSpawner : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private Stone stonePrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnRate;

    [Header("Balance")]
    [SerializeField] private Turret turret;
    [SerializeField] private int amountLevel1;
    [SerializeField] private int amountLevel3;
    [SerializeField] private int amountLevel6;
    [SerializeField][Range(0.0f, 1.0f)] private float minHitpointsPercentage;
    [SerializeField] private float maxHitpointsRate;

    [Header("Stone Colors")]
    [SerializeField] private Color[] stoneColors;

    [Space(10)] public UnityEvent Completed;

    private float timer;
    private float amountSpawned;
    private int stoneMaxHitpoints;
    private int stoneMinHitpoints;
    private int[] initialSizeCounts = new int[4];
    private int currentAmount;

    private void Start()
    {
        int damagePerSecond = (int)((turret.Damage * turret.ProjectileAmount) * (1 / turret.FireRate));
        stoneMaxHitpoints = (int)(damagePerSecond * maxHitpointsRate);
        stoneMinHitpoints = (int)(stoneMaxHitpoints * minHitpointsPercentage);

        timer = spawnRate;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnRate)
        {
            Spawn();
            timer = 0;
        }

        if (amountSpawned >= currentAmount)
        {
            enabled = false;
            Completed.Invoke();
        }
    }

    private void UpdateAmountForCurrentLevel()
    {
        int level = LevelState.CurrentLevel;
        
        if (level <= 1)
        {
            currentAmount = amountLevel1;
        }
        else if (level <= 3)
        {
            float t = (level - 1) / 2f;
            currentAmount = Mathf.RoundToInt(Mathf.Lerp(amountLevel1, amountLevel3, t));
        }
        else if (level <= 6)
        {
            float t = (level - 3) / 3f;
            currentAmount = Mathf.RoundToInt(Mathf.Lerp(amountLevel3, amountLevel6, t));
        }
        else
        {
            currentAmount = amountLevel6;
        }
    }

    private void Spawn()
    {
        Stone.Size size;
        if (LevelState.CurrentLevel >= 5)
        {
            size = (Stone.Size)Random.Range(1, 4);
        }
        else
        {
            size = (Stone.Size)Random.Range(1, 3);
        }

        for (int i = 0; amountSpawned < currentAmount; i++)
        {
            Stone stone = Instantiate(stonePrefab, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
            stone.SetSize(size);
            stone.maxHitPoints = Random.Range(stoneMinHitpoints, stoneMaxHitpoints + 1);
            initialSizeCounts[(int)size]++;
            Debug.Log($"Spawned stone size: {size}, Count for size {size}: {initialSizeCounts[(int)size]}");

            if (stoneColors.Length > 0)
            {
                Color randomColor = stoneColors[Random.Range(0, stoneColors.Length)];
                stone.SetStoneColor(randomColor);
            }

            amountSpawned++;
        }
    }

    public int GetAmount() { return currentAmount; }
    public int[] GetInitialSizeCounts() { return initialSizeCounts; }
    
    public void ResetSpawner()
    {
        amountSpawned = 0;
        timer = spawnRate;
        UpdateAmountForCurrentLevel();
        
        for (int i = 0; i < initialSizeCounts.Length; i++)
        {
            initialSizeCounts[i] = 0;
        }
    }
}