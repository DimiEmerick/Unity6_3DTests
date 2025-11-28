using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public HealthBase health;
    public Collider enemyCollider;
    public bool lookAtPlayer = false;

    private PlayerSpeed _player;

    private void Awake()
    {
        _player = FindAnyObjectByType<PlayerSpeed>();
    }

    protected virtual void OnCollisionEnter(Collision collision)
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

    protected virtual void OnKill()
    {
        enemyCollider.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (lookAtPlayer)
        {
            transform.LookAt(_player.transform.position);
        }
    }
}
