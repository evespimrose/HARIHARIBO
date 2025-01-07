using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//���� �ݹ� ������
public class PhotonManager : MonoBehaviourPunCallbacks
{

    private void Start()
    {
        //Active at GameScene
        //GameManager.isGameReady = true;
    }

    public override void OnJoinedRoom()
    {
        //if (isTestmode)
        //{
        //    GameObject.Find("Canvas/DebugText").GetComponent<Text>().text
        //        = PhotonNetwork.CurrentRoom.Name;
        //    GameManager.isGameReady = true;
        //}
    }
}
