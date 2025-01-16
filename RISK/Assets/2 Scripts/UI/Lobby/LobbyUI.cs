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
    //public PartyMemberUI partyMemberUI;

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
            //{ "PartyMember", partyMemberUI.gameObject },
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
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (openPopups.Count > 0)
                PopupClose();
            else if (openPanel != null)
                if (panelDic.TryGetValue(openPanel, out var previousPanel))
                    previousPanel.SetActive(false);
        }
        if (Input.GetKeyUp(KeyCode.P))
        {
            PanelOpen("PartyListBoard");
        }
    }

    public void PanelOpen(string panelName)
    {
        if (openPanel == panelName) return;

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
