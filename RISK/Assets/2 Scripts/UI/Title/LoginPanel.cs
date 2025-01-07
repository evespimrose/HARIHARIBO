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
    public Button quitButton;

    private void Awake()
    {
        loginButton.onClick.AddListener(OnLoginButtonClick);
        signupButton.onClick.AddListener(OnSignupButtonClick);
        quitButton.onClick.AddListener(OnQuitButtonClick);
    }

    private void OnQuitButtonClick()
    {

    }

    private void OnSignupButtonClick()
    {
        PanelManager.Instance.PanelOpen("Signup");
    }

    private void OnLoginButtonClick()
    {
        // 이메일 형식 검사
        string email = idInput.text;
        if (!IsValidEmail(email))
        {
            var popup = PanelManager.Instance.PopupOpen<PopupPanel>();
            popup.SetPopup("LogIn Failed", "Invalid ID!");
            return;
        }

        //로그인 성공 후 데베 접근해서 캐릭터 셋 불러오고, 캐릭터 고르는 창으로 넘어가야됨.
        FirebaseManager.Instance.SignIn(email, pwInput.text, (firebaseUser) => { PanelManager.Instance.PanelOpen("SelectCharacter"); });
    }
    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }


}