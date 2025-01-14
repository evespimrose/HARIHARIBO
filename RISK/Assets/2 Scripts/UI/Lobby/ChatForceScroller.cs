using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatScrollController : SingletonManager<ChatScrollController>
{
    public ScrollRect scrollRect;
    public VerticalLayoutGroup layoutGroup;
    public TMP_InputField chatInputField;
    public Button sendButton;
    public Button hideButton;
    public TextMeshProUGUI hideText;

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
        AddMessage();
    }

    private void OnSendButtonClick(string msg)
    {
        AddMessage();
    }

    private void Start()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
        ScrollToBottom();
    }

    public void AddMessage()
    {
        GameObject message = Instantiate<GameObject>(Resources.Load<GameObject>("message"), layoutGroup.transform);
        message.TryGetComponent(out TextMeshProUGUI text);
        text.text = chatInputField.text;
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
