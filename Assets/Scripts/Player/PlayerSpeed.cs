using TMPro;
using UnityEngine;

public class PlayerSpeed : MonoBehaviour
{
    public Animator animator;
    public Rigidbody body;               //  Corpo físico do jogador, usado para aplicar forças físicas
    public TextMeshProUGUI velocimeter;
    public HealthBase health;

    [Header("Movement")]
    public float acceleration = 50f;
    public float maxSpeed = 10f;
    public float turnSpeed = 10f;
    public float forceJump = 3f;

    [Header("Ground Check")]
    public LayerMask groundMask;         //  Layer que representa o chão
    public Transform groundCheck;        //  Um vazio posicionado no pé do personagem
    public float groundDistance = .25f;  //  Tamanho do check (raio da esfera)

    [Header("Debug")]
    public TextMeshProUGUI linearVelocityX;
    public TextMeshProUGUI linearVelocityZ;
    public TextMeshProUGUI magnitude;

    private float _boostSpeed;
    [SerializeField] private float _currentSpeed;
    private bool _isGrounded;
    //  private bool _isBoosting;

    private void Start()
    {
        _currentSpeed = 0f;
        _boostSpeed = maxSpeed * 2f;
    }

    private void Walk()
    {
        //  Cálculo da Direção
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) direction += camForward;
        if (Input.GetKey(KeyCode.A)) direction -= camRight;
        if (Input.GetKey(KeyCode.S)) direction -= camForward;
        if (Input.GetKey(KeyCode.D)) direction += camRight;

        //  Lógica de Boost
        float currentMaxSpeed = maxSpeed;
        if (Input.GetKey(KeyCode.LeftShift) && _isGrounded) // Use GetKey para manter o boost
        {
            currentMaxSpeed = _boostSpeed;
        }

        if (direction != Vector3.zero)
        {
            direction = direction.normalized;

            //  Aplica a Força (Aceleração)
            body.AddForce(direction * acceleration, ForceMode.Impulse);

            //  Limita a Velocidade
            //  Pegamos a velocidade atual no plano XZ
            Vector3 flatVelocity = new Vector3(body.linearVelocity.x, 0f, body.linearVelocity.z);
            linearVelocityX.text = "Linear Velocity X: " + body.linearVelocity.x.ToString();
            linearVelocityZ.text = "Linear Velocity Z: " + body.linearVelocity.z.ToString();
            magnitude.text = "Magnitude: " + flatVelocity.magnitude.ToString();

            if (flatVelocity.magnitude > currentMaxSpeed)
            {
                // Se a velocidade exceder o limite, a normalizamos e multiplicamos pelo limite
                Vector3 limitedVelocity = flatVelocity.normalized * currentMaxSpeed;

                // Aplicamos a velocidade limitada, mantendo a velocidade Y (para o pulo/gravidade)
                body.linearVelocity = new Vector3(limitedVelocity.x, body.linearVelocity.y, limitedVelocity.z);
            }

            //  Rotação e Animação
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime); // Use Time.fixedDeltaTime

            // Lógica de Animação (use a velocidade real do Rigidbody)
            float animationSpeed = flatVelocity.magnitude / maxSpeed;
            animator.speed = Mathf.Max(1f, animationSpeed); // Garante que a velocidade mínima seja 1
            animator.SetBool("Run", true);
            velocimeter.text = flatVelocity.magnitude.ToString("F1");
        }
        else // Desaceleração
        {
            //  Desaceleração (Opcional: A física já faz isso com Drag)
            //  Se você quiser uma parada mais abrupta, aumente o Drag (Arrasto) no Rigidbody.
            //  Se quiser uma parada suave, use o Drag padrão.

            // Para garantir que ele pare completamente no plano XZ
            Vector3 flatVelocity = new Vector3(body.linearVelocity.x, 0f, body.linearVelocity.z);
            if (flatVelocity.magnitude > 0.3f)
            {
                // Aplica uma força contrária para desacelerar
                body.AddForce(0.5f * acceleration * -flatVelocity.normalized, ForceMode.Impulse);
            }
            else
            {
                // Zera a velocidade XZ para evitar deslize
                body.linearVelocity = new Vector3(0f, body.linearVelocity.y, 0f);
            }

            // Lógica de Animação
            animator.SetBool("Run", false);
            animator.speed = 1;
            velocimeter.text = flatVelocity.magnitude.ToString("F1");
        }
    }
    public void Jump()
    {
        body.linearVelocity = Vector3.up * forceJump; //  Aplica uma velocidade linear para cima multiplicado pela força do pulo
        animator.SetTrigger("Jump");  // Aplica o trigger de animação do pulo
    }

    private void Update()
    {
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (!_isGrounded) animator.SetBool("Falling", true);  //  Define como true o bool da animação de queda se o Player não estiver no chão 
        else
        {
            animator.SetBool("Falling", false);  //  Define como false o bool da animação de queda se o Player estiver no chão
            animator.SetTrigger("Land");  //  Ativa o trigger da animação de aterrissagem
        }
    }

    private void FixedUpdate()
    {
        Walk();  //  Chamada do método Walk() para melhor controle da física
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded) Jump();  //  Se o Player apertar a tecla Espaço e estiver no chão, o método Jump() é chamado
    }
}
