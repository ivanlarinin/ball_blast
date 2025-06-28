using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float lifeTime;
    [SerializeField] private int damage; 

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(0, speed * Time.deltaTime, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destructable destructable = collision.transform.root.GetComponent<Destructable>();

        if (destructable != null)
        {
            destructable.ApplyDamage(damage);
        }

        Destroy(gameObject);
    }
}
