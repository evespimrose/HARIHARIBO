using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonManager<GameManager>
{
    public Transform playerPosition;
    public static bool isGameReady;

    protected override void Awake()
    {
        base.Awake();
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => isGameReady);
        yield return new WaitForSeconds(1f);

        int playerNumber = PhotonNetwork.LocalPlayer.GetPlayerNumber();
        Vector3 playerPos = playerPosition.GetChild(playerNumber).position;
        GameObject playerObj = PhotonNetwork.Instantiate("Player", playerPos, Quaternion.identity);
        playerObj.name = $"Player {playerNumber}";

        //Vector3 spawnPos = playerPosition.GetChild(Random.Range(0, playerPosition.childCount)).position;

        //PhotonNetwork.Instantiate("Player", spawnPos, Quaternion.identity).name
        //    = PhotonNetwork.NickName;

        if (false == PhotonNetwork.IsMasterClient)
        {
            yield break;
        }

        // MasterClient�� 5�ʸ��� Pill�� PhotonNetwork�� ���� Instantiate.
        while (true)
        {
            Vector3 spawnPos = Random.insideUnitSphere * 15f;
            spawnPos.y = 0;
            Quaternion spawnRot = Quaternion.Euler(0, Random.Range(0, 180f), 0);

            // �� Pill���� random color�� random healAmount�� �����ϰ� ������?

            Vector3 color = new Vector3(Random.value, Random.value, Random.value);
            float healAmount = Random.Range(10f, 30f);

            PhotonNetwork.Instantiate("Pill", spawnPos, spawnRot, data: new object[] { color, healAmount });

            yield return new WaitForSeconds(5f);
        }
    }
    public IEnumerator InstantiatePlayer(Playerstats playerStats)
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        yield return new WaitUntil(() => PhotonNetwork.IsConnected);

        yield return new WaitUntil(() => PhotonNetwork.InRoom);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("LobbyScene", LoadSceneMode.Single);

        yield return new WaitUntil(() => asyncLoad.isDone);

        switch(FirebaseManager.Instance.currentCharacterData.classType)
        {
            case ClassType.Warrior:
                break;
            case ClassType.SpearMan:
                break;
            case ClassType.Archer: 
                break;
            case ClassType.Mage:
                break;
        }

        Vector3 spawnPosition = Vector3.zero;
        GameObject playerObj = PhotonNetwork.Instantiate("Player", spawnPosition, Quaternion.identity);

        //GameObject playerObj = Instantiate(Resources.Load<GameObject>("Player"), spawnPosition, Quaternion.identity);

        UnitManager.Instance?.RegisterPlayer(playerObj);
    }
}
