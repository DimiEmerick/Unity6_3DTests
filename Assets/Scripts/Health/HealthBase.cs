using System;
using UnityEngine;

public class HealthBase : MonoBehaviour
{
    public float startLife;
    public float currentLife;
    public float healMultiplier = 1f;
    public float damageMultiplier = 1f;
    public bool destroyOnKill = false;

    private void Awake()
    {
        ResetLife();
    }

    private void ResetLife()
    {
        currentLife = startLife;
    }

    private void Heal(float heal)
    {
        currentLife += heal + healMultiplier;
    }

    public void Damage(float damage)
    {
        if (currentLife > 0)
        {
            currentLife -= damage * damageMultiplier;
        }
        else Kill();
    }

    private void Kill()
    {
        if (destroyOnKill) Destroy(gameObject);
    }
}
