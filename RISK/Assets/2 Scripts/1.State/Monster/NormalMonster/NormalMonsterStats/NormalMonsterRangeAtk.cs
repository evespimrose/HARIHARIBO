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
        curTime += Time.deltaTime; // ��� �ð� ����
        if (isAtk == false) monster.transform.LookAt(monster.target);
        // ���� ���� �� ��� �ð�
        if (curTime >= atkDelay && !isAtk)
        {
            Atk(monster); // atkDelay��ŭ ��ٸ� �� ���� �ߵ�
            isAtk = true; // ������ �ߵ������� ǥ��
        }

        // ���� ���� �ð��� ������ ���� ����
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
