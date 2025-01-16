using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using PhotonRealtimePlayer = Photon.Realtime.Player;

public class PartyMemberUI : MonoBehaviour
{
    public Text partyLeaderText;
    public Transform partyMemberContainer;
    public GameObject partyMemberItemPrefab;

    private void OnEnable()
    {
        UpdatePartyMembers();
    }

    public void UpdatePartyMembers()
    {
        PhotonRealtimePlayer partyLeader = PartyManager.Instance.GetPartyLeader();
        if (partyLeader != null)
        {
            partyLeaderText.text = $"Party Leader: {partyLeader.NickName} (Level: {partyLeader.CustomProperties["Level"]})";
        }

        foreach (Transform child in partyMemberContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (PhotonRealtimePlayer member in PartyManager.Instance.GetPartyMembers())
        {
            GameObject memberItem = Instantiate(partyMemberItemPrefab, partyMemberContainer);
            memberItem.GetComponentInChildren<Text>().text = $"{member.NickName} (Level: {member.CustomProperties["Level"]})";
        }
    }
}
