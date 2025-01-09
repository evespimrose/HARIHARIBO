using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

    private async void OnEnable()
    {
        GameObject createCharacterButton = Instantiate(createCharacterPrefab, characterListTransform);

        if (createCharacterButton.TryGetComponent(out Button button))
        {
            button.onClick.AddListener(OnCreateCharacterButtonClick);
        }

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
        currentCharacterData = characterDatalist[0];

        levelText.text = currentCharacterData.level.ToString();
        nickNameText.text = characterSelectButton.nickNameText.text;
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

    }

    private void OnDeleteButtonClick()
    {

    }

    private void OnSelectButtonClick()
    {


    }


}
