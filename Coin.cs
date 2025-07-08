using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 5f;
    [SerializeField] private float lifeTime = 10f;
    [SerializeField] private int value = 1;

    private Rigidbody2D rb;
    private bool landed = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Cart cart = collision.transform.root.GetComponent<Cart>();
        if (cart != null)
        {
            CoinManager coinManager = FindObjectOfType<CoinManager>();
            if (coinManager != null)
            {
                coinManager.AddCoins(value);
            }
            Destroy(gameObject);
            return;
        }

        LevelEdge levelEdge = collision.GetComponent<LevelEdge>();
        if (levelEdge != null && levelEdge.Type == EdgeType.Bottom)
        {
            if (!landed)
            {
                landed = true;
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.bodyType = RigidbodyType2D.Kinematic;
                }
            }
        }
    }
}