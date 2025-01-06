using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class LoginPanel : MonoBehaviour
{
    public InputField idInput;
    public InputField pwInput;
    public Button signupButton;
    public Button loginButton;

    private void Awake()
    {
        loginButton.onClick.AddListener(OnLoginButtonClick);
    }

    private void OnLoginButtonClick()
    {
        string userNickname = idInput.text;
        PhotonNetwork.NickName = userNickname;
        PhotonNetwork.ConnectUsingSettings();
        //PhotonNetwork.AutomaticallySyncScene = true;
    }
}