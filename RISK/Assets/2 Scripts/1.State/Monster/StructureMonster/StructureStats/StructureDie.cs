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
        monster.monsterDebuff.DebuffAllOff();
        startPosition = monster.model.transform.position;
        targetPosition = new Vector3(startPosition.x, targetHeight, startPosition.z);
        Debug.Log("Die 시작");
        monster.model.transform.position = new Vector3(monster.model.transform.position.x, 0.1f, monster.model.transform.position.z);
        monster.StartCoroutine(DestroyGameObject(monster, 1f));
    }

    public override void Update(StructureMonster monster)
    {
        if (!isMoving)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp(elapsedTime / moveDuration, 0f, 1f);
            monster.model.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            if (elapsedTime >= moveDuration)
            {
                isMoving = false;
                monster.model.transform.position = targetPosition;
            }
        }
    }

    public override void Exit(StructureMonster monster)
    {
        Debug.Log("Die 종료");
    }

    private IEnumerator DestroyGameObject(StructureMonster monster, float dieTime)
    {
        yield return new WaitForSeconds(dieTime);
        monster.Die();
    }
}
