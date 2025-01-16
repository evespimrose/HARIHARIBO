using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSkillG : BaseState<BossMonster>
{
    public BossMonsterSkillG(StateHandler<BossMonster> handler) : base(handler) { }

    // ��ų7 �뽬 + ����
    public float atkDelay = 2.24f; // �ڷ���Ʈ �� ��� �ð�
    private float endTime = 0.5f; // �ڷ���Ʈ �� ��� �ð�
    private bool isAction = false; // �ڷ���Ʈ ���θ� ����

    public override void Enter(BossMonster monster)
    {
        Debug.Log("SkillG ����");
        monster.StartSkillCoroutine(SkillGCoroutine(monster)); // �ڷ�ƾ ����
    }

    public override void Exit(BossMonster monster)
    {
        Debug.Log("SkillG ����");
    }

    private IEnumerator SkillGCoroutine(BossMonster monster)
    {
        monster.skillGPrefab.SetActive(true);
        monster.animator.SetTrigger("SkillG");
        monster.TargetLook(monster.target.position);
        // ù ��° ��� �ð� �� �ڷ���Ʈ
        yield return new WaitForSeconds(atkDelay);
        monster.skillGPrefab.SetActive(false);
        // �ڷ���Ʈ ����
        SkillGAtk(monster);
        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo stateInfo = monster.animator.GetCurrentAnimatorStateInfo(0);
            return !stateInfo.IsName("SkillG") || stateInfo.normalizedTime >= 1f;
        });
        yield return null;

        // ��ų�� ���� �� ���� ���·� ��ȯ
        monster.bMHandler.ChangeState(typeof(BossMonsterSkillB)); // ���� ��ȯ
    }

    public void SkillGAtk(BossMonster monster)
    {
        // Ÿ�� ���� ���� ���
        Vector3 targetDir = (monster.target.position - monster.transform.position).normalized;

        // Ÿ���� ���� ��ġ ���
        Vector3 teleportPos = monster.target.position - targetDir * 2f; // 2f�� �Ÿ� (�ʿ�� ���� ����)

        // Y���� ���� ���� ���̸� ����
        teleportPos.y = monster.transform.position.y;

        // ���� ��ġ ����
        monster.transform.position = teleportPos;

        Debug.Log($"���Ͱ� Ÿ�� �ڷ� �̵�: {teleportPos}");
    }
}
