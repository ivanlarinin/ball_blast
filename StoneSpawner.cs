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
    [SerializeField] private int amount;
    [SerializeField][Range(0.0f, 1.0f)] private float minHitpointsPercentage;
    [SerializeField] private float maxHitpointsRate;

    [Header("Stone Colors")]
    [SerializeField] private Color[] stoneColors;

    [Space(10)] public UnityEvent Completed;

    private float timer;
    private float amountSpawned;
    private int stoneMaxHitpoints;
    private int stoneMinHitpoints;
    private int[] initialSizeCounts = new int[4]; // Huge, Big, Normal, Small

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

        if (amountSpawned >= amount)
        {
            enabled = false;

            Completed.Invoke();
        }
    }

    private void Spawn()
    {
        Stone.Size size = (Stone.Size)Random.Range(1, 4); // 1=Normal, 2=Big, 3=Huge
        Stone stone = Instantiate(stonePrefab, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
        stone.SetSize(size);
        stone.maxHitPoints = Random.Range(stoneMinHitpoints, stoneMaxHitpoints + 1);
        initialSizeCounts[(int)size]++;

        if (stoneColors.Length > 0)
        {
            Color randomColor = stoneColors[Random.Range(0, stoneColors.Length)];
            stone.SetStoneColor(randomColor);
        }

        amountSpawned++;
    }

    public int GetAmount() { return amount; }
    public int[] GetInitialSizeCounts() { return initialSizeCounts; }
}