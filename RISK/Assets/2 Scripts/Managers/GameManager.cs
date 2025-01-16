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

        switch (FirebaseManager.Instance.currentCharacterData.classType)
        {
            case ClassType.Warrior:
                {
                    GameObject warriorObj = PhotonNetwork.Instantiate("Warrior", spawnPosition, Quaternion.identity);
                    warriorObj.name = $"Warrior {playerStats.nickName}";

                    if (warriorObj.TryGetComponent(out Warrior warrior))
                    {
                        warrior.InitializeStatsPhoton(playerStats);
                    }
                }
                break;
            case ClassType.Destroyer:
                {
                    GameObject destroyerObj = PhotonNetwork.Instantiate("Destroyer", spawnPosition, Quaternion.identity);
                    destroyerObj.name = $"Destroyer {playerStats.nickName}";

                    if (destroyerObj.TryGetComponent(out Destroyer destroyer))
                    {
                        destroyer.InitializeStatsPhoton(playerStats);
                    }
                }
                break;
            case ClassType.Healer:
                {
                    GameObject healerObj = PhotonNetwork.Instantiate("Healer", spawnPosition, Quaternion.identity);
                    healerObj.name = $"Healer {playerStats.nickName}";

                    if (healerObj.TryGetComponent(out Healer healer))
                    {
                        healer.InitializeStatsPhoton(playerStats);
                    }
                }
                break;
            case ClassType.Mage:
                {
                    GameObject mageObj = PhotonNetwork.Instantiate("Mage", spawnPosition, Quaternion.identity);
                    mageObj.name = $"Mage {playerStats.nickName}";

                    if (mageObj.TryGetComponent(out Mage mage))
                    {
                        mage.InitializeStatsPhoton(playerStats);
                    }
                }
                break;
        }
    }
}
