using Photon.Pun.Demo.Cockpit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, ITakedamage
{
    public enum PlayerState
    {
        Idle,
        Moving,
        Skill,
        Dead,
    }

    [Tooltip("�÷��̾� ����")]
    public PlayerScroptableObjects playerStats;
    [Tooltip("���� ������")]
    public float atkDamage;
    [Tooltip("���� �ӵ�")]
    public float atkSpeed;
    [Tooltip("�̵� �ӵ�")]
    public float moveSpeed;
    [Tooltip("ġ��Ÿ Ȯ��")]
    public float criticalChance;
    [Tooltip("ġ��Ÿ ������")]
    public float criticalDamage;
    [Tooltip("��ų ��Ÿ�� ������")]
    public float cooldownReduction;
    [Tooltip("���� ü��")]
    public float curHp;
    [SerializeField]
    private bl_Joystick joystick;
    [SerializeField] private WeaponController weaponController;


    private PlayerState currentState;
    private bool isMobile;
    private float maxHp;
    private Rigidbody rb;
    private Animator animator;

    
    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        currentState = PlayerState.Idle;
        atkDamage = playerStats.atkDamage;
        atkSpeed = playerStats.atkSpeed;
        moveSpeed = playerStats.moveSpeed;
        criticalChance = playerStats.criticalChance;
        criticalDamage = playerStats.criticalDamage;
        cooldownReduction = playerStats.cooldownReduction;
        curHp = playerStats.curHp;
        maxHp = curHp;
        #if UNITY_ANDROID || UNITY_IOS
            isMobile = true;
        #else
            isMobile = false;
        #endif
    }

    private void Start()
    {
        if (weaponController == null)
        {
            weaponController = GetComponent<WeaponController>();
        }
        UnitManager.Instance.players.Add(this.gameObject);
    }

   
    private void Update()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
            case PlayerState.Moving:
                Move();  
                break;
            case PlayerState.Skill:
                break;
            case PlayerState.Dead:
                break;
        }
    }

    public void Move()
    {
        Vector3 moveDirection;

        if (isMobile)
        {
            if (joystick == null)
            {
                Debug.LogError("Joystick reference is missing!");
                return;
            }
            moveDirection = new Vector3(joystick.Horizontal, 0f, joystick.Vertical);
        }
        else
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
        }

        if (moveDirection.magnitude > 0)
        {
            currentState = PlayerState.Moving;
            if (animator != null)
            {
                animator.SetBool("IsMoving", true);
            }

            Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;
            rb.MovePosition(transform.position + movement);

            if (moveDirection != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 720f * Time.deltaTime);

            }
        }
        else
        {
            currentState = PlayerState.Idle;
            if (animator != null)
            {
                animator.SetBool("IsMoving", false);
            }
        }

    }

    public void Takedamage(float damage)
    {
        curHp -= damage;
    }
}
