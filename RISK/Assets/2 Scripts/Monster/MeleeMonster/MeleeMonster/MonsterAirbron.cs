using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class MonsterAirbron : NormalMonsterBaseState
{
    private NormalMonster normalMonster;

    private float airborneTime = 1f;
    private float upDuration;
    private float downDuration;
    private float startY;
    private float targetY;
    private float elapsedTime = 0f;
    private bool isAscending = true;

    public override void Enter(BaseCharacter enemy)
    {
        normalMonster = enemy as NormalMonster;
        normalMonster.isGround = false;
        normalMonster.animator?.SetTrigger("Airborne");

        // 공중 체공 시간 설정
        upDuration = airborneTime * 0.4f;
        downDuration = airborneTime * 0.6f;

        // 시작 높이와 목표 높이 설정
        startY = normalMonster.transform.position.y;
        targetY = startY + 5f;

        elapsedTime = 0f;
        isAscending = true;
    }

    public override void Update(BaseCharacter enemy)
    {
        if (isAscending)
        {
            // 상승 단계
            elapsedTime += Time.deltaTime;
            if (elapsedTime < upDuration)
            {
                float newY = Mathf.Lerp(startY, targetY, elapsedTime / upDuration);
                normalMonster.transform.position = new Vector3(
                    normalMonster.transform.position.x,
                    newY,
                    normalMonster.transform.position.z
                );
            }
            else
            {
                isAscending = false;
                elapsedTime = 0f;
            }
        }
        else
        {
            // 하강 단계
            elapsedTime += Time.deltaTime;
            float newY = Mathf.Lerp(targetY, startY, elapsedTime / downDuration);
            normalMonster.transform.position = new Vector3(
                normalMonster.transform.position.x,
                newY,
                normalMonster.transform.position.z
            );

            // 지면에 닿았는지 체크
            if (normalMonster.isGround)
            {
                normalMonster.ChangeState(new MonsterIdle());
            }
        }
    }

    public override void Exit(BaseCharacter enemy)
    {
        normalMonster.isAirborne = false;
    }
}

