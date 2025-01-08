using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeMonster : Monster
{
    public Transform shotPos;
    public GameObject bulletPrefab;

    public enum Stats
    {
        Move,
        Atk,
        Hitsturn,
        Airborne,
        Die
    }

    private Stats stats;

    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        //print($"{stats}");
        if (isAction == true) return;
        if (target == null) Targeting();
        StatsChange();
        switch (stats)
        {
            case Stats.Move:
                break;
            case Stats.Atk:
                rb.velocity = Vector3.zero;
                action = StartCoroutine(Attack());
                break;
            case Stats.Hitsturn:
                rb.velocity = Vector3.zero;
                if (action != null)
                {
                    StopCoroutine(action);
                }
                action = StartCoroutine(Hit());
                break;
            case Stats.Airborne:
                rb.velocity = Vector3.zero;
                if (action != null)
                {
                    StopCoroutine(action);
                }
                action = StartCoroutine(Airborne());
                break;
            case Stats.Die:
                rb.velocity = Vector3.zero;
                animator.SetBool("Die", true);
                break;
        }
    }

    private void FixedUpdate()
    {
        if (stats == Stats.Move)
        {
            Move();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
        }
    }

    public void StatsChange()
    {
        if (isDie == true)
        {
            StopAllCoroutines();
            stats = Stats.Die;
        }
        else
        {
            if (isAirborne == true)
            {
                stats = Stats.Airborne;
            }
            else if (isHit == true && isAirborne == false)
            {
                stats = Stats.Hitsturn;
            }
            else
            {
                if (Vector3.Distance(target.position, transform.position) < atkRange)
                {
                    stats = Stats.Atk;
                }
                else
                {
                    stats = Stats.Move;
                }
            }
        }
    }

    public IEnumerator Attack()
    {
        isAction = true;
        transform.LookAt(target);
        yield return new WaitForSeconds(0.2f);
        //공격범위바꿀거면 atkRange만 바꾸면됨
        GameObject bullet = Instantiate(bulletPrefab, shotPos.position, bulletPrefab.transform.rotation);
        bullet.GetComponent<RangeSpear>().BulletSeting(this.target, atkDamage);
        yield return new WaitForSeconds(0.2f);
        isAction = false;
    }

    public IEnumerator Hit()
    {
        isAction = true;
        //다른행동 중지하고 애니메이션 변경하고  피격모션
        yield return new WaitForSeconds(0.2f);//경직시간
        isHit = false;
        isAction = false;
    }

    public IEnumerator Airborne()
    {
        isAction = true;
        isGround = false;
        float airborneTime = 1f;
        float upDuration = airborneTime * 0.4f;
        float downDuration = airborneTime * 0.6f;

        float startY = model.transform.position.y;
        float targetY = startY + 5f;
        float elapsedTime = 0f;
        //다른행동 중지하고 애니메이션 변경하고  에어본모션

        //올라가는 부분
        while (elapsedTime < upDuration)
        {
            elapsedTime += Time.deltaTime;
            float newY = Mathf.Lerp(startY, targetY, elapsedTime / upDuration);
            model.transform.position = new Vector3(model.transform.position.x, newY, model.transform.position.z);
            yield return null;
        }
        model.transform.position = new Vector3(model.transform.position.x, targetY, model.transform.position.z);

        //내려오는 부분
        elapsedTime = 0f;
        float originalY = model.transform.position.y;

        while (elapsedTime < downDuration)
        {
            elapsedTime += Time.deltaTime;
            float newY = Mathf.Lerp(originalY, startY, elapsedTime / downDuration);
            model.transform.position = new Vector3(model.transform.position.x, newY, model.transform.position.z);
            yield return null;
        }
        model.transform.position = new Vector3(model.transform.position.x, startY, model.transform.position.z);

        yield return new WaitUntil(() => isGround == true);
        isAirborne = false;
        isAction = false;
    }

    public void Did()
    {
        //애니메이션실행 마지막부분에 실행
        UnitManager.Instance.monsters.Remove(this.gameObject);
        Destroy(this.gameObject);
    }
}
