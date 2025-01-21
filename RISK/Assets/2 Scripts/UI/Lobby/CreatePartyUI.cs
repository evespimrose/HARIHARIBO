using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;
using Photon.Realtime;
using HashTable = ExitGames.Client.Photon.Hashtable;
using PhotonRealtimePlayer = Photon.Realtime.Player;

public class CreatePartyUI : MonoBehaviour
{
    public Transform difficultyButtonTransform;

    public TextMeshProUGUI maxPartyMember;
    public TextMeshProUGUI defficultyText;
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
            button.onClick.AddListener(() => { goal = i; defficultyText.text = goal.ToString(); });
            button.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = (i + 1).ToString();
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
                maxPartyMemberCount = maxPartyMembers,
                goal = goal,
                minPartyLevel = minLevel,
                maxPartyLevel = maxLevel
            };



            RoomOptions roomOptions = new RoomOptions
            {
                MaxPlayers = newPartyInfo.maxPartyMemberCount,
                IsVisible = true,
                IsOpen = true,
                CustomRoomProperties = new HashTable {
                    { "Difficulty", newPartyInfo.goal }, { "minLevel", newPartyInfo.minPartyLevel },
                    { "maxLevel", newPartyInfo.maxPartyLevel },{ "roomId", newPartyInfo.partyId }
                },
                CustomRoomPropertiesForLobby = new string[] { "Difficulty", "minLevel", "maxLevel", "roomId" }
            };

            PhotonNetwork.CreateRoom(newPartyInfo.name, roomOptions, TypedLobby.Default);

            //PanelManager.Instance.partyListBoard.UpdateRoomList();
            PanelManager.Instance.PanelOpen("PartyMember");
        }
        else
        {
            PanelManager.Instance.PopupOpen<PopupPanel>().SetPopup("Error", "party name must be filled.\n", () => { PanelManager.Instance.PanelOpen("PartyListBoard"); });
        }
    }
}
