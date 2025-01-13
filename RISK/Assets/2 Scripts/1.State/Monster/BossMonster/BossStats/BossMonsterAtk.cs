using UnityEngine;

public class BossMonsterAtk : BaseState<BossMonster>
{
    public BossMonsterAtk(StateHandler<BossMonster> handler) : base(handler) { }

    public enum CurAction
    {
        Start,
        AtkA,
        AtkB,
        AtkC,
        End
    }
    private CurAction curAction = CurAction.Start;
    public float atkRange = 20f;
    //���� ���� ������
    public float atkDelayA = 0.17f;
    public float atkDelayB = 0.43f;
    public float atkDelayC = 0.18f;
    //�ִϸ��̼� ���� �ð�
    public float atkADuration = 1.4f;
    public float atkBDuration = 1.6f;
    public float atkCDuration = 1.8f;
    //�ִϸ��̼� ��ȯ ������ ������ �ð�
    public float nextBTime = 0.45f;//AtkA -> AtkB�� �Ѿ�� �ð�
    public float nextCTime = 1f;//AtkB -> AtkC�� �Ѿ�� �ð�
    public float nextEndTime = 1.3f;//AtkC -> End�� �Ѿ�� �ð�

    //������Ʈ ���� �� ��������
    public float startDelay = 0.5f;

    private float phaseStartTime;
    private float startTime = 0f;

    private bool isAtkAHit = false;
    private bool isAtkBHit = false;
    private bool isAtkCHit = false;

    public override void Enter(BossMonster monster)
    {
        Debug.Log("���� ����");
        curAction = CurAction.AtkA;
        phaseStartTime = Time.time + startDelay;//�������� �����Ͽ� ���� ���� �ð� ���
        startTime = 0f;//�ִϸ��̼� ���� ���� �ð� �ʱ�ȭ
        isAtkAHit = false;
        isAtkBHit = false;
        isAtkCHit = false;
        AttackAnimationPlay(monster, 1);
    }

    public override void Update(BossMonster monster)
    {
        startTime = Time.time - phaseStartTime;//��� �ð� ���

        if (curAction == CurAction.End)
        {
            //End ���¿����� ������ ����� �� �ٷ� Idle ���·� ��ȯ
            monster.bMHandler.ChangeState(typeof(BossMonsterIdle));
            return;//End ���¿��� �ٷ� ��ȯ�ϹǷ� �� �̻� ������ �ʿ� ����
        }
        switch (curAction)
        {
            case CurAction.AtkA:
                //AtkA ���� ���� ������
                if (!isAtkAHit && startTime >= atkDelayA)
                {
                    AttackHit(monster, 105f);//AtkA ���� ����
                    isAtkAHit = true;//���� ������ �� ���� ����ǵ��� ��
                }
                //AtkA �ִϸ��̼��� nextBTime ������ �����ϸ� AtkB�� ��ȯ
                if (startTime >= nextBTime && startTime < atkADuration)
                {
                    ChangeAction(monster, CurAction.AtkB);
                }
                break;
            case CurAction.AtkB:
                //AtkB �ִϸ��̼� ���� �� atkDelayB ��ŭ ��ٸ� �� ���� ���� ����
                if (!isAtkBHit && startTime >= atkDelayB)
                {
                    AttackHit(monster, 45f);//AtkB ���� ����
                    isAtkBHit = true;//���� ������ �� ���� ����ǵ��� ��
                }
                //AtkB �ִϸ��̼��� nextCTime ������ �����ϸ� AtkC�� ��ȯ
                if (startTime >= nextCTime && startTime < atkBDuration)
                {
                    ChangeAction(monster, CurAction.AtkC);
                }
                break;
            case CurAction.AtkC:
                //AtkC �ִϸ��̼� ���� �� atkDelayC ��ŭ ��ٸ� �� ���� ���� ����
                if (!isAtkCHit && startTime >= atkDelayC)
                {
                    AttackHit(monster, 180f);//AtkC ���� ����
                    isAtkCHit = true;//���� ������ �� ���� ����ǵ��� ��
                }
                //AtkC �ִϸ��̼��� nextEndTime ������ �����ϸ� End�� ��ȯ
                if (startTime >= nextEndTime && startTime < atkCDuration)
                {
                    ChangeAction(monster, CurAction.End);
                }
                break;
        }
    }

    public override void Exit(BossMonster monster)
    {
        Debug.Log("���� ����");
    }

    private void AttackAnimationPlay(BossMonster monster, int animationNumber)
    {
        //���� ���� ���� �ִϸ��̼��� �������� Ȯ���ϰ� ��ġ�� �ʰ� ����
        if (monster.animator.GetCurrentAnimatorStateInfo(0).IsName("AtkA") && animationNumber == 1 ||
            monster.animator.GetCurrentAnimatorStateInfo(0).IsName("AtkB") && animationNumber == 2 ||
            monster.animator.GetCurrentAnimatorStateInfo(0).IsName("AtkC") && animationNumber == 3)
        {
            return;//�ִϸ��̼��� ������̸� ����
        }
        switch (animationNumber)
        {
            case 1:
                monster.animator.SetTrigger("AtkA");
                break;
            case 2:
                monster.animator.SetTrigger("AtkB");
                break;
            case 3:
                monster.animator.SetTrigger("AtkC");
                break;
        }
        Debug.Log($"{curAction} �ִϸ��̼� ���");
    }

    private void AttackHit(BossMonster monster, float angleThreshold)
    {
        Vector3 atkDir = monster.transform.forward;
        Collider[] cols = Physics.OverlapSphere(monster.transform.position, atkRange);
        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                Vector3 dirToTarget = (col.transform.position - monster.transform.position).normalized;
                float angle = Vector3.Angle(atkDir, dirToTarget);

                if (angle <= angleThreshold)
                {
                    col.gameObject.GetComponent<ITakedamage>()?.Takedamage(monster.atkDamage);
                    Debug.Log($"{curAction} ���� ����");
                }
                else
                {
                    Debug.Log($"{curAction} ���� ���� - ���� ��");
                }
            }
        }
    }

    private void ChangeAction(BossMonster monster, CurAction nextPhase)
    {
        curAction = nextPhase;
        if (nextPhase != CurAction.End)
        {
            phaseStartTime = Time.time;//�ð� �ʱ�ȭ
            //�ִϸ��̼��� �����ϱ� ���� �̹� Ʈ���Ű� �����Ƿ�, �ٷ� ���� �ִϸ��̼��� ����
            switch (nextPhase)
            {
                case CurAction.AtkB:
                    AttackAnimationPlay(monster, 2);
                    break;
                case CurAction.AtkC:
                    AttackAnimationPlay(monster, 3);
                    break;
            }
        }
    }
}
