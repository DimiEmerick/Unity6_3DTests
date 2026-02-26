using System;
using System.Collections;
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
    [SerializeField] private float groundedTimer = .1f;
    [SerializeField] private float stepVerticalVelocity = -2f;

    private float verticalVelocity;
    private float speed;
    private bool isAttacking;

    [Header("Animation")]
    private int animMoveSpeed;
    private int animMoveSpeedX;
    private int animMoveSpeedZ;
    private int animJump;
    private int animGrounded;
    private int animAttackNormal;
    private int animAttackStrong;
    public float dampTime = 0.1f;

    [Header("Input")]
    private float moveInput;
    private float turnInput;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        SetupAnimator();
    }

    private void InputManagement()
    {
        moveInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");
    }

    private void GroundMovement()
    {
        if (isAttacking) return;
        Vector3 move = new Vector3(turnInput, 0, moveInput);
        move = camera.transform.TransformDirection(move);
        if (Input.GetKey(KeyCode.LeftShift)) speed = Mathf.Lerp(speed, sprintSpeed, sprintTransitSpeed * Time.deltaTime);
        else speed = Mathf.Lerp(speed, walkSpeed, sprintTransitSpeed * Time.deltaTime);
        move *= speed;
        move.y = VerticalVelocityCalculation();
        controller.Move(move * Time.deltaTime);
        Vector3 localMove = transform.InverseTransformDirection(move);

        //  Animations
        //  animator.SetFloat(animMoveSpeed, speed * Mathf.Max(Mathf.Abs(moveInput), Mathf.Abs(turnInput)));
        localMove.Normalize();
        animator.SetFloat(animMoveSpeedX, speed * localMove.x * Mathf.Max(Mathf.Abs(moveInput), Mathf.Abs(turnInput)), dampTime, Time.deltaTime);
        animator.SetFloat(animMoveSpeedZ, speed * localMove.z * Mathf.Max(Mathf.Abs(moveInput), Mathf.Abs(turnInput)), dampTime, Time.deltaTime);
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
        StartCoroutine(VerticalVelocityCalculationCoroutine()); 
        return verticalVelocity;
    }

    IEnumerator VerticalVelocityCalculationCoroutine()
    {
        if (controller.isGrounded)
        {
            verticalVelocity = stepVerticalVelocity;  //  Controlar a queda quando já estiver no chão (útil para escadas)
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
            yield return new WaitForSeconds(groundedTimer);
            if (!controller.isGrounded) animator.SetBool(animGrounded, false);
        }
        yield return verticalVelocity;
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
        animMoveSpeedX = Animator.StringToHash("MoveSpeedX");
        animMoveSpeedZ = Animator.StringToHash("MoveSpeedZ");
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
