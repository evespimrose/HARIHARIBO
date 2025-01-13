using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyBulletinBoard : MonoBehaviour
{
    public GameObject partyListItemPrefab;
    public Transform partyListContainer;
    public InputField roomNameInput;
    public Dropdown difficultyDropdown;

    private void Start()
    {
        UpdatePartyList();
    }

    public void UpdatePartyList()
    {
        foreach (Transform child in partyListContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (RoomInfo room in PhotonManager.Instance.partyRoomInfoList)
        {
            if (room.CustomProperties.ContainsKey("RoomType") && room.CustomProperties["RoomType"].ToString() == "Party")
            {
                GameObject partyItem = Instantiate(partyListItemPrefab, partyListContainer);
                //partyItem.GetComponentInChildren<TextMeshProUGUI>().text = room.Name;
                // button addListener logic
            }
        }
    }

    public void CreatePartyRoom()
    {
        string roomName = roomNameInput.text;
        int difficulty = difficultyDropdown.value + 1;

        if (!string.IsNullOrEmpty(roomName))
        {
            PhotonManager.Instance.CreatePartyRoom(roomName, difficulty);
            UpdatePartyList();
        }
        else
        {
            Debug.LogWarning("Room name cannot be empty.");
        }
    }
}