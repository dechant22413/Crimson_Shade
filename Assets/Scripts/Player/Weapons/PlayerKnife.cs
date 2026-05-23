using UnityEngine;

public class PlayerKnife : MonoBehaviour
{
    [Header("References")]
    public LayerMask enemyBodyHit;
    public LayerMask enemyHeadHit;
    public LayerMask environmentHit;
    public LayerMask enemyArmorHit;
    public Camera playerCam;

    [Header("Knife Stats")]
    public float attackRange = 3f;
    public float attackDamage;
    public int powerUpBonus = 2;

    public void Attack()
    {
        Debug.Log("Attack() aufgerufen");
        LayerMask combined = enemyBodyHit | enemyHeadHit | environmentHit | enemyArmorHit;

        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out RaycastHit hit, attackRange, combined))
        {
            int layer = hit.collider.gameObject.layer;

            if (((1 << layer) & enemyBodyHit) != 0)
                EnemyBodyHit(hit.point, hit.collider);
            else if (((1 << layer) & enemyHeadHit) != 0)
                EnemyHeadHit(hit.point, hit.collider);
            else if (((1 << layer) & enemyArmorHit) != 0)
                EnemyArmorHit(hit.point, hit.collider);
            else if (((1 << layer) & environmentHit) != 0)
                EnvironmentHit(hit.point);
        }
    }

    private void EnemyBodyHit(Vector3 pos, Collider col)
    {
        Debug.Log("Body Hit!");
        PlayerStatsAndUIPanel.Instance.ChangePowerUp(powerUpBonus);

        EnemyBase enemy = col.GetComponentInParent<EnemyBase>();
        if (enemy != null)
            enemy.TakeDamage(attackDamage);
    }

    private void EnemyHeadHit(Vector3 pos, Collider col)
    {
        Debug.Log("Head Hit!");
        PlayerStatsAndUIPanel.Instance.ChangePowerUp(powerUpBonus);

        EnemyBase enemy = col.GetComponentInParent<EnemyBase>();
        if (enemy != null)
            enemy.TakeDamage(attackDamage * 2f);
    }

    private void EnemyArmorHit(Vector3 pos, Collider col)
    {
        Debug.Log("ArmorHit");

        EnemyBase enemy = col.GetComponentInParent<EnemyBase>();
        if (enemy != null)
            enemy.ArmorHit();
    }

    private void EnvironmentHit(Vector3 pos)
    {
        Debug.Log("Environment Hit");
    }

    public void OnAnimationStart()
    {
        PlayerAnimations.Instance.IsLeftArmPlaying = true;
    }

    public void OnAnimationEnd()
    {
        PlayerAnimations.Instance.IsLeftArmPlaying = false;
    }
}
