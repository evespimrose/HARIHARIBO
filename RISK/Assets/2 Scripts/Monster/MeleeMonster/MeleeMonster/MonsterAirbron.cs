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

        // ���� ü�� �ð� ����
        upDuration = airborneTime * 0.4f;
        downDuration = airborneTime * 0.6f;

        // ���� ���̿� ��ǥ ���� ����
        startY = normalMonster.transform.position.y;
        targetY = startY + 5f;

        elapsedTime = 0f;
        isAscending = true;
    }

    public override void Update(BaseCharacter enemy)
    {
        if (isAscending)
        {
            // ��� �ܰ�
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
            // �ϰ� �ܰ�
            elapsedTime += Time.deltaTime;
            float newY = Mathf.Lerp(targetY, startY, elapsedTime / downDuration);
            normalMonster.transform.position = new Vector3(
                normalMonster.transform.position.x,
                newY,
                normalMonster.transform.position.z
            );

            // ���鿡 ��Ҵ��� üũ
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

