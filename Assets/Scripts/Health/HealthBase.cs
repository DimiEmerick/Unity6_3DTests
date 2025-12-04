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

    public Action<HealthBase> OnKill;
    public Action<HealthBase> OnDamage;

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
        if (currentLife <= 0) Kill();
        UpdateLifeUI(currentLife);
        OnDamage?.Invoke(this);
    }

    public Vector3 Damage(float damage, Vector3 hitPosition)
    {
        currentLife -= damage * damageMultiplier;
        if (currentLife <= 0) Kill();
        UpdateLifeUI(currentLife);
        Vector3 knockbackDirection = (transform.position - hitPosition).normalized;
        OnDamage?.Invoke(this);
        return knockbackDirection;
    }

    public void Kill()
    {
        gameObject.GetComponent<Collider>().enabled = false;
        gameObject.GetComponent<Rigidbody>().useGravity = false;
        gameObject.GetComponentInChildren<Animator>()?.SetTrigger("Death");
        OnKill?.Invoke(this);
    }

    public void UpdateLifeUI(float life)
    {
        if (lifeText != null) lifeText.text = life.ToString();
    }
}
