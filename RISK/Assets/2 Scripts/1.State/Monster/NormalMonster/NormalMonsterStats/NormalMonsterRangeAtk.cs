using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalMonsterRangeAtk : BaseState<NormalMonster>
{
    public NormalMonsterRangeAtk(StateHandler<NormalMonster> handler) : base(handler) { }

    public float atkDuration = 1f;
    public float atkDelay = 0.6f;
    private float curTime = 0;
    private bool isAtk = false;

    public override void Enter(NormalMonster monster)
    {
        monster.animator.SetTrigger("Atk");
        curTime = 0;
        isAtk = false;
    }

    public override void Update(NormalMonster monster)
    {
        curTime += Time.deltaTime; // 경과 시간 누적
        if (isAtk == false) monster.transform.LookAt(monster.target);
        // 공격 시작 전 대기 시간
        if (curTime >= atkDelay && !isAtk)
        {
            Atk(monster); // atkDelay만큼 기다린 후 공격 발동
            isAtk = true; // 공격이 발동했음을 표시
        }

        // 공격 지속 시간이 지나면 상태 변경
        if (curTime >= atkDuration)
        {
            monster.nMHandler.ChangeState(typeof(NormalMonsterIdle));
        }
    }

    public override void Exit(NormalMonster monster)
    {
         
    }

    private void Atk(NormalMonster monster)
    {
        GameObject bullet = monster.GetComponent<RangeShooter>().BulletSpwan();
        bullet.GetComponent<RangeBullet>().Seting(monster.target.transform.position, monster.atkDamage);
        monster.StartCoroutine(monster.AtkCoolTime());
    }
}
