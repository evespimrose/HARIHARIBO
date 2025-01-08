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
        // �̸��� �Է� Ȯ��
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
        // ���̵�â �����, ��й�ȣâ �����, ��й�ȣ!=��й�ȣȮ��, ��й�ȣȮ��â �����, ��� �����, ���̵� ��� �ȸ���, ��й�ȣ ��� �ȸ���, �ߺ� �õ��غþ���ߵ�, �ߺ� �ƴ� ���̵𿩾ߵ�
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
        // ���̵� ��� �ȸ���, ��й�ȣ ��� �ȸ���(�̱���)

        return true;
    }

    private void CreateCallback(FirebaseUser user)
    {
        PanelManager.Instance.PopupOpen<PopupPanel>().
            SetPopup("Auth Successed!", "Auth is Successed.\nPlease LogIn.", DialogCallback);
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
