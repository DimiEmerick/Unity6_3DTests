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

    [Header("Input")]
    private float moveInput;
    private float turnInput;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
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
            if (Input.GetButtonDown("Jump")) verticalVelocity = Mathf.Sqrt(jumpHeight * gravity * 2);
        }
        else verticalVelocity -= gravity * Time.deltaTime;
        return verticalVelocity;
    }

    private void MovementAnimation()
    {
        if (Mathf.Abs(turnInput) > 0 || Mathf.Abs(moveInput) > 0) animator.SetBool("Walk", true);
        else animator.SetBool("Walk", false);
        if (controller.isGrounded) animator.SetTrigger("Land");
        else if (Input.GetButtonDown("Jump")) animator.SetTrigger("Jump");
    }

    private void Movement()
    {
        GroundMovement();
        Turn();
        MovementAnimation();
    }

    private void Update()
    {
        InputManagement();
        Movement();
    }
}
