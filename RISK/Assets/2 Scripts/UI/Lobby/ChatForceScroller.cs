using Google.MiniJSON;
using Newtonsoft.Json;
using Photon.Pun;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using PhotonRealtimePlayer = Photon.Realtime.Player;

public class ChatScrollController : PhotonSingletonManager<ChatScrollController>
{
    public ScrollRect scrollRect;
    public VerticalLayoutGroup layoutGroup;
    public TMP_InputField chatInputField;
    public Button sendButton;
    public Button hideButton;
    public TextMeshProUGUI hideText;

    public TextMeshProUGUI playerList;
    string players;

    protected override void Awake()
    {
        base.Awake();
        sendButton.onClick.AddListener(OnSendButtonClick);
        hideButton.onClick.AddListener(OnHideButtonClick);

        chatInputField.onSubmit.AddListener(OnSendButtonClick);
    }

    private void OnHideButtonClick()
    {
        if (gameObject.activeSelf)
            hideText.text = "+";
        else
            hideText.text = "-";

        gameObject.SetActive(!gameObject.activeSelf);
    }

    private void OnSendButtonClick()
    {
        AddMessage(chatInputField.text);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        gameObject.SetActive(true);
    }

    private void OnSendButtonClick(string msg)
    {
        AddMessage(msg);
    }

    private void Start()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
        ScrollToBottom();
    }

    void Update()
    {
        ChatterUpdate();

        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && chatInputField.isFocused) SendMessage();
    }

    public void SendMessage()
    {
        if (chatInputField.text.Equals(""))
        {
            Debug.Log("Empty");
            return;
        }

        string msg =
            string.Format("[{0}] (lv.{1}, {2}) : {3}", FirebaseManager.Instance.currentCharacterData.nickName, FirebaseManager.Instance.currentCharacterData.level,
            FirebaseManager.Instance.currentCharacterData.classType.ToString(), chatInputField.text);

        photonView.RPC("ReceiveMsg", RpcTarget.OthersBuffered, msg);
        AddMessage(msg);
        chatInputField.ActivateInputField();
        chatInputField.text = "";
    }

    [PunRPC]
    public void ReceiveMsg(string msg)
    {
        print(msg);
        AddMessage(msg);
    }

    void ChatterUpdate()
    {
        players = "Players\n";
        foreach (var data in GameManager.Instance.connectedPlayers)
        {
            players += data.nickName + "(Lv."+ data.level +", "+ data.classType.ToString() + ")\n";
        }
        playerList.text = players;
    }

    public override void OnPlayerEnteredRoom(PhotonRealtimePlayer newPlayer)
    {
        FireBaseCharacterData data = JsonConvert.DeserializeObject<FireBaseCharacterData>(newPlayer.NickName);
        string msg = string.Format("<color=#00ff00>[{0}] joined room.</color>", data.nickName);
        AddMessage(msg);
    }

    public override void OnPlayerLeftRoom(PhotonRealtimePlayer otherPlayer)
    {
        FireBaseCharacterData data = JsonConvert.DeserializeObject<FireBaseCharacterData>(otherPlayer.NickName);
        string msg = string.Format("<color=#ff0000>[{0}] left room.</color>", data.nickName);
        AddMessage(msg);
    }

    public void AddMessage()
    {
        GameObject message = PhotonNetwork.Instantiate("message", layoutGroup.transform.position, Quaternion.identity);
        message.TryGetComponent(out TextMeshProUGUI text);
        text.text = chatInputField.text;
        chatInputField.text = "";

        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
        ScrollToBottom();
    }

    public void AddMessage(string msg)
    {
        GameObject message = PhotonNetwork.Instantiate("message", layoutGroup.transform.position, Quaternion.identity);
        message.transform.SetParent(layoutGroup.transform, true);
        message.TryGetComponent(out TextMeshProUGUI text);
        text.fontSize = 24f;
        text.text = msg;
            
        chatInputField.text = "";

        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
        ScrollToBottom();
    }

    private void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();
    }

    public bool isFocused()
    {
        return chatInputField.isFocused;
    }
}
