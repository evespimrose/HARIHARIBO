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
        PanelManager.Instance.PopupOpen<TwoButtonPopupPanel>().SetPopup("Quit Game", "Are you sure you want to quit?",
           ok =>
           {
               if (ok)
               {
#if UNITY_EDITOR
                   UnityEditor.EditorApplication.isPlaying = false;
#else
                       Application.Quit();
#endif
               }
               else
                   PanelManager.Instance.PopupClose();
           });
    }

    private void OnSignupButtonClick()
    {
        PanelManager.Instance.PanelOpen("Signup");
    }

    private void OnLoginButtonClick()
    {
        // �̸��� ���� �˻�
        string email = idInput.text;
        if (!IsValidEmail(email))
        {
            var popup = PanelManager.Instance.PopupOpen<PopupPanel>();
            popup.SetPopup("LogIn Failed", "Invalid ID!");
            return;
        }

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