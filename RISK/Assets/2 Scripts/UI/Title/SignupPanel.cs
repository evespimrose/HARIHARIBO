using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignupPanel : MonoBehaviour
{
    public TMP_InputField idInput;
    public TMP_InputField pwInput;
    public TMP_InputField pwMatchInput;

    public Button signupButton;
    public Button duplicateCheckButton;

    public bool isDuplicateChecked;
    public bool isDuplicate;

    public Toggle termToggle;

    private void OnEnable()
    {
        signupButton.onClick.AddListener(OnSignupButtonClick);
    }

    private void OnSignupButtonClick()
    {
        if (string.IsNullOrEmpty(idInput.text))
        {
            var popup = PanelManager.Instance.PopupOpen<PopupPanel>();
            popup.SetPopup("회원가입 실패", "아이디를 입력해주세요.");
        }
        else if (string.IsNullOrEmpty(pwInput.text))
        {
            var popup = PanelManager.Instance.PopupOpen<PopupPanel>();
            popup.SetPopup("회원가입 실패", "비밀번호를 입력해주세요.");
        }
        else if (string.IsNullOrEmpty(pwMatchInput.text) || pwInput.text == pwMatchInput.text)
        {
            var popup = PanelManager.Instance.PopupOpen<PopupPanel>();
            popup.SetPopup("회원가입 실패", "비밀번호가 다릅니다.");
        }
    }
}
