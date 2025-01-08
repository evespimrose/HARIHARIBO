using Photon.Pun.Demo.Cockpit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private bl_Joystick joystick;
    [SerializeField] private WeaponController weaponController;
    [SerializeField] private Animator animator;

    private StateHandler<Player> stateHandler;
    private bool isMobile;
    private float moveSpeed = 5f;

    public Animator Animator => animator;
    public WeaponController WeaponController => weaponController;

    private void Awake()
    {
        InitializeComponents();
        InitializeStateHandler();
        SetPlatform();
    }

    private void InitializeComponents()
    {
        animator = GetComponent<Animator>();
        if (weaponController == null)
            weaponController = GetComponent<WeaponController>();
    }

    private void InitializeStateHandler()
    {
        stateHandler = new StateHandler<Player>(this);

        // 상태들 등록
        stateHandler.RegisterState(new PlayerIdleState(stateHandler));
        stateHandler.RegisterState(new PlayerMoveState(stateHandler));
        //stateHandler.RegisterState(new PlayerAttackState(stateHandler));

        // 초기 상태 설정
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
}

