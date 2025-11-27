using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public HealthBase health;
    public Collider Collider;
    public bool lookAtPlayer = false;

    private PlayerSpeed _player;

    private void Awake()
    {
        _player = FindAnyObjectByType<PlayerSpeed>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.TryGetComponent<PlayerSpeed>(out var player)) player.health.Damage(1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<GroundCheck>(out var playerFeet))
        {
            health.Damage(1);
            _player.Jump();
            if (health.currentLife <= 0) OnKill();
        }
    }

    private void OnKill()
    {
        Collider.gameObject.SetActive(false);
    }
}
