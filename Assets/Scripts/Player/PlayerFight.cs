using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFight : MonoBehaviour
{
    public Animator animator;
    public CharacterController characterController;
    public float speed;
    public float turnSpeed;

    public InputAction move;
    public InputAction jump;

    private void Walk()
    {
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

        direction = direction.normalized;
        characterController.SimpleMove(direction * Time.deltaTime * speed);

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            animator.SetBool("Walk", true);
        }
        else animator.SetBool("Walk", false);
    }

    private void Update()
    {
        Walk();
    }
}
