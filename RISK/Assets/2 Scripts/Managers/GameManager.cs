using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonManager<GameManager>
{
    //public Transform playerPosition;
    public static bool isGameReady;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => isGameReady);
        //yield return new WaitForSeconds(1f);

        //int playerNumber = PhotonNetwork.LocalPlayer.GetPlayerNumber();
        //Vector3 playerPos = playerPosition.GetChild(playerNumber).position;
        //GameObject playerObj = PhotonNetwork.Instantiate("Player", playerPos, Quaternion.identity);
        //playerObj.name = $"Player {playerNumber}";

        ////Vector3 spawnPos = playerPosition.GetChild(Random.Range(0, playerPosition.childCount)).position;

        ////PhotonNetwork.Instantiate("Player", spawnPos, Quaternion.identity).name
        ////    = PhotonNetwork.NickName;

        //if (false == PhotonNetwork.IsMasterClient)
        //{
        //    yield break;
        //}

        //while (true)
        //{
        //    Vector3 spawnPos = Random.insideUnitSphere * 15f;
        //    spawnPos.y = 0;
        //    Quaternion spawnRot = Quaternion.Euler(0, Random.Range(0, 180f), 0);

        //    Vector3 color = new Vector3(Random.value, Random.value, Random.value);
        //    float healAmount = Random.Range(10f, 30f);

        //    PhotonNetwork.Instantiate("Pill", spawnPos, spawnRot, data: new object[] { color, healAmount });

        //    yield return new WaitForSeconds(5f);
        //}
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

        if (playerObj != null)
            playerObj.GetComponent<PhotonView>().RPC("RequestPlayerSync", RpcTarget.MasterClient);
    }
}
