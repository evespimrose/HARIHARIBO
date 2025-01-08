using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalMonsterRangeAtk : BaseState<NormalMonster>
{
    public NormalMonsterRangeAtk(StateHandler<NormalMonster> handler) : base(handler) { }

    public float atkDuration = 1f;
    public float atkDelay = 0.4f;
    private float curTime = 0;
    private bool isAtk = false;

    public override void Enter(NormalMonster monster)
    {
        //entity.animator.SetTrigger("Atk");
        curTime = 0;
        isAtk = false;
    }

    public override void Update(NormalMonster monster)
    {
        if (curTime - atkDuration < 0.1f)
        {
            //공격종료
            monster.nMHandler.ChangeState(typeof(NormalMonsterIdle));
        }
        if (curTime - atkDelay < 0.1f && isAtk == false)
        {
            Atk(monster);
            monster.isAtk = true;
        }
        curTime += Time.deltaTime;
    }

    public override void Exit(NormalMonster monster)
    {
         
    }

    private void Atk(NormalMonster entity)
    {
        GameObject bullet = entity.GetComponent<RangeShooter>().BulletSpwan();
        bullet.GetComponent<RangeBullet>().Seting(entity.target.transform.position, entity.atkDamage);
        entity.StartCoroutine(entity.AtkCoolTime());
    }
}
