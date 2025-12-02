using UnityEngine;
using TMPro;

public class PlayerSpeed : MonoBehaviour
{
    public Animator animator;
    public Rigidbody body;               //  Corpo f�sico do jogador, usado para aplicar for�as f�sicas
    public TextMeshProUGUI velocimeter;
    public HealthBase health;

    [Header("Movement")]
    public float acceleration = 1.0001f;
    public float maxSpeed = 3f;
    public float turnSpeed = 250f;
    public float forceJump = 3f;

    [Header("Ground Check")]
    public LayerMask groundMask;         //  Layer que representa o ch�o
    public Transform groundCheck;        //  Um vazio posicionado no p� do personagem
    public float groundDistance = .25f;  //  Tamanho do check (raio da esfera)

    //  private Vector3 _lastDirection;
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
        // 1. Cálculo da Direção
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

        // 2. Lógica de Boost
        float currentMaxSpeed = maxSpeed;
        if (Input.GetKey(KeyCode.LeftShift)) // Use GetKey para manter o boost
        {
            currentMaxSpeed = _boostSpeed;
        }

        if (direction != Vector3.zero)
        {
            direction = direction.normalized;
            //  _lastDirection = direction;

            // 3. Aplica a Força (Aceleração)
            body.AddForce(direction * acceleration, ForceMode.Impulse);

            // 4. Limita a Velocidade (O Ponto Chave)
            // Pegamos a velocidade atual no plano XZ
            Vector3 flatVelocity = new Vector3(body.linearVelocity.x, 0f, body.linearVelocity.z);

            if (flatVelocity.magnitude > currentMaxSpeed)
            {
                // Se a velocidade exceder o limite, a normalizamos e multiplicamos pelo limite
                Vector3 limitedVelocity = flatVelocity.normalized * currentMaxSpeed;

                // Aplicamos a velocidade limitada, mantendo a velocidade Y (para o pulo/gravidade)
                body.linearVelocity = new Vector3(limitedVelocity.x, body.linearVelocity.y, limitedVelocity.z);
            }

            // 5. Rota��o e Anima��o (Seu c�digo de rota��o est� bom)
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime); // Use Time.fixedDeltaTime

            // L�gica de Anima��o (use a velocidade real do Rigidbody)
            float animationSpeed = flatVelocity.magnitude / maxSpeed;
            animator.speed = Mathf.Max(1f, animationSpeed); // Garante que a velocidade m�nima seja 1
            animator.SetBool("Run", true);
        }
        else // Desacelera��o
        {
            // 6. Desacelera��o (Opcional: A f�sica j� faz isso com Drag)
            // Se voc� quiser uma parada mais abrupta, aumente o Drag (Arrasto) no Rigidbody.
            // Se quiser uma parada suave, use o Drag padr�o.

            // Para garantir que ele pare completamente no plano XZ
            Vector3 flatVelocity = new Vector3(body.linearVelocity.x, 0f, body.linearVelocity.z);
            if (flatVelocity.magnitude > 0.1f)
            {
                // Aplica uma for�a contr�ria para desacelerar
                body.AddForce(-flatVelocity.normalized * acceleration * 0.5f, ForceMode.Impulse);
            }
            else
            {
                // Zera a velocidade XZ para evitar deslize
                body.linearVelocity = new Vector3(0f, body.linearVelocity.y, 0f);
            }

            // L�gica de Anima��o
            animator.SetBool("Run", false);
            animator.speed = 1;
        }
    }

    /*    PULO
     *    Para pular, o c�digo pega o Rigidbody do Player e aplica uma velocidade linear (em linha reta), essa velocidade
     * � um Vetor para cima (0, 1, 0) multiplicado pela vari�vel de for�a do pulo que vai determinar o qu�o alto o Player
     * ir� pular.
     *    Tamb�m � feita uma altera��o na velocidade da anima��o para o valor padr�o (1), isso evita que a velocidade da 
     * anima��o seja a mesma da velocidade da corrida que est� sendo alterada constantemente no m�todo de andar Walk().
     */
    public void Jump()
    {
        body.linearVelocity = Vector3.up * forceJump;
        animator.speed = 1;
        animator.SetTrigger("Jump");
    }

    /*    UPDATE
     *    � posicionado uma esfera invis�vel que checa se o Player est� tocando o ch�o, essa esfera est� posicionada na
     * mesma posi��o do transform de groundCheck, possui um raio do tamanho de groundDistance e uma m�scara da camada
     * que vai ser considerada o "ch�o" (groundMask). Se a esfera estiver colidindo com qualquer objeto que tenha essa
     * camada, retorna true para a vari�vel _isGrounded, ou seja, o Player est� tocando o ch�o.
     *    � feito uma checagem se o Player n�o estiver no ch�o, ent�o a anima��o de cair "Falling", ser� marcada como true.
     * Caso ele estiver tocando o ch�o, "Falling" ser� false e o trigger de aterrisagem "Land" ser� marcado (� configurado
     * no Animator para a anima��o de aterrisagem s� acontecer ap�s a anima��o de cair, para ele n�o ficar repetindo a
     * anima��o de aterrisagem a cada frame.
     *    O m�todo de andar � executado.
     *    O m�todo de pulo s� � executado se o Player estiver no ch�o e apertar a tecla Espa�o.
     *    O texto do veloc�metro � alterado constantemente para a velocidade atual (_currentSpeed), que � transformada em
     * string para poder ser exibida na UI. 
     */
    private void Update()
    {
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (!_isGrounded) animator.SetBool("Falling", true);
        else
        {
            animator.SetBool("Falling", false);
            animator.SetTrigger("Land");
        }
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded) Jump();
        velocimeter.text = _currentSpeed.ToString();
    }

    private void FixedUpdate()
    {
        Walk();
    }
}
