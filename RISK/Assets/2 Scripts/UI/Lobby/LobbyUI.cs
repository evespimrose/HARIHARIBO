using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    public static LobbyUI Instance;
    private Dictionary<string, GameObject> panelDic;
    private Dictionary<string, GameObject> popupDic;
    private Stack<UIPopup> openPopups = new Stack<UIPopup>();

    public PartyListBoard board;
    public CreatePartyUI createPartyInUI;
    public PartyMemberUI partyMemberUI;
    public CharacterUpgradeUI characterUpgradeUI;

    public PopupPanel popup;
    public TwoButtonPopupPanel twoButtonPopup;

    private string openPanel = null;

    private void Awake()
    {
        Instance = this;
        panelDic = new Dictionary<string, GameObject>()
        {
            { "PartyListBoard", board.gameObject },
            { "CreateParty", createPartyInUI.gameObject },
            { "PartyMember", partyMemberUI.gameObject },
            { "CharacterUpgrade" , characterUpgradeUI.gameObject }
        };

        popupDic = new Dictionary<string, GameObject>()
        {
            { "Popup", popup.gameObject },
            { "TwoButtonPopup", twoButtonPopup.gameObject }
        };

        PanelOpen("Login");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (openPopups.Count > 0)
                PopupClose();
            else if (openPanel != null)
                if (panelDic.TryGetValue(openPanel, out var previousPanel))
                    previousPanel.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            PanelOpen("PartyListBoard");
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (PartyManager.Instance.isInParty && PartyManager.Instance.IsPartyLeader(PhotonNetwork.LocalPlayer))
                PopupOpen<TwoButtonPopupPanel>().SetPopup("Dungeon Enter", $"Sure to Enter the Dungeon? Level : {PartyManager.Instance.currentPartyInfo?.goal}",
                    (ok) =>
                    {
                        PhotonManager.Instance.CreateDungeonRoom(PartyManager.Instance.currentPartyInfo);
                    }
                );
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            PanelOpen("CharacterUpgrade");
        }
    }

    public void PanelOpen(string panelName)
    {
        //if (openPanel == panelName) return;

        if (openPanel != null && panelDic.TryGetValue(openPanel, out var previousPanel))
        {
            previousPanel.SetActive(false);
        }

        if (panelDic.TryGetValue(panelName, out var newPanel))
        {
            newPanel.SetActive(true);
            openPanel = panelName;
        }
        else
        {
            Debug.LogWarning($"Panel '{panelName}' not found in panelDic.");
        }
    }

    public void PanelClose()
    {
        if (openPanel != null && panelDic.TryGetValue(openPanel, out var previousPanel))
        {
            previousPanel.SetActive(false);
        }

        openPanel = null;
    }

    public T PopupOpen<T>() where T : UIPopup
    {
        foreach (GameObject popup in popupDic.Values)
        {
            if (popup.TryGetComponent(out T popupComponent))
            {
                if (!popup.activeSelf) popup.SetActive(true);
                openPopups.Push(popupComponent);
                return popupComponent;
            }
        }

        return null;
    }

    public void PopupClose()
    {
        if (openPopups.Count > 0)
        {
            UIPopup targetPopup = openPopups.Pop();
            targetPopup.gameObject.SetActive(false);
        }

    }
}
