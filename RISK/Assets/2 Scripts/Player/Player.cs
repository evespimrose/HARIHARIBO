using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Player : MonoBehaviourPun, ITakedamage, IPunObservable
{
    [SerializeField] private bl_Joystick joystick;
    [SerializeField] private Animator animator;

    private Vector3 networkPosition;
    private Quaternion networkRotation;

    private StateHandler<Player> stateHandler;
    private bool isMobile;
    private bool isSkillInProgress = false;
    private PlayerStats stats;

    public Animator Animator => animator;
    public PlayerStats Stats => stats;
    public int ComboCount { get; set; } = 0;
    protected abstract void InitializeStats();
    protected abstract void InitializeStateHandler();
    protected abstract void HandleSkillInput();

    protected virtual void Awake()
    {
        InitializeComponents();
        InitializeStateHandler();
        SetPlatform();
        InitializeStats();
    }

        private void InitializeStats()
    {
        if (stats == null)
        {
            stats = new PlayerStats();
        }
    }
    public void InitializeStats(PlayerStats stats)
    {
        this.stats = stats;
    }
    private void InitializeComponents()
    {
        animator = GetComponent<Animator>();

        tag = photonView.IsMine ? "LocalPlayer" : "RemotePlayer";

        if (photonView.IsMine)
        {
            if (GameObject.Find("Outline").TryGetComponent(out bl_Joystick joystick))
            {
                InitializeJoystick(joystick);
            }
        }
        else
        {
            networkPosition = transform.position;
            networkRotation = transform.rotation;
        }
    }

    protected virtual void SetPlatform()
    {
#if UNITY_ANDROID || UNITY_IOS
        isMobile = true;
#else
        isMobile = false;
#endif
    }

    private void Start()
    {
        //if (photonView.IsMine)
        //    UnitManager.Instance.RegisterPlayer(gameObject);
        //else
        //    UnitManager.Instance.RegisterPlayer(gameObject);
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            stats.UpadateHealthRegen(Time.deltaTime);

            if (ChatScrollController.Instance.isFocused()) return;

            if (isSkillInProgress)
                HandleInput();

            stateHandler.Update();
        }
        else
        {

        }
    }

    private void HandleInput()
    {


        if (Input.GetKeyDown(KeyCode.A))
        {
            stateHandler.ChangeState(typeof(PlayerAttackState));
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(WSkillState));
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(ESkillState));
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(RSkillState));
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {

        }
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
        if (photonView.IsMine)
        {
            transform.position += direction * stats.moveSpeed * Time.deltaTime;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * 10f);
        }
    }

    public virtual void Takedamage(float damage)
    {
        float finalDamage = stats.CalculateDamage(damage);
        stats.currentHealth -= Mathf.RoundToInt(finalDamage);
    }

    public void InitializeJoystick(bl_Joystick joystick)
    {
        this.joystick = joystick;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(stats.currentHealth);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            stats.currentHealth = (float)stream.ReceiveNext();
        }
    }

    [PunRPC]
    public void SyncStateChange(string stateName, PhotonMessageInfo info)
    {
        Type stateType = Type.GetType(stateName);
        if (stateType != null)
        {
            stateHandler.ChangeState(stateType);
        }
    }
}
