using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Type = System.Type;

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
        if (Vector3.Distance(monster.target.position, monster.transform.position) < monster.atkRange && !monster.isAtk)
        {
            monster.AtkEnd();

            // 스킬 랜덤 선택
            Type selectedSkillType = GetRandomSkillType();

            // 방장만 상태 변경 관리
            if (PhotonNetwork.IsMasterClient)
            {
                monster.eMHandler.ChangeState(selectedSkillType);
                monster.photonView.RPC("SyncSkillStateChange", RpcTarget.All, selectedSkillType.ToString());
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

    private Type GetRandomSkillType()
    {
        // 랜덤으로 스킬 선택
        switch (Random.Range(0, 3))
        {
            case 0: return typeof(EliteMonsterSkillA);
            case 1: return typeof(EliteMonsterSkillB);
            case 2: return typeof(EliteMonsterSkillC);
            default: return null;
        }
    }
}
