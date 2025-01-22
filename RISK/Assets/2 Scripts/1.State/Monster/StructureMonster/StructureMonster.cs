using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using UnityEngine;

public class StructureMonster : Monster
{
    public StateHandler<StructureMonster> sMHandler;

    protected Collider col;

    private Coroutine hit = null;

    private void Awake()
    {
        InitializeComponents();
        InitializeStateHandler();
    }

    void Start()
    {
        UnitManager.Instance.monsters.Add(this.gameObject);
        Targeting();
    }

    void Update()
    {
        RemoveBodyAtkHit();
        monsterDebuff.DebuffCheck(this);
        if (target == null) Targeting();
        if (isHit == false) hit = null;
        if (isAirborne == true && !isAirborneAction)
        {
            sMHandler.ChangeState(typeof(StructureAirborne));
        }
        if (isAirborne == false && isStun == true && !isStunAction)
        {
            sMHandler.ChangeState(typeof(StructureStun));
        }
        float newYRotation = model.transform.eulerAngles.y - 90f * Time.deltaTime;
        model.transform.rotation = Quaternion.Euler(model.transform.eulerAngles.x, newYRotation, model.transform.eulerAngles.z);
        sMHandler.Update();
    }

    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<SphereCollider>();
        this.bodyAtkDamage = monsterState.atkDamage;
        this.bodyAtkCoolTime = monsterState.atkDelay;
        this.moveSpeed = monsterState.moveSpeed;
        this.curHp = monsterState.curHp;
        this.maxHp = curHp;
        this.won = monsterState.won;
        this.exp = monsterState.exp;
    }

    protected void InitializeStateHandler()
    {
        sMHandler = new StateHandler<StructureMonster>(this);

        sMHandler.RegisterState(new StructureIdle(sMHandler));
        sMHandler.RegisterState(new StructureAirborne(sMHandler));
        sMHandler.RegisterState(new StructureStun(sMHandler));
        sMHandler.RegisterState(new StructureDie(sMHandler));
        sMHandler.RegisterState(new StructureMove(sMHandler));

        sMHandler.ChangeState(typeof(StructureIdle));
    }

    public override void Takedamage(float damage)
    {
        base.Takedamage(damage);
        if (hit != null)
        {
            StopCoroutine(hit);
        }
        hit = StartCoroutine(Hit());
    }

    public override void DieStatChange()
    {
        this.sMHandler.ChangeState(typeof(StructureDie));
    }

    private IEnumerator Hit()
    {
        isHit = true;
        yield return new WaitForSeconds(0.2f);
        isHit = false;
    }

    public void Die()
    {
        if (isDie == true)
        {
            UnitManager.Instance.monsters.Remove(this.gameObject);
            Destroy(this.gameObject);
        }
    }
}
