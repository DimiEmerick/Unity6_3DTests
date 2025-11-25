using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    public Rigidbody body;               //  Corpo físico do jogador, usado para aplicar forças físicas
    public float speed = 2f;
    public float turnSpeed = 250f;
    public float forceJump = 3f;
    public bool isGrounded;

    [Header("Ground Check")]
    public LayerMask groundMask;         //  Layer que representa o chão
    public Transform groundCheck;        //  Um vazio posicionado no pé do personagem
    public float groundDistance = .25f;  //  Tamanho do check (raio da esfera)

    private float _currentSpeed;

    private void Start()
    {
        _currentSpeed = speed;
    }

    private void Walk()
    {
        //  Direção da câmera, usada para mover o player relativo ao ângulo atual da câmera
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;
        //  Zera o eixo Y pra evitar inclinação de movimento
        camForward.y = 0f;
        camRight.y = 0f;
        //  Normaliza vetores para garantir magnitude 1 (evita aumento de velocidade)
        camForward.Normalize();
        camRight.Normalize();
        Vector3 direction = Vector3.zero;  //  Direção final do movimento
        //  Recebe entrada do teclado e soma vetores
        if (Input.GetKey(KeyCode.W)) direction += camForward;  //  Frente
        if (Input.GetKey(KeyCode.A)) direction -= camRight;    //  Esquerda
        if (Input.GetKey(KeyCode.S)) direction -= camForward;  //  Trás
        if (Input.GetKey(KeyCode.D)) direction += camRight;    //  Direita
        if (Input.GetKeyDown(KeyCode.LeftShift)) _currentSpeed = speed * 2;
        else if (Input.GetKeyUp(KeyCode.LeftShift)) _currentSpeed = speed;
        if (direction != Vector3.zero)  //  Só movimenta se houver direção pressionada
        {
            direction = direction.normalized;  //  Normaliza a direção (evita dobro de velodiade nas diagonais)
            //  Move o jogador usando incremento da posição
            transform.position += _currentSpeed * Time.deltaTime * direction;
            //  Rotaciona o jogador suavemente para olhar na direção de movimento
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }

    private void Jump()
    {
        //  Só pula se estiver no chão e a tecla Espaço for pressionada
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) body.linearVelocity = Vector3.up * forceJump;
    }

    private void Update()
    {
        //  Posiciona uma esfera invisível que checa se o personagem está tocando o chão
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        Walk();
        Jump();
    }
}
