using Photon.Pun.Demo.Cockpit;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Player : MonoBehaviour, ITakedamage
{
    [SerializeField] private bl_Joystick joystick;
    [SerializeField] private Animator animator;

    private StateHandler<Player> stateHandler;
    private bool isMobile;
    private float moveSpeed = 2f;

    public Animator Animator => animator;
    //public Playerstats Stats => stats;
    public int ComboCount { get; set; } = 0;


    private void Awake()
    {
        InitializeComponents();
        InitializeStateHandler();
        SetPlatform();
        UnitManager.Instance.players.Add(this.gameObject);
    }

    private void InitializeComponents()
    {
        animator = GetComponent<Animator>();      
    }

    private void InitializeStateHandler()
    {
        stateHandler = new StateHandler<Player>(this);

        stateHandler.RegisterState(new PlayerIdleState(stateHandler));
        stateHandler.RegisterState(new PlayerMoveState(stateHandler));
        stateHandler.RegisterState(new PlayerAttackState(stateHandler));

        stateHandler.ChangeState(typeof(PlayerIdleState));
    }

    private void SetPlatform()
    {
#if UNITY_ANDROID || UNITY_IOS
            isMobile = true;
#else
        isMobile = false;
#endif
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            stateHandler.ChangeState(typeof(PlayerAttackState));
        }
        stateHandler.Update();
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
        transform.position += direction * moveSpeed * Time.deltaTime;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    public void Takedamage(float damage)
    {
        print("¾Æ¾ß");
    }
}

