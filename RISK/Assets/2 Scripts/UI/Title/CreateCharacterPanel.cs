using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateCharacterPanel : MonoBehaviour
{
    [SerializeField]
    private List<ClassNameToCharacterData> classDataList;

    private Dictionary<ClassName, CharacterData> diccd;

    public Image characterModelImage;
    public Image characterImage;
    public TMP_InputField nickNameInputfield;
    public Button createButton;
    public Button closeButton;

    private void Awake()
    {
        // List를 Dictionary로 변환
        diccd = new Dictionary<ClassName, CharacterData>();
        foreach (var entry in classDataList)
        {
            if (!diccd.ContainsKey(entry.className))
            {
                diccd.Add(entry.className, entry.characterData);
            }
        }

        createButton.onClick.AddListener(OnCreateButtonClick);
        closeButton.onClick.AddListener(OnCloseButtonClick);
    }

    private void OnCloseButtonClick()
    {
        PanelManager.Instance.PanelOpen("SelectCharacter");
    }

    private void OnCreateButtonClick()
    {
        // 캐릭터 생성 로직
    }
}


public enum ClassName
{
    None,
    Warrior,
    SpearMan,
    Archer,
    Mage,
}
[Serializable]
public class CharacterData
{
    public string description;
    public string characteristic;
}
public class FireBaseCharacterData
{
    public string nickName = null;
    public int className;
    public string maxHp;
    public string moveSpeed;

}

[Serializable]
public class ClassNameToCharacterData
{
    public ClassName className;
    public CharacterData characterData;
}

