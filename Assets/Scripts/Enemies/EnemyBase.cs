using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public Animator animator;
    public HealthBase health;
    public Collider enemyCollider;
    public Rigidbody body;
    public int damageAmount = 1;
    public float animationDuration = 1f;
    public float rotationSpeed = 5f;
    public bool lookAtPlayer = false;

    private PlayerSpeed _player;

    private void Awake()
    {
        _player = FindAnyObjectByType<PlayerSpeed>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(transform.name + " colidiu com: " + collision.transform.name);
        if (collision.transform.TryGetComponent<PlayerSpeed>(out var player)) Attack(player);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GroundCheck"))
        {
            health.Damage(1);
            _player.Jump();
            if (health.currentLife <= 0) Kill();
        } 
    }

    protected virtual void Attack(PlayerSpeed player)
    {
        player.health.Damage(damageAmount);
        animator?.SetTrigger("Attack");
    }

    protected virtual void Kill()
    {
        enemyCollider.enabled = false;
        body.useGravity = false;
        animator?.SetTrigger("Death");
        if (health.destroyOnKill) 
        {
            Debug.Log("Matou o " + transform.name);
            Destroy(gameObject, animationDuration);
        }
    }

    private void Update()
    {
        if (lookAtPlayer && _player != null)
        {
            Vector3 direction = (_player.transform.position - transform.position).normalized;
            direction.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
