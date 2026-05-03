using System.Collections;
using UnityEngine;

public class PlayerShotgun : MonoBehaviour
{
    [Header("References")]
    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    [Header("Shotgun Stats")]
    public float bulletVelocity;
    public float bulletRange;

    public void Fire()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
       
        bullet.GetComponent<Rigidbody>().AddForce(bulletSpawn.forward.normalized * bulletVelocity, ForceMode.Impulse);

        StartCoroutine(DestroyBulletAfterTime(bullet, bulletRange));
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }

    public void OnAnimationStart()
    {
        PlayerAnimations.Instance.IsRightArmPlaying = true;
    }

    public void OnAnimationEnd()
    {
        PlayerAnimations.Instance.IsRightArmPlaying = false;
    }
}
