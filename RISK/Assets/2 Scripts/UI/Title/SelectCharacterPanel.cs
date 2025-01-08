using System;
using System.Collections;
using System.Collections.Generic;
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

    public FireBaseCharacterData currentCharacterData;


    private void Awake()
    {
        selectButton.onClick.AddListener(OnSelectButtonClick);
        deleteButton.onClick.AddListener(OnDeleteButtonClick);
        closeButton.onClick.AddListener(OnCloseButtonClick);
    }

    private async void OnEnable()
    {
        List<FireBaseCharacterData> list = await FirebaseManager.Instance.LoadCharacterDataList();
        
        if(list != null)
        {
            foreach (var data in list)
            {
                if (!characterSelectDic.ContainsKey(data.nickName))
                {
                    GameObject characterData = Instantiate(characterDataPrefab, characterListTransform);
                    if (characterData.TryGetComponent(out CharacterSelectButton characterSelectButton))
                    {
                        characterSelectButton.nickNameText.text = data.nickName;
                        characterSelectButton.levelText.text = data.level.ToString();
                        // 추후 클래스 타입에 따라서 캐릭터 이미지 업로드
                    }
                    if (characterData.TryGetComponent(out Button databutton))
                    {
                        //databutton.onClick.AddListener();
                    }
                    characterSelectDic.Add(data.nickName, characterData);
                }
            }
        }
        GameObject createCharacterButton = Instantiate(createCharacterPrefab, characterListTransform);

        if(createCharacterButton.TryGetComponent(out Button button))
        {
            button.onClick.AddListener(OnCreateCharacterButtonClick);
        }
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
