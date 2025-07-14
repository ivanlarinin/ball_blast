using UnityEngine;

public enum BonusType
{
    Freeze,
    Invincibility
}

public class Bonus : MonoBehaviour
{
    [SerializeField] private float lifeTime = 10f;
    [SerializeField] private BonusType bonusType;

    private Rigidbody2D rb;
    private bool landed = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime);
    }

    public void SetBonusType(BonusType type)
    {
        bonusType = type;
    }

    public BonusType GetBonusType()
    {
        return bonusType;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Cart cart = collision.transform.root.GetComponent<Cart>();
        if (cart != null)
        {
            if (BonusManager.Instance != null)
            {
                if (bonusType == BonusType.Freeze)
                {
                    BonusManager.Instance.TriggerFreeze();
                }
                else if (bonusType == BonusType.Invincibility)
                {
                    BonusManager.Instance.TriggerInvincibility(cart);
                }
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