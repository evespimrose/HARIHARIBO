using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class NormalMonsterMeleeAtk : BaseState<NormalMonster>
{
    public NormalMonsterMeleeAtk(StateHandler<NormalMonster> handler) : base(handler) { }

    public float meleeAtkDamage;
    public float atkHitTime = 0.4f;
    private Coroutine action;

    public override void Enter(NormalMonster monster)
    {
        meleeAtkDamage = monster.atkDamage * 1f;
        Debug.Log("MeleeAtk공격 시작");
        monster.animator.SetTrigger("Atk");
        action = monster.StartCoroutine(StartAtk(monster));
    }

    public override void Update(NormalMonster monster)
    {

    }

    public override void Exit(NormalMonster monster)
    {
        monster.StopCoroutine(action);
        Debug.Log("MeleeAtk공격 종료");
    }

    private IEnumerator StartAtk(NormalMonster monster)
    {
        yield return new WaitForSeconds(atkHitTime);
        monster.TargetLook(monster.target.position);
        Atk(monster);
        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo stateInfo = monster.animator.GetCurrentAnimatorStateInfo(0);
            return !stateInfo.IsName("Atk") || stateInfo.normalizedTime >= 1f;
        });
        yield return new WaitUntil(() => monster.isAtk == false);
        monster.nMHandler.ChangeState(typeof(NormalMonsterIdle));
    }

    private void Atk(NormalMonster monster)
    {
        Debug.Log("MeleeAtk공격");
        Vector3 atkDir = monster.transform.forward;
        //monster.transform.position = 공격판정범위 중심
        //monster.atkRange = 공격판정의 범위(원형)
        Collider[] cols = Physics.OverlapSphere(monster.transform.position, monster.atkRange);
        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag("LocalPlayer") || col.gameObject.CompareTag("RemotePlayer"))
            {
                Vector3 dirToTarget = (col.transform.position - monster.transform.position).normalized;
                //정면기준으로 반원범위내에 있는지 확인
                float angle = Vector3.Angle(atkDir, dirToTarget);
                if (angle <= 90f)
                {
                    col.gameObject.GetComponent<ITakedamage>().Takedamage(meleeAtkDamage);
                    //monster.CalculateAndSendDamage(col.gameObject, meleeAtkDamage);
                }
                else
                {
                    Debug.Log("공격판정 밖임");
                }
            }
        }
    }
}
