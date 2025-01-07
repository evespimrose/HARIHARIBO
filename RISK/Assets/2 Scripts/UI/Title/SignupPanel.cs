using Firebase.Auth;
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

    public TextMeshProUGUI duplicationCheckResultText;

    public Button signupButton;
    public Button duplicateCheckButton;

    public bool isDuplicationChecked;
    public bool isDuplicate;

    public Toggle termToggle;

    private void OnEnable()
    {
        signupButton.onClick.AddListener(OnSignupButtonClick);
        duplicateCheckButton.onClick.AddListener(OnDuplicateCheckButtonClick);
    }

    private void OnDuplicateCheckButtonClick()
    {
        // 이메일 입력 확인
        if (string.IsNullOrEmpty(idInput.text))
        {
            duplicationCheckResultText.text = "ID is Null.";
            return;
        }
        else if (!IsValidEmail(idInput.text))
        {
            duplicationCheckResultText.text = "ID is Invalid E-Mail.";
            return;
        }

        // Firebase Duplication Check
        FirebaseManager.Instance.DuplicationCheck(idInput.text, DuplicationConfirmed);
    }

    private void DuplicationConfirmed(bool isDuplicate)
    {
        if (isDuplicate)
        {
            duplicationCheckResultText.text = "ID is already in use.";
            this.isDuplicate = true;
        }
        else
        {
            duplicationCheckResultText.text = "Valid ID!";
            this.isDuplicate = false;
            isDuplicationChecked = true;
        }
    }


    private void OnSignupButtonClick()
    {
        if (isValidIDPW())
        {
            FirebaseManager.Instance.Create(idInput.text, pwInput.text, CreateCallback);
        }
    }

    private bool isValidIDPW()
    {
        // 아이디창 비워짐, 비밀번호창 비워짐, 비밀번호!=비밀번호확인, 비밀번호확인창 비워짐, 토글 비워짐, 아이디 양식 안맞음, 비밀번호 양식 안맞음, 중복 시도해봤었어야됨, 중복 아닌 아이디여야됨
        if (string.IsNullOrEmpty(idInput.text))
        {
            var popup = PanelManager.Instance.PopupOpen<PopupPanel>();
            popup.SetPopup("Auth Failed", "ID null.");
            return false;
        }
        else if (!IsValidEmail(idInput.text))
        {
            var popup = PanelManager.Instance.PopupOpen<PopupPanel>();
            popup.SetPopup("Auth Failed", "Invalid ID!");
            return false;
        }
        else if (!isDuplicationChecked)
        {
            var popup = PanelManager.Instance.PopupOpen<PopupPanel>();
            popup.SetPopup("Auth Failed", "ID didn't DuplicationChecked!");
            return false;
        }
        else if (isDuplicationChecked && isDuplicate)
        {
            var popup = PanelManager.Instance.PopupOpen<PopupPanel>();
            popup.SetPopup("Auth Failed", "Duplicated ID!");
            return false;
        }
        else if (string.IsNullOrEmpty(pwInput.text))
        {
            var popup = PanelManager.Instance.PopupOpen<PopupPanel>();
            popup.SetPopup("Auth Failed", "PW null.");
            return false;
        }
        else if (string.IsNullOrEmpty(pwMatchInput.text) || pwInput.text != pwMatchInput.text)
        {
            var popup = PanelManager.Instance.PopupOpen<PopupPanel>();
            popup.SetPopup("Auth Failed", "pwmatch null || text mismatch.");
            return false;
        }
        else if (!termToggle.isOn)
        {
            var popup = PanelManager.Instance.PopupOpen<PopupPanel>();
            popup.SetPopup("Auth Failed", "yakgwan.");
            return false;
        }
        // 아이디 양식 안맞음, 비밀번호 양식 안맞음(미구현)

        return true;
    }

    private void CreateCallback(FirebaseUser user)
    {
        PanelManager.Instance.PopupOpen<PopupPanel>().
            SetPopup("회원가입 완료", "회원 가입이 완료 되었습니다.\n 로그인 해주세요. ", DialogCallback);
    }

    private void DialogCallback()
    {
        PanelManager.Instance.PanelOpen("Login");
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
