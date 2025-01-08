using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class NormalMonsterAirbron : BaseState<NormalMonster>
{
    public NormalMonsterAirbron(StateHandler<NormalMonster> handler) : base(handler) { }

    private float airborneTime = 1f;
    private float upDuration;
    private float downDuration;
    private float startY;
    private float targetY;
    private float elapsedTime = 0f;
    private bool isAscending = true;

    public override void Enter(NormalMonster enemy)
    {
        enemy.isGround = false;
        enemy.animator?.SetTrigger("Airborne");

        // ���� ü�� �ð� ����
        upDuration = airborneTime * 0.4f;
        downDuration = airborneTime * 0.6f;

        // ���� ���̿� ��ǥ ���� ����
        startY = enemy.transform.position.y;
        targetY = startY + 5f;

        elapsedTime = 0f;
        isAscending = true;
    }

    public override void Update(NormalMonster enemy)
    {
        if (isAscending)
        {
            // ��� �ܰ�
            elapsedTime += Time.deltaTime;
            if (elapsedTime < upDuration)
            {
                float newY = Mathf.Lerp(startY, targetY, elapsedTime / upDuration);
                enemy.transform.position = new Vector3(
                    enemy.transform.position.x,
                    newY,
                    enemy.transform.position.z
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
            enemy.transform.position = new Vector3(
                enemy.transform.position.x,
                newY,
                enemy.transform.position.z
            );

            // ���鿡 ��Ҵ��� üũ
            if (enemy.isGround)
            {
                enemy.nMHandler.ChangeState(typeof(NormalMonsterIdle));
            }
        }
    }

    public override void Exit(NormalMonster enemy)
    {
        enemy.isAirborne = false;
    }
}

