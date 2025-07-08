using System;
using UnityEngine;
using UnityEngine.Events;

public class Destructable : MonoBehaviour
{
    public int maxHitPoints;
    private int hitPoints;

    [HideInInspector] public UnityEvent Die;
    [HideInInspector] public UnityEvent ChangeHitPoints;

    private bool isDie = false;

    public void Start()
    {
        hitPoints = maxHitPoints;
        ChangeHitPoints.Invoke();
    }

    public void ApplyDamage(int damage)
    {
        hitPoints -= damage;

        ChangeHitPoints.Invoke();

        if (hitPoints <= 0)
        {
            Kill();
        }
    }

    public void Kill()
    {
        if (isDie == true) return;
        hitPoints = 0;
        isDie = true;
        ChangeHitPoints.Invoke();
        Die.Invoke();
    }

    public int GetHitPoints()
    {
        return hitPoints;
    }
}
