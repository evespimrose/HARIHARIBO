using Photon.Pun;
using System.Collections;
using UnityEngine;
using Photon.Realtime;
using System.Collections.Generic;
using Newtonsoft.Json;

public class PhotonRequest : MonoBehaviourPunCallbacks
{
    #region Singleton
    private static PhotonRequest instance;
    public static PhotonRequest Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PhotonRequest>();
                if (instance == null)
                {
                    GameObject go = new GameObject("PhotonRequest");
                    instance = go.AddComponent<PhotonRequest>();
                }
            }
            return instance;
        }
    }
    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region UnitManager RPCs
    public void RequestPlayerSync(int viewID, int actorNumber, string playerName)
    {
        photonView.RPC("SyncPlayer", RpcTarget.MasterClient, viewID, actorNumber, playerName);
    }

    [PunRPC]
    private void SyncPlayer(int viewID, int actorNumber, string playerName)
    {
        PhotonView targetView = PhotonView.Find(viewID);
        if (targetView != null)
        {
            UnitManager.Instance.players[actorNumber] = targetView.gameObject;
        }
    }

    // 데미지 적용
    public void ApplyDamage(int targetID, float damage)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("ApplyDamageRPC", RpcTarget.All, targetID, damage);
        }
    }

    [PunRPC]
    private void ApplyDamageRPC(int targetID, float damage)
    {
        PhotonView targetView = PhotonView.Find(targetID);
        if (targetView != null)
        {
            targetView.GetComponent<ITakedamage>()?.Takedamage(damage);
        }
    }

    #endregion

    #region GameManager RPCs
    // 게임 준비 상태 설정
    public void SetGameReady()
    {
        photonView.RPC("SetGameReadyRPC", RpcTarget.All);
    }

    [PunRPC]
    private void SetGameReadyRPC()
    {
        GameManager.Instance.isGameRunning = true;
        GameManager.Instance.isGamePaused = false;
        GameManager.Instance.isGameForceOver = false;
    }

    public void RiskUIActive(bool isActive)
    {
        photonView.RPC("RiskUIActiveRPC", RpcTarget.All, isActive);
    }

    [PunRPC]
    private void RiskUIActiveRPC(bool isActive)
    {
        GameManager.Instance.riskUIController.gameObject.SetActive(isActive);
    }

    // 게임 오버 처리
    public void GameOver()
    {
        photonView.RPC("GameOverRPC", RpcTarget.All);
    }

    [PunRPC]
    private void GameOverRPC()
    {
        if (GameManager.Instance != null)
        {
            PhotonNetwork.LeaveRoom();
            GameManager.Instance.chat.gameObject.SetActive(false);
            PanelManager.Instance?.PanelOpen("PartyListBoard");
        }
    }

    public void RequestRiskUIGold(float gold)
    {
       photonView.RPC("RequestRiskUIGoldRPC",RpcTarget.All, gold);
    }

    [PunRPC]
    private void RequestRiskUIGoldRPC(float gold)
    {
        GameManager.Instance.riskUIController.rewardText.text = gold.ToString();
    }

    // 항복 처리
    public void ProcessSurrender()
    {
        photonView.RPC("ProcessSurrenderRPC", RpcTarget.MasterClient);
    }

    [PunRPC]
    private void ProcessSurrenderRPC()
    {
        GameManager.Instance.isWaveDone = true;
        GameManager.Instance.isGameForceOver = true;
        CalculateRewards(true);
    }

    // 보상 계산 및 업데이트
    public void CalculateRewards(bool isSurrender)
    {
        float baseReward = GameManager.Instance.roomReward;
        float surrenderPenalty = isSurrender ? 0f : 1f;
        float finalReward = baseReward * surrenderPenalty;

        photonView.RPC("UpdatePlayerRewardsRPC", RpcTarget.All, finalReward);
    }

    [PunRPC]
    public void UpdatePlayerRewardsRPC(float rewards)
    {
        FirebaseManager.Instance.RewardUpdate(rewards);
    }

    // 플레이어 데이터 동기화
    public void SyncPlayerData(string serializedData)
    {
        photonView.RPC("SyncPlayerDataRPC", RpcTarget.All, serializedData);
    }

    [PunRPC]
    private void SyncPlayerDataRPC(string serializedData)
    {
        var playerData = JsonConvert.DeserializeObject<FireBaseCharacterData>(serializedData);
        if (!GameManager.Instance.connectedPlayers.Contains(playerData))
        {
            GameManager.Instance.connectedPlayers.Add(playerData);
        }
    }

    // 스턴 상태 적용
    public void ApplyStunStatus(int targetID)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("ApplyStunStatusRPC", RpcTarget.All, targetID);
        }
    }

    [PunRPC]
    private void ApplyStunStatusRPC(int targetID)
    {
        PhotonView targetView = PhotonView.Find(targetID);
        if (targetView != null)
        {
            if (targetView.TryGetComponent(out Monster monster))
            {
                monster.isStun = true;
            }
        }
    }
    #endregion
}
