using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using HashTable = ExitGames.Client.Photon.Hashtable;
using PhotonRealtimePlayer = Photon.Realtime.Player;
using Photon.Pun.UtilityScripts;

public class GameManager : MonoBehaviourPunSingletonManager<GameManager>
{
    public static bool isGameReady;

    public List<FireBaseCharacterData> connectedPlayers = new List<FireBaseCharacterData>();

    public Transform playerPosition;

    public MonsterSpwan spawner;

    public bool isWaveDone = false;

    public RiskUIController riskUIController;

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
        yield return new WaitUntil(() => isGameReady);
        print("isGameReady!!! GOGOGOGO!!!!");
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "GameScene");
        spawner = (MonsterSpwan)FindAnyObjectByType(typeof(MonsterSpwan));

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

        int playerNumber = PhotonNetwork.LocalPlayer.GetPlayerNumber();
        print($"playerNumber : {playerNumber}");
        playerPosition = GameObject.Find("SpawnPosition").transform;

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

        if (false == PhotonNetwork.IsMasterClient)
        {
            yield break;
        }

        UnitManager.Instance.players.Clear();

        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            string name = player.Value.NickName;
            UnitManager.Instance.players.Add(player.Value.GetPlayerNumber(), GameObject.Find(name));
        }

        StartCoroutine(Dungeon());
    }


    public IEnumerator Dungeon()
    {
        while (true)
        {
            //spawner cour 끝날때가지 대기
            yield return StartCoroutine(spawner.MonsterSpwanCorutine());
            // riskUI enable

            //
            yield return new WaitUntil(() => false == riskUIController.gameObject.activeSelf);

        }
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
        isGameReady = true;
    }
}
