using UnityEngine;

public class fireWeapon : MonoBehaviour
{
    public Rigidbody bullet;      //  Captura o Rigidbody da bala que será atirada
    public Transform spawnPoint;  //  A posição onde a bala deve ser spawnada

    public int multiplier;        //  A quantidade de força aplicada

    public void FireWeapon()
    {
        //  Spawna a bala e a atira para frente
        Rigidbody bulletInstance;
        bulletInstance = Instantiate(bullet, spawnPoint.position, bullet.transform.rotation) as Rigidbody;
        bulletInstance.AddForce(spawnPoint.forward * multiplier);
    }
}
