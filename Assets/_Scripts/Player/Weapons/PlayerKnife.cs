using UnityEngine;

public class PlayerKnife : Weapon
{
    [Header("References")]
    public Camera playerCam;

    [Header("Knife Stats")]
    public float attackRange = 3f;
    public float attackDamage;
    public int powerUpBonus = 2;

    private static readonly int hit1 = Animator.StringToHash("Hit1");
    private static readonly int hit2 = Animator.StringToHash("Hit2");
    private static readonly int hit3 = Animator.StringToHash("Hit3");

    private readonly int[] hitTriggers = { hit1, hit2, hit3 };

    private int comboIndex;
    private bool attackButtonHeld;

    public override void OnAttackPressed()
    {
        attackButtonHeld = true;

        StartAttack();
    }

    public override void OnAttackReleased()
    {
        attackButtonHeld = false;
    }

    private void StartAttack()
    {
        if (isPlaying)
            return;

        animator.SetTrigger(hitTriggers[comboIndex]);

        comboIndex++;

        if (comboIndex >= hitTriggers.Length)
            comboIndex = 0;
    }

    /// <summary>
    /// Wird per Animation Event aufgerufen.
    /// </summary>
    public void Attack()
    {
        armorHitThisAttack.Clear();

        if (Physics.Raycast(
                playerCam.transform.position,
                playerCam.transform.forward,
                out RaycastHit hit,
                attackRange,
                Combined))
        {
            ProcessHit(hit, attackDamage);
        }
    }

    /// <summary>
    /// Animation Event am Anfang der Animation.
    /// </summary>
    public override void OnAnimationStart()
    {
        base.OnAnimationStart();
    }

    /// <summary>
    /// Animation Event am Ende der Animation.
    /// </summary>
    public override void OnAnimationEnd()
    {
        base.OnAnimationEnd();

        if (attackButtonHeld)
            StartAttack();
    }

    protected override void EnemyBodyHit(Vector3 pos, Collider col, float damage)
    {
        PlayerStatsAndUIPanel.Instance.ChangePowerUp(powerUpBonus);

        base.EnemyBodyHit(pos, col, damage);
    }

    protected override void EnemyHeadHit(Vector3 pos, Collider col, float damage)
    {
        PlayerStatsAndUIPanel.Instance.ChangePowerUp(powerUpBonus);

        base.EnemyHeadHit(pos, col, damage);
    }
}