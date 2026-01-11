using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFight : MonoBehaviour
{
    [Header("References")]
    private CharacterController controller;
    private Animator animator;
    [SerializeField] private new Transform camera;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float sprintTransitSpeed = 5f;
    [SerializeField] private float turnSpeed = 2f;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float jumpHeight = 2f;

    private float verticalVelocity;
    private float speed;
    private bool isAttacking;

    [Header("Animation")]
    private int animMoveSpeed;
    private int animJump;
    private int animGrounded;
    private int animAttackNormal;
    private int animAttackStrong;

    [Header("Input")]
    private float moveInput;
    private float turnInput;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        SetupAnimator();
    }

    private void InputManagement()
    {
        moveInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");
    }

    private void GroundMovement()
    {
        Vector3 move = new Vector3(turnInput, 0, moveInput);
        move = camera.transform.TransformDirection(move);
        if (Input.GetKey(KeyCode.LeftShift)) speed = Mathf.Lerp(speed, sprintSpeed, sprintTransitSpeed * Time.deltaTime);
        else speed = Mathf.Lerp(speed, walkSpeed, sprintTransitSpeed * Time.deltaTime);
        move *= speed;
        move.y = VerticalVelocityCalculation();
        controller.Move(move * Time.deltaTime);

        //  Animations
        animator.SetFloat(animMoveSpeed, speed * Mathf.Max(Mathf.Abs(moveInput), Mathf.Abs(turnInput)));
    }

    private void Turn()
    {
        if (Mathf.Abs(turnInput) > 0 || Mathf.Abs(moveInput) > 0)
        {
            Vector3 currentLookDirection = controller.velocity.normalized;
            currentLookDirection.y = 0;
            currentLookDirection.Normalize();
            Quaternion targetRotation = Quaternion.LookRotation(currentLookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        }
    }

    private float VerticalVelocityCalculation()
    {
        if (controller.isGrounded)
        {
            verticalVelocity = -1f;
            animator.SetBool(animGrounded, true);
            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * gravity * 2);
                animator.SetTrigger(animJump);
            }
        }
        else 
        {
            verticalVelocity -= gravity * Time.deltaTime;
            animator.SetBool(animGrounded, false);
        } 
        return verticalVelocity;
    }

    /* private void MovementAnimation()
    {
        if (Mathf.Abs(turnInput) > 0 || Mathf.Abs(moveInput) > 0) animator.SetBool("Walk", true);
        else animator.SetBool("Walk", false);
        if (controller.isGrounded) animator.SetTrigger("Land");
        else if (Input.GetButtonDown("Jump")) animator.SetTrigger("Jump");
    } */

    private void Movement()
    {
        if (isAttacking) return;
        GroundMovement();
        Turn();
        //  MovementAnimation();
    }

    private void Attack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            animator.SetTrigger(animAttackNormal);
        }

        if (Input.GetButtonDown("Fire2"))
        {
            animator.SetTrigger(animAttackStrong);
        }
    }

    public void SetAttackingState(int state)
    {
        isAttacking = (state == 1);
    }

    private void SetupAnimator()
    {
        animMoveSpeed = Animator.StringToHash("MoveSpeed");
        animJump = Animator.StringToHash("Jump");
        animGrounded = Animator.StringToHash("Grounded");
        animAttackNormal = Animator.StringToHash("Attack Normal");
        animAttackStrong = Animator.StringToHash("Attack Strong");
    }

    private void Update()
    {
        InputManagement();
        Movement();
        Attack();
    }
}
