using Newtonsoft.Json;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectCharacterPanel : MonoBehaviour
{
    public Dictionary<string, GameObject> characterSelectDic = new Dictionary<string, GameObject>();
    private List<FireBaseCharacterData> characterDatalist = new List<FireBaseCharacterData>();
    [SerializeField] List<Sprite> classSpriteList = new List<Sprite>();

    public FireBaseCharacterData currentCharacterData;

    public GameObject createCharacterPrefab;
    public GameObject characterDataPrefab;

    public Transform characterListTransform;

    public Button deleteButton;
    public Button closeButton;
    public Button selectButton;

    public Image characterModelImage;

    public TextMeshProUGUI levelText;
    public TextMeshProUGUI nickNameText;

    public bool isOnceSelected;

    private void Awake()
    {
        selectButton.onClick.AddListener(() => { StartCoroutine(OnSelectButtonClick()); });
        deleteButton.onClick.AddListener(OnDeleteButtonClick);
        closeButton.onClick.AddListener(OnCloseButtonClick);
    }

    private void OnEnable()
    {
        ReLoadCharacterList();
    }

    private void OnCreateCharacterButtonClick()
    {
        PanelManager.Instance.PanelOpen("CreateCharacter");
    }

    private void OnDisable()
    {
        foreach (var character in characterSelectDic)
        {
            DestroyImmediate(character.Value);
        }
        characterSelectDic.Clear();
    }

    private void OnCloseButtonClick()
    {
        PanelManager.Instance.PopupOpen<TwoButtonPopupPanel>().SetPopup("SignOut", "sign out?",
            ok =>
        {
            if (ok)
            {
                FirebaseManager.Instance.SignOut();
                PanelManager.Instance.PanelOpen("Login");
            }
            else
                PanelManager.Instance.PopupClose();
        });
    }

    private void OnDeleteButtonClick()
    {
        PanelManager.Instance.PopupOpen<TwoButtonPopupPanel>().SetPopup("DeleteCharacter", "Delete Character?",
           ok =>
           {
               if (ok)
               {
                   FirebaseManager.Instance.DeleteCharacterData(currentCharacterData.nickName, ReLoadCharacterList);
               }
               else
                   PanelManager.Instance.PopupClose();
           });
    }

    public async void ReLoadCharacterList()
    {
        foreach (var character in characterSelectDic)
        {
            DestroyImmediate(character.Value);
        }
        characterSelectDic.Clear();

        characterDatalist = await FirebaseManager.Instance.LoadCharacterDataList();

        Dictionary<ClassType, CharacterData> characterdatadic = GameManager.Instance.characterDataDic;

        if (characterDatalist != null)
        {
            foreach (var data in characterDatalist)
            {
                if (!characterSelectDic.ContainsKey(data.nickName))
                {
                    GameObject characterData = Instantiate(characterDataPrefab, characterListTransform);
                    if (characterData.TryGetComponent(out CharacterSelectButton characterSelectButton))
                    {
                        characterSelectButton.nickNameText.text = data.nickName;
                        characterSelectButton.levelText.text = data.level.ToString();
                        characterSelectButton.characterImage.sprite = characterdatadic[data.classType].headSprite;
                    }
                    if (characterData.TryGetComponent(out Button databutton))
                    {
                        databutton.onClick.AddListener(() =>
                        {
                            levelText.text = characterSelectButton.levelText.text;
                            nickNameText.text = characterSelectButton.nickNameText.text;
                            currentCharacterData = data;
                            characterModelImage.sprite = characterdatadic[data.classType].sprite;
                            isOnceSelected = true;
                        });
                    }
                    characterSelectDic.Add(data.nickName, characterData);
                }
            }
        }

        GameObject createCharacterButton = Instantiate(createCharacterPrefab, characterListTransform);

        if (createCharacterButton.TryGetComponent(out Button button))
        {
            button.onClick.AddListener(OnCreateCharacterButtonClick);
        }

        characterSelectDic.Add("create", createCharacterButton);

        if (characterDatalist.Count > 0)
        {
            currentCharacterData = characterDatalist[0];

            levelText.text = currentCharacterData.level.ToString();
            nickNameText.text = currentCharacterData.nickName;
            characterModelImage.sprite = GameManager.Instance.characterDataDic[currentCharacterData.classType].sprite;
        }
    }

    private IEnumerator OnSelectButtonClick()
    {
        if (!isOnceSelected) yield break;
        FirebaseManager.Instance.currentCharacterData = currentCharacterData;

        PhotonNetwork.ConnectUsingSettings();

        string Json = JsonConvert.SerializeObject(currentCharacterData);

        print(Json);

        PhotonNetwork.NickName = Json;

        yield return new WaitUntil(() => PhotonNetwork.IsConnected);

        PhotonNetwork.LocalPlayer.NickName = Json;

        PanelManager.Instance.PanelOpen("PartyListBoard");
    }

}
