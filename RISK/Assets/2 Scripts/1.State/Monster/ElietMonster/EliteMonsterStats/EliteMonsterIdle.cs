using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditorInternal.VersionControl.ListControl;

public class EliteMonsterIdle : BaseState<EliteMonster>
{
    public EliteMonsterIdle(StateHandler<EliteMonster> handler) : base(handler) { }

    public override void Enter(EliteMonster monster)
    {
        Debug.Log("Idle진입");
        monster.animator?.SetTrigger("Idle");
    }

    public override void Update(EliteMonster monster)
    {
        if (Vector3.Distance(monster.target.position, monster.transform.position) < monster.atkRange && monster.isAtk == false)
        {
            if (!monster.isAtk)
            {
            //공격으로 이동
            monster.AtkEnd();
            int a = Random.Range(0, 3);
                switch (a)
                {
                    case 0:
                        monster.eMHandler.ChangeState(typeof(EliteMonsterSkillA));
                        break;
                    case 1:
                        monster.eMHandler.ChangeState(typeof(EliteMonsterSkillB));
                        break;
                    case 2:
                        monster.eMHandler.ChangeState(typeof(EliteMonsterSkillC));
                        break;
                }
            }
            else
            {
                Debug.Log("공격범위지만 공격쿨타임임");
            }
        }
        else
        {
            monster.eMHandler.ChangeState(typeof(EliteMonsterMove));
        }
    }

    public override void Exit(EliteMonster monster)
    {
        Debug.Log("Idle퇴장");
    }
}
