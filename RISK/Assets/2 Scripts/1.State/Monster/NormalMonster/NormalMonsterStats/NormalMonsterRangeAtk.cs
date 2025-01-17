using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalMonsterRangeAtk : BaseState<NormalMonster>
{
    public NormalMonsterRangeAtk(StateHandler<NormalMonster> handler) : base(handler) { }

    public float atkHitTime = 0.6f;

    public override void Enter(NormalMonster monster)
    {
        Debug.Log("RangeAtk공격 시작");
        monster.animator.SetTrigger("Atk");
        monster.StartCoroutine(StartAtk(monster));
    }

    public override void Update(NormalMonster monster)
    {

    }

    public override void Exit(NormalMonster monster)
    {
        Debug.Log("RangeAtk공격 종료");
    }
    private IEnumerator StartAtk(NormalMonster monster)
    {
        yield return new WaitForSeconds(atkHitTime);
        Atk(monster);
        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo stateInfo = monster.animator.GetCurrentAnimatorStateInfo(0);
            return !stateInfo.IsName("Atk") || stateInfo.normalizedTime >= 1f;
        });
        yield return null;
        monster.nMHandler.ChangeState(typeof(NormalMonsterIdle));
    }

    private void Atk(NormalMonster monster)
    {
        GameObject bullet = monster.GetComponent<RangeShooter>().BulletSpwan();
        bullet.GetComponent<RangeBullet>().Seting(monster.target.transform.position, monster.atkDamage);
        monster.StartCoroutine(monster.AtkCoolTime());
    }
}
