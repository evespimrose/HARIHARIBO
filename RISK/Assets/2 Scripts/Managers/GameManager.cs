using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using HashTable = ExitGames.Client.Photon.Hashtable;
using PhotonRealtimePlayer = Photon.Realtime.Player;
using System;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunSingletonManager<GameManager>
{
    public bool isGameRunning;

    public bool isWaveDone = false;

    public bool isTickGoes = false;

    public bool isGamePaused = false;
    private bool isGameForceOver;
    public List<FireBaseCharacterData> connectedPlayers = new List<FireBaseCharacterData>();

    public Transform playerPosition;

    public MonsterSpwan spawner;

    public RiskUIController riskUIController;

    public ChatScrollController chat;

    private Canvas persistentCanvas;

    private DungeonUIController dungeonUIController;

    public float roomReward = 0f;
    private float rewardMagnification = 1.0f;

    public Action<float> WhenMonsterDies;

    [SerializeField]
    private List<ClassNameToCharacterData> classDataList;

    public Dictionary<ClassType, CharacterData> characterDataDic = new Dictionary<ClassType, CharacterData>();

    [Header("Timer")]
    [SerializeField]
    private float startTime = 300f;
    private float remainingTime;

    protected override void Awake()
    {
        base.Awake();

        foreach (var classdata in classDataList)
        {
            if (!characterDataDic.ContainsKey(classdata.classType))
            {
                characterDataDic.Add(classdata.classType, classdata.characterData);

            }
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
        CreatePersistentCanvas();
        WhenMonsterDies += MonsterReward;
    }

    public void MonsterReward(float won)
    {
        roomReward += won;
    }

    private IEnumerator GameClock()
    {
        isTickGoes = true;
        while (remainingTime > 0)
        {
            if (isTickGoes && !isGamePaused)
            {
                remainingTime -= Time.deltaTime;
            }

            yield return null;
        }

        remainingTime = 0;

        Debug.Log("Time's up!");
        OnTimeOut();
    }

    private void OnTimeOut()
    {
        //isGameRunning = false;
        Debug.Log("Game over! Timer reached 0.");
    }

    private void CreatePersistentCanvas()
    {
        GameObject canvasObject = new GameObject("PersistentCanvas");
        persistentCanvas = canvasObject.AddComponent<Canvas>();
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        persistentCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

        DontDestroyOnLoad(canvasObject);

        chat.gameObject.transform.SetParent(persistentCanvas.transform, false);
    }

    public void AttachToNewCanvas(Canvas newCanvas)
    {
        // ??Canvas??UI ?????鍮????곗뵯???????쇰뮚??
        if (chat != null && newCanvas != null)
        {
            chat.gameObject.transform.SetParent(newCanvas.transform, false);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Canvas newCanvas = FindObjectOfType<Canvas>();
        AttachToNewCanvas(newCanvas);
    }

    public IEnumerator CollectPlayerData(PhotonRealtimePlayer player)
    {
        yield return new WaitUntil(() => !string.IsNullOrEmpty(player.NickName));
        print($"{player.NickName}");

        FireBaseCharacterData playerData = JsonConvert.DeserializeObject<FireBaseCharacterData>(player.NickName);

        connectedPlayers.Add(playerData);

        player.NickName = playerData.nickName;

        SyncAllPlayers();
    }

    private void SyncAllPlayers()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            string jsonData = JsonConvert.SerializeObject(connectedPlayers);
            photonView.RPC("SyncPlayerDataRPC", RpcTarget.All, jsonData);
        }
    }

    [PunRPC]
    public void SyncPlayerDataRPC(string jsonData)
    {
        connectedPlayers = JsonConvert.DeserializeObject<List<FireBaseCharacterData>>(jsonData);

        Debug.Log($"Player data synchronized with all clients! : {FirebaseManager.Instance.currentCharacterData.nickName}");
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => isGameRunning);
        print("isGameReady!!! GOGOGOGO!!!!");
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "GameScene");
        spawner = (MonsterSpwan)FindAnyObjectByType(typeof(MonsterSpwan));
        riskUIController = (RiskUIController)FindAnyObjectByType(typeof(RiskUIController));
        dungeonUIController = (DungeonUIController)FindAnyObjectByType(typeof(DungeonUIController));
        riskUIController.gameObject.SetActive(false);

        FireBaseCharacterData fireBaseCharacterData = FirebaseManager.Instance.currentCharacterData;

        PlayerStats playerStats = new PlayerStats
        {
            nickName = fireBaseCharacterData.nickName,
            level = fireBaseCharacterData.level,
            maxExp = fireBaseCharacterData.maxExp,
            currentExp = fireBaseCharacterData.currExp,
            maxHealth = fireBaseCharacterData.maxHp,
            currentHealth = fireBaseCharacterData.maxHp,
            moveSpeed = fireBaseCharacterData.moveSpeed,
            attackPower = fireBaseCharacterData.atk,
            damageReduction = fireBaseCharacterData.dmgRed,
            healthRegen = fireBaseCharacterData.hpReg,
            regenInterval = fireBaseCharacterData.regInt,
            criticalChance = fireBaseCharacterData.cri,
            criticalDamage = fireBaseCharacterData.criDmg,
            cooldownReduction = fireBaseCharacterData.coolRed,
            healthPerLevel = fireBaseCharacterData.hPperLv,
            attackPerLevel = fireBaseCharacterData.atkperLv,
        };

        playerPosition = GameObject.Find("SpawnPosition").transform;
        int playerNumber = PhotonNetwork.LocalPlayer.ActorNumber;

        Vector3 playerPos = playerPosition.GetChild(playerNumber).position;


        GameObject playerObj = null;
        switch (FirebaseManager.Instance.currentCharacterData.classType)
        {
            case ClassType.Warrior:
                playerObj = PhotonNetwork.Instantiate("Warrior", playerPos, Quaternion.identity, 0, new object[] { playerStats.nickName });
                playerObj.name = fireBaseCharacterData.nickName;
                if (playerObj.TryGetComponent(out Warrior warrior))
                {
                    warrior.InitializeStatsPhoton(playerStats);
                }
                break;
            case ClassType.Destroyer:
                playerObj = PhotonNetwork.Instantiate("Destroyer", playerPos, Quaternion.identity, 0, new object[] { playerStats.nickName });
                playerObj.name = playerStats.nickName;
                if (playerObj.TryGetComponent(out Destroyer destroyer))
                {
                    destroyer.InitializeStatsPhoton(playerStats);
                }
                break;
            case ClassType.Healer:
                playerObj = PhotonNetwork.Instantiate("Healer", playerPos, Quaternion.identity, 0, new object[] { playerStats.nickName });
                playerObj.name = playerStats.nickName;
                if (playerObj.TryGetComponent(out Healer healer))
                {
                    healer.InitializeStatsPhoton(playerStats);
                }
                break;
            case ClassType.Mage:
                playerObj = PhotonNetwork.Instantiate("Mage", playerPos, Quaternion.identity, 0, new object[] { playerStats.nickName });
                playerObj.name = playerStats.nickName;
                if (playerObj.TryGetComponent(out Mage mage))
                {
                    mage.InitializeStatsPhoton(playerStats);
                }
                break;
        }

        UnitManager.Instance.players.Add(PhotonNetwork.LocalPlayer.ActorNumber, GameObject.Find(PhotonNetwork.LocalPlayer.NickName));

        if (false == PhotonNetwork.IsMasterClient)
        {
            yield break;
        }

        UnitManager.Instance.players.Clear();

        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            string name = player.Value.NickName;
            UnitManager.Instance.players.Add(player.Value.ActorNumber, GameObject.Find(name));
        }


        remainingTime = startTime; // ?곸궠???筌뤾퍓堉????蹂?뜟 ?貫?껆뵳??
        StartCoroutine(GameClock());

        StartCoroutine(Dungeon());
        StartCoroutine(UpdateTimer());

        yield return new WaitUntil(() => !isGameRunning);
        // TODO : game over logic initiate
    }

    private IEnumerator UpdateTimer()
    {
        while (remainingTime > 0)
        {
            if (isTickGoes && dungeonUIController != null)
            {
                TimeSpan timeSpan = TimeSpan.FromSeconds(remainingTime);
                dungeonUIController.UpdateTimerText($"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}");
            }
            yield return null;
        }

        // ???????⑹쾸? 0????琉?????
        if (dungeonUIController == null)
        {
            dungeonUIController.UpdateTimerText("00:00");
        }
    }

    public IEnumerator Dungeon() // Main LOOP
    {
        while (isGameRunning)
        {
            isWaveDone = false;
            yield return StartCoroutine(spawner.MonsterSpwanCorutine());
            // riskUI enable
            Debug.Log("All Wave Launched....");

            yield return new WaitUntil(() => isWaveDone && UnitManager.Instance.monsters.Count <= 0);

            Debug.Log("All Monsters Dead || Time Out....");
            riskUIController.gameObject.SetActive(true);

            //
            yield return new WaitUntil(() => false == riskUIController.gameObject.activeSelf);

            if (isGameForceOver)
            {
                ProcessGameOver(true);
            }
        }
    }


    public void ProcessGameOver(bool isSurrender)
    {
        CalculateRewards(isSurrender);
        isWaveDone = true;
        isGameRunning = false;
    }

    private void CalculateRewards(bool isSurrender)
    {
        float finalReward = 0;

        float baseReward = roomReward;
        float surrenderPenalty = isSurrender ? 0f : 1f;
        finalReward = baseReward * surrenderPenalty;

        photonView.RPC("UpdatePlayerRewardsRPC", RpcTarget.All, finalReward);
    }

    [PunRPC]
    private void UpdatePlayerRewardsRPC(int rewardsJson)
    {
        FirebaseManager.Instance.RewardUpdate(rewardsJson);
    }

    public IEnumerator InstantiatePlayer(PlayerStats playerStats)
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        yield return new WaitUntil(() => PhotonNetwork.IsConnected);

        yield return new WaitUntil(() => PhotonNetwork.InRoom);

        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "LobbyScene");

        //yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "GameScene");
        Vector3 spawnPosition = Vector3.zero;
        PhotonNetwork.LocalPlayer.NickName = playerStats.nickName;

        GameObject playerObj = null;
        switch (FirebaseManager.Instance.currentCharacterData.classType)
        {
            case ClassType.Warrior:
                playerObj = PhotonNetwork.Instantiate("Warrior", spawnPosition, Quaternion.identity);
                playerObj.name = playerStats.nickName;
                if (playerObj.TryGetComponent(out Warrior warrior))
                {
                    warrior.InitializeStatsPhoton(playerStats);
                }
                break;
            case ClassType.Destroyer:
                playerObj = PhotonNetwork.Instantiate("Destroyer", spawnPosition, Quaternion.identity);
                playerObj.name = playerStats.nickName;
                if (playerObj.TryGetComponent(out Destroyer destroyer))
                {
                    destroyer.InitializeStatsPhoton(playerStats);
                }
                break;
            case ClassType.Healer:
                playerObj = PhotonNetwork.Instantiate("Healer", spawnPosition, Quaternion.identity);
                playerObj.name = playerStats.nickName;
                if (playerObj.TryGetComponent(out Healer healer))
                {
                    healer.InitializeStatsPhoton(playerStats);
                }
                break;
            case ClassType.Mage:
                playerObj = PhotonNetwork.Instantiate("Mage", spawnPosition, Quaternion.identity);
                playerObj.name = playerStats.nickName;
                if (playerObj.TryGetComponent(out Mage mage))
                {
                    mage.InitializeStatsPhoton(playerStats);
                }
                break;
        }

        playerObj?.GetComponent<PhotonView>().RPC("RequestPlayerSync", RpcTarget.MasterClient);
    }

    [PunRPC]
    private void SetGameReady()
    {
        isGameRunning = true;
        isGamePaused = false;
        isGameForceOver = false;
    }

    public void RemovePlayerData(PhotonRealtimePlayer otherPlayer)
    {
        FireBaseCharacterData playerToRemove = connectedPlayers.Find(player => player.nickName == otherPlayer.NickName);

        if (playerToRemove != null)
        {
            connectedPlayers.Remove(playerToRemove);
        }
        else
        {
            Debug.LogWarning($"Player {otherPlayer.NickName} not found in connectedPlayers.");
        }

        SyncAllPlayers();
    }
}
