using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BossMonsterSkillA : BaseState<BossMonster>
{
    public BossMonsterSkillA(StateHandler<BossMonster> handler) : base(handler) { }

    public float atkADuration = 1f;
    public float atkADamage = 1f;
    private bool isAtkA = false;

    public float atkBDuration = 1f;
    public float atkBDamage = 1f;
    private bool isAtkB = false;

    public float outDuration = 1f;
    private float curTime = 0f;

    public override void Enter(BossMonster monster)
    {
        Debug.Log("스킬A 진입");
        curTime = 0f;
        isAtkA = false;
        isAtkB = false;
    }

    public override void Update(BossMonster monster)
    {
        if (curTime > atkADuration)
        {
            SkillAtkA(monster);
        }
        else if (curTime > atkBDuration && isAtkA)
        {
            SkillAtkB(monster);
        }
        else if (curTime > outDuration && isAtkA && isAtkB)
        {
            //상태 변환
            monster.bMHandler.ChangeState(typeof(BossMonsterIdle));
        }

    }

    public override void Exit(BossMonster monster)
    {

    }

    public void SkillAtkA(BossMonster monster)
    {
        //애니메이션 재생
        Debug.Log("SkillAtkA공격");
        isAtkA = true;
        Vector3 atkDir = Quaternion.Euler(0, 45, 0) * monster.transform.forward;
        //monster.transform.position = 공격판정범위 중심
        //monster.atkRange = 공격판정의 범위(원형)
        Collider[] cols = Physics.OverlapSphere(monster.transform.position, monster.atkRange);
        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                Vector3 dirToTarget = (col.transform.position - monster.transform.position).normalized;
                //범위내에 있는지 확인
                float angle = Vector3.Angle(atkDir, dirToTarget);
                if (angle <= 90f)
                {
                    col.gameObject.GetComponent<ITakedamage>().Takedamage(monster.atkDamage);
                }
                else
                {
                    Debug.Log("공격판정 밖임");
                }
            }
        }
        curTime = 0f;
    }

    public void SkillAtkB(BossMonster monster)
    {
        //애니메이션 재생
        Debug.Log("SkillAtkB공격");
        isAtkB = true;
        Vector3 atkDir = Quaternion.Euler(0, -45, 0) * monster.transform.forward;
        //monster.transform.position = 공격판정범위 중심
        //monster.atkRange = 공격판정의 범위(원형)
        Collider[] cols = Physics.OverlapSphere(monster.transform.position, monster.atkRange);
        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                Vector3 dirToTarget = (col.transform.position - monster.transform.position).normalized;
                //범위내에 있는지 확인
                float angle = Vector3.Angle(atkDir, dirToTarget);
                if (angle <= 90f)
                {
                    col.gameObject.GetComponent<ITakedamage>().Takedamage(monster.atkDamage);
                }
                else
                {
                    Debug.Log("공격판정 밖임");
                }
            }
        }
        curTime = 0f;
    }
}
