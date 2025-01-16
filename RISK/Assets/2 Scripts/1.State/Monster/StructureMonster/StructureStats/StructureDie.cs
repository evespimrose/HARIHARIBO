using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StructureDie : BaseState<StructureMonster>
{
    public StructureDie(StateHandler<StructureMonster> handler) : base(handler) { }

    public float moveDuration = 0.5f;
    private float elapsedTime = 0f;
    public float targetHeight = 0.1f;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool isMoving = false;

    public override void Enter(StructureMonster monster)
    {
        startPosition = monster.model.transform.position;
        targetPosition = new Vector3(startPosition.x, targetHeight, startPosition.z);
        Debug.Log("Die 시작");
        monster.model.transform.position = new Vector3(monster.model.transform.position.x, 0.1f, monster.model.transform.position.z);
        monster.Invoke("Die", 1f);
    }

    public override void Update(StructureMonster monster)
    {
        if (!isMoving)
        {
            // 경과 시간 업데이트
            elapsedTime += Time.deltaTime;

            // 비율을 계산하여 위치 보간 (Lerp)
            float t = Mathf.Clamp(elapsedTime / moveDuration, 0f, 1f);
            monster.model.transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            // 목표 위치에 도달했으면 이동 종료
            if (elapsedTime >= moveDuration)
            {
                isMoving = false;  // 이동 종료
                monster.model.transform.position = targetPosition;  // 정확히 목표 위치로 설정
            }
        }
    }

    public override void Exit(StructureMonster monster)
    {
        Debug.Log("Die 종료");
    }
}
