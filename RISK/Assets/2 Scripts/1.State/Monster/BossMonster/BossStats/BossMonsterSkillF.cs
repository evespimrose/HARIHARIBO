using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSkillF : BaseState<BossMonster>
{
    public BossMonsterSkillF(StateHandler<BossMonster> handler) : base(handler) { }

    // ��ų6 �뽬 �а�
    public Vector3 dashDirection; // �뽬 ����
    public float atkTime = 1f; // �뽬 ���� �ð�
    public float skillFMoveSpeed = 20f; // �뽬 �ӵ�
    public float dashDistance = 5f; // �뽬 �̵� �Ÿ�
    public float slowSpeed = 0.1f; // ���� �浹�� �� �������� �ӵ� ����

    public override void Enter(BossMonster monster)
    {
        monster.StartSkillCoroutine(SkillFCoroutine(monster)); // �ڷ�ƾ ����
        monster.Targeting(); // Ÿ�� ���� ����

        // �뽬 ���� ������ ���� (���� �ٶ󺸴� ����)
        dashDirection = monster.transform.forward;

        // �ʱ� ȸ�� ���� (�뽬 �������� ȸ��)
        monster.TargetLook(monster.transform.position + dashDirection);

        monster.SkillFReset(); // ��ų �ʱ�ȭ
        Debug.Log("SkillF ����");
    }

    public override void Exit(BossMonster monster)
    {
        monster.AtkEnd();
        Debug.Log("SkillF ����");
        monster.SkillFReset();
        monster.animator.SetBool("SkillF", false); // �ִϸ��̼� ����
    }

    private IEnumerator SkillFCoroutine(BossMonster monster)
    {
        // �ִϸ��̼� ���� (�뽬 ����)
        monster.animator.SetBool("SkillF", true);

        // �뽬 ����
        monster.isMoving = true;
        Debug.Log("���� ����");

        float elapsedTime = 0f; // ��� �ð�

        // �뽬 ���� �ð� ���� �̵�
        while (elapsedTime < atkTime)
        {
            if (monster.isMoving)
            {
                // �뽬 �������� ��� �̵�
                Vector3 targetPosition = monster.transform.position + dashDirection * skillFMoveSpeed * Time.deltaTime;

                // ���� �浹�ߴ��� üũ
                if (monster.isWall)
                {
                    Debug.Log("���� �浹�Ͽ� �ӵ� ����");
                    // ���� �浹�ϸ� �ӵ��� ���߱�: ���� ��������� �̵� �ӵ��� �� ���߱�
                    targetPosition = monster.transform.position + dashDirection * skillFMoveSpeed * slowSpeed * Time.deltaTime;
                }

                targetPosition.y = monster.transform.position.y;  // y�� ����
                monster.transform.position = targetPosition;
            }

            elapsedTime += Time.deltaTime;
            yield return null; // �� ������ ���
        }

        // �뽬 ����
        monster.isMoving = false;
        monster.SkillFReset();
        Debug.Log("���� ����");

        // �뽬 ���� �� �ִϸ��̼��� ���� 0.2�� �� Idle�� ��ȯ
        yield return new WaitForSeconds(0.2f);
        monster.animator.SetBool("SkillF", false);

        // ���� ��ȯ
        monster.bMHandler.ChangeState(typeof(BossMonsterIdle)); // ���� ��ȯ
    }
}
