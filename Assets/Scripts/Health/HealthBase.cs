using UnityEngine;
using TMPro;
using System;

public class HealthBase : MonoBehaviour
{
    public TextMeshProUGUI lifeText;
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
        UpdateLifeUI(currentLife);
    }

    private void Heal(float heal)
    {
        currentLife += heal + healMultiplier;
        UpdateLifeUI(currentLife);
    }

    public void Damage(float damage)
    {
        currentLife -= damage * damageMultiplier;
        UpdateLifeUI(currentLife);
    }

    public void UpdateLifeUI(float life)
    {
        if (lifeText != null) lifeText.text = life.ToString();
    }
}
