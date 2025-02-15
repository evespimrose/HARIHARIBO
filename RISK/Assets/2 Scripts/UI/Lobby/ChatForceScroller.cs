using Google.MiniJSON;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Chat;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using PhotonRealtimePlayer = Photon.Realtime.Player;
using ExitGames.Client.Photon;

public class ChatScrollController : SingletonManager<ChatScrollController>, IChatClientListener
{
    public ScrollRect scrollRect;
    public VerticalLayoutGroup layoutGroup;
    public TMP_InputField chatInputField;
    public Button sendButton;
    public Button hideButton;
    public Button showButton;
    public TextMeshProUGUI hideText;

    public TextMeshProUGUI playerList;
    string players;

    private ChatClient chatClient;
    private string currentChannelName;
    public TextMeshProUGUI chatLogText;
    public GameObject Users;

    protected override void Awake()
    {
        base.Awake();

        sendButton.onClick.AddListener(OnSendButtonClick);
        hideButton.onClick.AddListener(OnHideButtonClick);
        showButton.onClick.AddListener(OnShowButtonClick);

        chatInputField.onSubmit.AddListener(OnSendButtonClick);

        chatLogText.fontSize = 24f;

        gameObject.SetActive(false);
    }

    private void AttachToCurrentCanvas()
    {
        Canvas newCanvas = FindObjectOfType<Canvas>();
        if (newCanvas != null)
        {
            transform.SetParent(newCanvas.transform, false);
        }
    }

    private void OnHideButtonClick()
    {
        Users.gameObject.SetActive(!Users.gameObject.activeSelf);
    }

    private void OnShowButtonClick()
    {
        Users.SetActive(!Users.gameObject.activeSelf);
    }

    private void OnSendButtonClick()
    {
        AddMessage(chatInputField.text);
    }

    private void OnSendButtonClick(string msg)
    {
        //AddMessage(msg);

        if (chatClient.State == ChatState.ConnectedToFrontEnd)
        {
            chatClient.PublishMessage(currentChannelName, msg);

            chatInputField.text = "";
        }
    }

    private void Start()
    {
        Application.runInBackground = true;
        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0", new AuthenticationValues(FirebaseManager.Instance.currentCharacterData.nickName));
        currentChannelName = "Channel 001";

        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
        ScrollToBottom();
    }

    void Update()
    {
        ChatterUpdate();

        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && chatInputField.isFocused) OnSendButtonClick(chatInputField.text);

        chatClient.Service();
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
            players += data.nickName + "(Lv." + data.level + ", " + data.classType.ToString() + ")\n";
        }
        playerList.text = players;
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
        chatLogText.text += msg + "\n";

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

    void IChatClientListener.DebugReturn(DebugLevel level, string message)
    {
    }

    void IChatClientListener.OnConnected()
    {
        chatClient.Subscribe(new string[] { currentChannelName }, 10);
    }

    void IChatClientListener.OnChatStateChange(ChatState state)
    {
        Debug.Log("OnChatStateChange = " + state);
    }

    void IChatClientListener.OnSubscribed(string[] channels, bool[] results)
    {
        AddMessage(string.Format("channel ({0}) Subscribed", string.Join(",", channels)));
    }

    void IChatClientListener.OnUnsubscribed(string[] channels)
    {
        AddMessage(string.Format("channel ({0}) Unsubscribed", string.Join(",", channels)));
    }

    void IChatClientListener.OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < messages.Length; i++)
        {
            AddMessage(string.Format("{0} : {1}", senders[i], messages[i].ToString()));
        }
    }

    void IChatClientListener.OnPrivateMessage(string sender, object message, string channelName)
    {
        Debug.Log("OnPrivateMessage : " + message);
    }

    void IChatClientListener.OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log("status : " + string.Format("{0} is {1}, Msg : {2} ", user, status, message));
    }

    void IChatClientListener.OnUserSubscribed(string channel, string user)
    {
        throw new NotImplementedException();
    }

    void IChatClientListener.OnUserUnsubscribed(string channel, string user)
    {
        throw new NotImplementedException();
    }

    void IChatClientListener.OnDisconnected()
    {

    }

}
