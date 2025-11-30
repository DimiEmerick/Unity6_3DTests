using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public HealthBase health;
    public Collider enemyCollider;
    public float rotationSpeed = 5f;
    public bool lookAtPlayer = false;

    private PlayerSpeed _player;

    private void Awake()
    {
        _player = FindAnyObjectByType<PlayerSpeed>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Colidiu com: " + collision.transform.name);
        if (collision.transform.TryGetComponent<PlayerSpeed>(out var player)) player.health.Damage(1);
        if (collision.transform.CompareTag("GroundCheck"))
        {
            health.Damage(1);
            _player.Jump();
            if (health.currentLife <= 0) OnKill();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.transform.name + " entrou da área de visão do " + transform.name);

        if (other.CompareTag("Player")) lookAtPlayer = true;
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log(other.transform.name + " saiu da área de visão do " + transform.name);
        if (other.CompareTag("Player")) lookAtPlayer = false;
    }

    protected virtual void OnKill()
    {
        enemyCollider.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (lookAtPlayer && _player != null)
        {
            Vector3 direction = (_player.transform.position - transform.position).normalized;
            direction.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            //  transform.LookAt(_player.transform.position);
        }
    }
}
