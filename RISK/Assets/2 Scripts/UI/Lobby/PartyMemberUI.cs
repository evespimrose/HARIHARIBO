using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using PhotonRealtimePlayer = Photon.Realtime.Player;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] private Transform partyMemberContainer;
    [SerializeField] private GameObject partyMemberInfoPrefab;

    private void OnEnable()
    {
        UpdatePartyMembers();
    }

    public void UpdatePartyMembers()
    {
        foreach (Transform child in partyMemberContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var member in PartyManager.Instance.GetPartyMembers())
        {
            GameObject memberInfoObj = Instantiate(partyMemberInfoPrefab, partyMemberContainer);
            if (memberInfoObj.TryGetComponent(out PartyMemberInfoUI memberInfoUI))
            {
                memberInfoUI.Initialize(member);
            }
        }
    }
}
