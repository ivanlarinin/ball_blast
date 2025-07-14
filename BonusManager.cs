using System.Collections;
using UnityEngine;

/*
    Bonus System Explanation:

    The bonus system in this game introduces special collectible items ("bonuses") that provide temporary effects to the player when collected. The system consists of the following components:

    1. BonusType Enum:
        - Defines the types of bonuses available in the game.
        - Currently includes:
            - Freeze: Temporarily freezes all stones, stopping their movement.
            - Invincibility: Temporarily makes the player's cart invincible to collisions.

    2. Bonus MonoBehaviour:
        - Represents a bonus item in the game world.
        - Fields:
            - lifeTime: How long the bonus exists before disappearing if not collected.
            - bonusType: The type of bonus this object provides.
            - rb: Reference to the Rigidbody2D for physics interactions.
            - landed: Tracks if the bonus has landed on the ground.
        - On Awake:
            - Initializes the Rigidbody2D and schedules the bonus for destruction after its lifetime expires.
        - OnTriggerEnter2D:
            - If the cart collides with the bonus, the corresponding effect is triggered via the BonusManager.
            - The bonus is then destroyed.
            - If the bonus lands on the ground, it stops moving and becomes kinematic.

    3. BonusManager Singleton:
        - Manages the activation and timing of bonus effects.
        - Handles two main effects:
            - Freeze: Disables movement for all stones for a set duration.
            - Invincibility: Sets the cart to an invincible state for a set duration.
        - Ensures that only one instance of each effect can be active at a time.
        - Uses coroutines to manage effect durations and automatically revert effects when time is up.

    4. Integration:
        - Stones have a chance to spawn a bonus when destroyed.
        - When the player collects a bonus, the effect is applied immediately and lasts for a limited time.

    In summary, the bonus system adds variety and strategic opportunities for the player by providing temporary advantages when bonuses are collected.
*/

public class BonusManager : MonoBehaviour
{
    public static BonusManager Instance;

    [SerializeField] private float freezeDuration = 5f;
    [SerializeField] private float invincibilityDuration = 5f;

    private bool isFrozen = false;
    private bool isInvincible = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void TriggerFreeze()
    {
        if (!isFrozen)
            StartCoroutine(FreezeCoroutine());
    }

    public void TriggerInvincibility(Cart cart)
    {
        if (!isInvincible)
            StartCoroutine(InvincibilityCoroutine(cart));
    }

    private IEnumerator FreezeCoroutine()
    {
        isFrozen = true;
        StoneMovement[] stones = FindObjectsByType<StoneMovement>(FindObjectsSortMode.None);
        foreach (var stone in stones)
        {
            stone.SetFrozen(true);
        }
        yield return new WaitForSeconds(freezeDuration);
        foreach (var stone in stones)
        {
            stone.SetFrozen(false);
        }
        isFrozen = false;
    }

    private IEnumerator InvincibilityCoroutine(Cart cart)
    {
        isInvincible = true;
        cart.SetInvincible(true);
        yield return new WaitForSeconds(invincibilityDuration);
        cart.SetInvincible(false);
        isInvincible = false;
    }

    public bool IsFrozen() => isFrozen;
    public bool IsInvincible() => isInvincible;
} 