using UnityEngine;

[RequireComponent(typeof(StoneMovement))]
public class Stone : Destructable
{
    public enum Size
    {
        Small,
        Normal,
        Big,
        Huge
    }

    [SerializeField] private Size size;
    [SerializeField] private Stone stoneObject;
    [SerializeField] private float spawnUpForce;

    [SerializeField] private SpriteRenderer visualModelSpriteRenderer; 

    [SerializeField] private GameObject coinPrefab;
    [SerializeField][Range(0f, 1f)] private float coinSpawnChance = 0.5f;


    private StoneMovement movement;

    private void Awake()
    {
        movement = GetComponent<StoneMovement>();

        Die.AddListener(OnStoneDestroyed);
        SetSize(size);

        if (visualModelSpriteRenderer == null)
        {
            Transform visualModel = transform.Find("VisualModel");
            if (visualModel != null)
            {
                visualModelSpriteRenderer = visualModel.GetComponent<SpriteRenderer>();
            }
        }
    }

    private void OnDestroy()
    {
        Die.RemoveListener(OnStoneDestroyed);
    }

    private void OnStoneDestroyed()
    {
        LevelState levelState = FindObjectOfType<LevelState>();
        if (levelState != null)
        {
            levelState.AddDestroyedStone(size);
        }
        if (size != Size.Small)
        {
            SpawnStones();
        }
        TrySpawnCoin();
        Destroy(gameObject);
    }

    private void TrySpawnCoin()
    {
        if (coinPrefab != null && Random.value <= coinSpawnChance)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }
    }

    private void SpawnStones()
    {
        for (int i = 0; i < 2; i++)
        {
            Stone stone = Instantiate(stoneObject, transform.position, Quaternion.identity);
            stone.SetSize(size - 1);
            stone.maxHitPoints = Mathf.Clamp(maxHitPoints / 2, 1, maxHitPoints);
            stone.movement.AddVerticalVelicity(spawnUpForce);
            stone.movement.SetHorizontalDirection((i % 2 * 2) - 1);
        }
    }

    public void SetSize(Size size)
    {
        if (size < 0) return;
        transform.localScale = GetVectorFromSize(size);
        this.size = size;
    }

    private Vector3 GetVectorFromSize(Size size)
    {
        if (size == Size.Huge) return new Vector3(1, 1, 1);
        if (size == Size.Big) return new Vector3(0.75f, 0.75f, 0.75f);
        if (size == Size.Normal) return new Vector3(.6f, .6f, .6f);
        if (size == Size.Small) return new Vector3(0.4f, 0.4f, 0.4f);
        return Vector3.one;
    }

    public void SetStoneColor(Color color)
    {
        if (visualModelSpriteRenderer != null)
        {
            visualModelSpriteRenderer.color = color;
        }
        else
        {
            Debug.LogWarning("VisualModel SpriteRenderer not found on Stone. Cannot set color.");
        }
    }
}