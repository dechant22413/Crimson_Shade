using UnityEngine;
using System.Collections.Generic;

public abstract class Weapon : MonoBehaviour
{
    [Header("Hit Layers")]
    public LayerMask enemyBodyHit;
    public LayerMask enemyHeadHit;
    public LayerMask enemyArmorHit;
    public LayerMask enemyHostHit;
    public LayerMask environmentHit;

    protected LayerMask Combined => enemyBodyHit | enemyHeadHit | enemyArmorHit | enemyHostHit | environmentHit;

    protected HashSet<EnemyBase> armorHitThisAttack = new HashSet<EnemyBase>();

    protected virtual void ProcessHit(RaycastHit hit, float damage)
    {
        int layer = hit.collider.gameObject.layer;

        if (((1 << layer) & enemyBodyHit) != 0)
            EnemyBodyHit(hit.point, hit.collider, damage);
        else if (((1 << layer) & enemyHeadHit) != 0)
            EnemyHeadHit(hit.point, hit.collider, damage * 2f);
        else if (((1 << layer) & enemyArmorHit) != 0)
            EnemyArmorHit(hit.point, hit.collider);
        else if (((1 << layer) & enemyHostHit) != 0)
            EnemyHostHit(hit.point, hit.collider, damage);
        else if (((1 << layer) & environmentHit) != 0)
            EnvironmentHit(hit.point);
    }

    protected virtual void EnemyBodyHit(Vector3 pos, Collider col, float damage)
    {
        EnemyBase enemy = col.GetComponentInParent<EnemyBase>();
        if (enemy != null) enemy.TakeDamage(damage);
    }

    protected virtual void EnemyHeadHit(Vector3 pos, Collider col, float damage)
    {
        EnemyBase enemy = col.GetComponentInParent<EnemyBase>();
        if (enemy != null) enemy.TakeDamage(damage);
    }

    protected virtual void EnemyArmorHit(Vector3 pos, Collider col)
    {
        EnemyBase enemy = col.GetComponentInParent<EnemyBase>();
        if (enemy != null && !armorHitThisAttack.Contains(enemy))
        {
            armorHitThisAttack.Add(enemy);
            enemy.ArmorHit();
        }
    }

    protected virtual void EnemyHostHit(Vector3 pos, Collider col, float damage)
    {

    }

    protected virtual void EnvironmentHit(Vector3 pos) { }
}