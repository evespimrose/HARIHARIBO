using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectCharacterPanel : MonoBehaviour
{
    public Button deleteButton;
    public Button closeButton;

    public Button selectButton;
    public GameObject createCharacterPrefab;
    public GameObject characterDataPrefab;

    public Image characterModelImage;

    public Transform characterListTransform;

    public Dictionary<string, GameObject> characterSelectDic = new Dictionary<string, GameObject>();
    List<FireBaseCharacterData> characterDatalist = new List<FireBaseCharacterData>();

    public FireBaseCharacterData currentCharacterData;

    public TextMeshProUGUI levelText;
    public TextMeshProUGUI nickNameText;

    private void Awake()
    {
        selectButton.onClick.AddListener(OnSelectButtonClick);
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
        characterSelectDic.Clear();

        characterDatalist = await FirebaseManager.Instance.LoadCharacterDataList();

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
                        // TODO : class image load
                    }
                    if (characterData.TryGetComponent(out Button databutton))
                    {
                        databutton.onClick.AddListener(() =>
                        {
                            levelText.text = characterSelectButton.levelText.text;
                            nickNameText.text = characterSelectButton.nickNameText.text;
                            currentCharacterData = data;
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
        }
    }

    private void OnSelectButtonClick()
    {
        FirebaseManager.Instance.currentCharacterData = currentCharacterData;

        Playerstats playerStats = new Playerstats
        {
            nickName = FirebaseManager.Instance.currentCharacterData.nickName,
            level = FirebaseManager.Instance.currentCharacterData.level,
            maxHealth = FirebaseManager.Instance.currentCharacterData.maxHp,
            currentHealth = FirebaseManager.Instance.currentCharacterData.maxHp,
            moveSpeed = 2f
        };

        SceneManager.LoadScene("LobbyScene", LoadSceneMode.Single);

        StartCoroutine(InstantiatePlayer(playerStats));
    }

    private IEnumerator InstantiatePlayer(Playerstats playerStats)
    {
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "LobbyScene");

        Vector3 spawnPosition = Vector3.zero;
        GameObject playerObj = PhotonNetwork.Instantiate("Player", spawnPosition, Quaternion.identity);
        Player player = playerObj.GetComponent<Player>();
        player.InitializeStats(playerStats);

        UnitManager.Instance.RegisterPlayer(playerObj);
    }

}
