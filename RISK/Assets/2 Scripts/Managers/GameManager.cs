using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonManager<GameManager>
{
    public Transform playerPosition;
    public static bool isGameReady;

    protected override void Awake()
    {
    }

    //Photon���� ��Ʈ�� ����ȭ �ϴ� ���
    //1. �����տ� PhotonView ������Ʈ�� ���̰�, PhotonNetwork.Instantiate�� ���� ���� Ŭ���̾�Ʈ�鿡�Ե�
    //����ȭ�� ������Ʈ�� �����ϵ��� ��.
    //2. PhotonView�� Observing �� �� �ֵ��� View ������Ʈ�� ����.
    //3. �� View�� �������� ���� ������Ʈ�� ���� �������� �ʵ��� ����ó���� �ݵ�� �Ұ�.

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => isGameReady);
        yield return new WaitForSeconds(1f);

        //GetPlayerNumber Ȯ���Լ� : ���� ��Ʈ��ũ�� ����� �ٸ� �÷��̾�� ���̿��� ����ȭ �� �÷��̾� ��ȣ.
        //Actor Number�� �ٸ�. (Scene���� ���������� 0~�÷��̾� �� ��ŭ �ο���)
        //GetPlayerNumber Ȯ���Լ��� �����ϱ� ���ؼ��� ���� PlayerNumbering ������Ʈ�� �ʿ��ϴ�.
        int playerNumber = PhotonNetwork.LocalPlayer.GetPlayerNumber();
        Vector3 playerPos = playerPosition.GetChild(playerNumber).position;
        GameObject playerObj = PhotonNetwork.Instantiate("Player", playerPos, Quaternion.identity);
        playerObj.name = $"Player {playerNumber}";

        //Vector3 spawnPos = playerPosition.GetChild(Random.Range(0, playerPosition.childCount)).position;

        //PhotonNetwork.Instantiate("Player", spawnPos, Quaternion.identity).name
        //    = PhotonNetwork.NickName;

        // �� �ؿ����� ���� MasterClient�� �ƴϸ� �������� ����
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
}
