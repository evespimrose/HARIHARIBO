using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public abstract class Player : MonoBehaviour, ITakedamage
{
    [SerializeField] private bl_Joystick joystick;
    [SerializeField] private Animator animator;
    protected PhotonView photonView;

    protected StateHandler<Player> stateHandler;
    protected bool isMobile;
    protected bool isSkillInProgress = false;
    protected Playerstats stats;

    public Animator Animator => animator;
    public Playerstats Stats => stats;
    public int ComboCount { get; set; } = 0;
    protected abstract void InitializeStats();
    protected abstract void InitializeStateHandler();
    protected abstract void HandleSkillInput();

    protected virtual void Awake()
    {
        //photonView = GetComponent<PhotonView>();
        InitializeComponents();
        InitializeStateHandler();
        SetPlatform();
        InitializeStats();
    }
    protected virtual void Update()
    {
        stats.UpadateHealthRegen(Time.deltaTime);
        if (isSkillInProgress)
        {
            stateHandler.Update();
            return;
        }
        HandleSkillInput();
        stateHandler.Update();
    }
    private void InitializeComponents()
    {
        animator = GetComponent<Animator>();      
    }

    protected virtual void SetPlatform()
    {
#if UNITY_ANDROID || UNITY_IOS
            isMobile = true;
#else
        isMobile = false;
#endif
    }

    public void SetSkillInProgress(bool inProgress)
    {
        isSkillInProgress = inProgress;
    }

    public Vector3 GetMove()
    {
        if (isMobile)
        {
            if (joystick == null) return Vector3.zero;
            return new Vector3(joystick.Horizontal, 0f, joystick.Vertical);
        }
        else
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            return new Vector3(horizontal, 0f, vertical).normalized;
        }
    }

    public void Move(Vector3 direction)
    {
        transform.position += direction * stats.moveSpeed * Time.deltaTime;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    public virtual void Takedamage(float damage)
    {
        float finalDamage = stats.CalculateDamage(damage);
        stats.currentHealth -= Mathf.RoundToInt(finalDamage);
    }
}

