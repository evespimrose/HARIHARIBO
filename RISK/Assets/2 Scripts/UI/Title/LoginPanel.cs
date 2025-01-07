using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;

public class LoginPanel : MonoBehaviour
{
    public TMP_InputField idInput;
    public TMP_InputField pwInput;
    public Button signupButton;
    public Button loginButton;

    private void Awake()
    {
        loginButton.onClick.AddListener(OnLoginButtonClick);
        signupButton.onClick.AddListener(OnSignupButtonClick);
    }

    private void OnSignupButtonClick()
    {
        PanelManager.Instance.PanelOpen("Signup");
    }

    private void OnLoginButtonClick()
    {
        string userNickname = idInput.text;
        PhotonNetwork.NickName = userNickname;
        PhotonNetwork.ConnectUsingSettings();
    }

}