using System;
using System.Security;
using UnityEngine;

public class StoneMovement : MonoBehaviour
{
    private Vector3 velocity;
    [SerializeField] private bool useGravity;

    [SerializeField] private float gravity;
    [SerializeField] private float reboundSpeed;
    [SerializeField] private float horizontalSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float gravityOffset;

    private void Awake()
    {
        velocity.x = -Mathf.Sign(transform.position.x) * horizontalSpeed;
    }

    private void Update()
    {
        TryEnableGrabity();
        Move();
    }

    private void TryEnableGrabity()
    {
        if (Math.Abs(transform.position.x) <= Math.Abs(LevelBoundary.Instance.LeftBorder) - gravityOffset)
        {
            useGravity = true;
        }
    }

    private void Move()
    {
        if (useGravity == true)
        {
            velocity.y -= gravity * Time.deltaTime;
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }

        velocity.x = Mathf.Sign(velocity.x) * horizontalSpeed;
        transform.position += velocity * Time.deltaTime;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        LevelEdge levelEdge = collision.GetComponent<LevelEdge>();

        if (levelEdge != null)
        {
            if (levelEdge.Type == EdgeType.Bottom)
            {
                velocity.y = reboundSpeed;
            }
            if (levelEdge.Type == EdgeType.Left && velocity.x < 0 || levelEdge.Type == EdgeType.Right && velocity.x > 0)
            {
                velocity.x *= -1;
            }
        }
    }

    public void AddVerticalVelicity(float velicity)
    {
        this.velocity.y += velicity;
    }

    public void SetHorizontalDirection(float direction)
    {

        velocity.x = Mathf.Sign(direction) * horizontalSpeed;
    }
}