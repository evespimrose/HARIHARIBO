using ExitGames.Client.Photon;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class Player : MonoBehaviourPun, ITakedamage, IPunObservable
{
    [SerializeField] private bl_Joystick joystick;
    [SerializeField] private Animator animator;

    private Vector3 networkPosition;
    private Quaternion networkRotation;

    protected StateHandler<Player> stateHandler;
    protected bool isMobile;
    protected bool isSkillInProgress = false;
    protected PlayerStats stats;

    public Animator Animator => animator;
    public PlayerStats Stats { get; protected set; }
    public int ComboCount { get; set; } = 0;
    protected abstract void InitializeStats();
    protected abstract void InitializeStateHandler();
    protected abstract void HandleSkillInput();
    public int TeamID { get; private set; } = 1;

    protected virtual void Awake()
    {
        InitializeComponents();
        InitializeStateHandler();
        SetPlatform();
        InitializeStats();
        PhotonPeer.RegisterType(typeof(Destroyer), 100, SerializeDestroyer, DeserializeDestroyer);
        PhotonPeer.RegisterType(typeof(Healer), 101, SerializeHealer, DeserializeHealer);
        PhotonPeer.RegisterType(typeof(Mage), 102, SerializeMage, DeserializeMage);
        PhotonPeer.RegisterType(typeof(Warrior), 103, SerializeWarrior, DeserializeWarrior);


        if (photonView.IsMine)
        {
            var dungeonUI = FindObjectOfType<DungeonUIController>();
            if (dungeonUI != null)
            {
                dungeonUI.SetPlayer(this);
            }
        }
        //if (photonView.IsMine)
        //{
        //    UnitManager.Instance.RegisterPlayer(gameObject);
        //    UnitManager.Instance.photonView.RPC("SyncPlayer", RpcTarget.MasterClient);
        //}
    }

    private static byte[] SerializeDestroyer(object customType)
    {
        string json = JsonUtility.ToJson(customType);
        return System.Text.Encoding.UTF8.GetBytes(json);
    }
    private static byte[] SerializeHealer(object customType)
    {
        string json = JsonUtility.ToJson(customType);
        return System.Text.Encoding.UTF8.GetBytes(json);
    }

    private static byte[] SerializeMage(object customType)
    {
        string json = JsonUtility.ToJson(customType);
        return System.Text.Encoding.UTF8.GetBytes(json);
    }

    private static byte[] SerializeWarrior(object customType)
    {
        string json = JsonUtility.ToJson(customType);
        return System.Text.Encoding.UTF8.GetBytes(json);
    }

    private static object DeserializeDestroyer(byte[] data)
    {
        string json = System.Text.Encoding.UTF8.GetString(data);
        return JsonUtility.FromJson<Destroyer>(json);
    }

    private static object DeserializeWarrior(byte[] data)
    {
        string json = System.Text.Encoding.UTF8.GetString(data);
        return JsonUtility.FromJson<Warrior>(json);
    }

    private static object DeserializeHealer(byte[] data)
    {
        string json = System.Text.Encoding.UTF8.GetString(data);
        return JsonUtility.FromJson<Healer>(json);
    }

    private static object DeserializeMage(byte[] data)
    {
        string json = System.Text.Encoding.UTF8.GetString(data);
        return JsonUtility.FromJson<Mage>(json);
    }



    //public void InitializeStats(PlayerStats stats)
    //{
    //    this.stats = stats;
    //}

    // ?怨밴묶 癰궰野껋럩???袁る립 public 筌롫뗄苑???곕떽?
    public void MobileChangeState(Type stateType)
    {
        if (photonView.IsMine)
        {
            stateHandler.ChangeState(stateType);
        }
    }
    private void InitializeComponents()
    {
        animator = GetComponent<Animator>();

        tag = photonView.IsMine ? "LocalPlayer" : "RemotePlayer";

        if (photonView.IsMine)
        {
#if UNITY_ANDROID
        // 筌뤴뫀而??깅퓠??뺤춸 鈺곌퀣???쎈뼓 ?λ뜃由??
        if (GameObject.Find("Outline").TryGetComponent(out bl_Joystick joystick))
        {
            InitializeJoystick(joystick);
        }
#endif
        }
        else
        {
            networkPosition = transform.position;
            networkRotation = transform.rotation;
        }
    }
    public void InitializeStatsPhoton(PlayerStats stat) { stats = stat; stats.InitializeStats(); }
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
        if (photonView.IsMine) return;
        object[] instantiationData = photonView.InstantiationData;
        if (instantiationData != null && instantiationData.Length > 0)
        {
            string nickName = (string)instantiationData[0];
            gameObject.name = nickName;
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            stats.UpadateHealthRegen(Time.deltaTime);

            stateHandler.Update();
        }

    }

    public void SetSkillInProgress(bool inProgress)
    {
        isSkillInProgress = inProgress;
    }

    public Vector3 GetMove()
    {
#if UNITY_ANDROID
        if (joystick == null) return Vector3.zero;
        return new Vector3(joystick.Horizontal, 0f, joystick.Vertical);
#else
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        return new Vector3(horizontal, 0f, vertical).normalized;
#endif
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

    public void SetTeamID(int teamId)
    {
        TeamID = teamId;
    }

    public void ApplyAttackBuff(float amount, float duration)
    {
        StartCoroutine(ApplyAttackBuffCoroutine(amount, duration));
    }

    private IEnumerator ApplyAttackBuffCoroutine(float amount, float duration)
    {
        // ?⑤벀爰??甕곌쑵遊??怨몄뒠
        stats.attackPower += amount;

        yield return new WaitForSeconds(duration);

        // 甕곌쑵遊???곸젫
        stats.attackPower -= amount;
    }

    public void Heal(float amount)
    {
        stats.currentHealth = Mathf.Min(stats.currentHealth + amount, stats.maxHealth);
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

    [PunRPC]
    private void RequestPlayerSync(PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        foreach (var playerPair in UnitManager.Instance.players)
        {
            if (playerPair.Value != null && playerPair.Value.TryGetComponent(out PhotonView photonView))
            {
                photonView.RPC("SyncPlayer", info.Sender,
                    photonView.ViewID,
                    photonView.Owner.ActorNumber,
                    playerPair.Value.name);
            }
        }
    }

    [PunRPC]
    private void SyncPlayer(int viewId, int actorNumber, string playerName)
    {
        PhotonView targetView = PhotonView.Find(viewId);
        if (targetView != null)
        {
            GameObject playerObj = targetView.gameObject;
            playerObj.name = playerName;

            if (!UnitManager.Instance.HasPlayer(actorNumber))
            {
                UnitManager.Instance.RegisterPlayer(playerObj);
            }
        }
    }

    [PunRPC]
    private void NotifyPlayerRegistered(int actorNumber)
    {
        print("NotifyPlayerRegistered");
        Debug.Log($"Player {actorNumber} has been registered to all clients");
    }
}
