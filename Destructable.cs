using System;
using UnityEngine;
using UnityEngine.Events;

public class Destructable : MonoBehaviour
{
    [SerializeField] private int maxHitPoints;
    private int hitPoints;

    public UnityEvent Die;
    public UnityEvent ChangeHitPoints;

    public void Start()
    {
        hitPoints = maxHitPoints;
    }
    public void ApplyDamage(int damage)
    {
        ChangeHitPoints.Invoke();

        hitPoints -= damage;
        if (hitPoints <= 0)
        {
            Kill();
        }
    }

    public void Kill()
    {
        hitPoints = 0;
        Die.Invoke();
        ChangeHitPoints.Invoke();
    }

    public int GetHitPoints()
    {
        return hitPoints;
    }

    public int GetMaxHitPoints()
    {
        return maxHitPoints;
    }
}
