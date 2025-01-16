using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;

public class CreatePartyUI : MonoBehaviour
{
    public Transform difficultyButtonTransform;

    public TextMeshProUGUI maxPartyMember;
    public TMP_InputField partyName;

    public Button minusMinLevelButton;
    public Button minusMaxLevelButton;
    public Button minusMaxPartyMembersButton;

    public Button plusMinLevelButton;
    public Button plusMaxLevelButton;
    public Button plusMaxPartyMembersButton;


    public TextMeshProUGUI minPartyLevel;
    public TextMeshProUGUI maxPartyLevel;
    public int minLevel;
    public int maxLevel;
    public int maxPartyMembers;
    public int goal;

    public Toggle isPublicParty;

    public Button createButton;
    public Button closeButton;

    private void Awake()
    {
        minLevel = 1;
        maxLevel = 2;
        maxPartyMembers = 4;
        isPublicParty.isOn = true;

        for (int i = 0; i < 5; ++i)
        {
            Button button = Instantiate(Resources.Load<Button>("DifficultySettingButton"), difficultyButtonTransform);
            button.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = (i + 1).ToString();
            button.onClick.AddListener(() => { goal = i; });
        }

        createButton.onClick.AddListener(CreatePartyRoom);
        closeButton.onClick.AddListener(OnCloseButtonClick);

        minusMinLevelButton.onClick.AddListener(() => { minLevel--; });
        minusMaxLevelButton.onClick.AddListener(() => { maxLevel--; });
        plusMinLevelButton.onClick.AddListener(() => { minLevel++; });
        plusMaxLevelButton.onClick.AddListener(() => { maxLevel++; });
        minusMaxPartyMembersButton.onClick.AddListener(() => { maxPartyMembers--; });
        plusMaxPartyMembersButton.onClick.AddListener(() => { maxPartyMembers++; });
    }

    private void OnCloseButtonClick()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        minPartyLevel.text = minLevel.ToString();
        maxPartyLevel.text = maxLevel.ToString();
        maxPartyMember.text = maxPartyMembers.ToString();

        plusMinLevelButton.interactable = !(minLevel >= maxLevel);
        minusMaxLevelButton.interactable = !(minLevel >= maxLevel);

        minusMinLevelButton.interactable = !(minLevel <= 1);
        minusMaxPartyMembersButton.interactable = !(maxPartyMembers <= 1);
        plusMaxPartyMembersButton.interactable = !(maxPartyMembers >= 4);

    }

    public void CreatePartyRoom()
    {
        // public add?
        string partyName = this.partyName.text;

        if (!string.IsNullOrEmpty(partyName))
        {
            PartyInfo newPartyInfo = new PartyInfo
            {
                name = partyName,
                partyId = PhotonManager.Instance.partyRoomInfoList.Count,
                currentMemberCount = 1,
                currentMemberActorNumber = new int[] { PhotonNetwork.LocalPlayer.ActorNumber },
                currentLeaderActorNumber = PhotonNetwork.LocalPlayer.ActorNumber,
                goal = goal
            };

            PartyManager.Instance.CreateParty(newPartyInfo);

            LobbyUI.Instance.board.UpdatePartyList();

            gameObject.SetActive(false);
        }
        else
        {
            LobbyUI.Instance.PopupOpen<PopupPanel>().SetPopup("Error", "party name must be filled.\n");
        }
    }
}
