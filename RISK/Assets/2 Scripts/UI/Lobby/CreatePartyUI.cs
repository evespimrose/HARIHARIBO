using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CreatePartyUI : MonoBehaviour
{
    public int difficultyText;
    public Transform difficultyButtonTransform;

    public TextMeshProUGUI maxPartyMember;
    public TMP_InputField partyName;

    public Button minusMinLevelButton;
    public Button minusMaxLevelButton;
    public Button plusMinLevelButton;
    public Button plusMaxLevelButton;

    public TextMeshProUGUI minPartyLevel;
    public TextMeshProUGUI maxPartyLevel;
    public Toggle isPublicParty;

    public Button CreateButton;
    public Button CloseButton;

    private void Awake()
    {
        for (int i = 0; i < 5; ++i)
        {
            Button button = Instantiate(Resources.Load<Button>("DifficultySettingButton"), difficultyButtonTransform);
            button.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = i.ToString();
        }
    }

    public void CreatePartyRoom()
    {
        string partyName = this.partyName.text;
        int difficulty = difficultyText + 1;

        if (!string.IsNullOrEmpty(partyName))
        {
            PartyInfo newPartyInfo = new PartyInfo
            {
                name = partyName,
                partyId = PhotonManager.Instance.partyRoomInfoList.Count,
                currentMemberCount = 1,
                currentMemberActorNumber = new int[] { PhotonNetwork.LocalPlayer.ActorNumber },
                currentLeaderActorNumber = PhotonNetwork.LocalPlayer.ActorNumber
            };

            PartyManager.Instance.CreateParty(newPartyInfo);
            LobbyUI.Instance.board.UpdatePartyList();
        }
        else
        {
            Debug.LogWarning("Room name cannot be empty.");
        }
    }
}
