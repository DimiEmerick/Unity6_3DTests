using UnityEngine;

public class PlayerFight : MonoBehaviour
{
    public CharacterController characterController;
    public float speed;

    private void Walk()
    {
        string direction;
        if (Input.GetKey(KeyCode.W)) direction = "forward";
        if (Input.GetKey(KeyCode.A)) direction = "left";
        if (Input.GetKey(KeyCode.S)) direction = "right";
        if (Input.GetKey(KeyCode.D)) direction = "back";
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.W))
            characterController.SimpleMove(Vector3.forward * Time.deltaTime * speed);
    }
}
