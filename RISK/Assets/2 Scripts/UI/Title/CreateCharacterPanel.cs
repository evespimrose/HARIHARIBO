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

    private Dictionary<ClassType, CharacterData> characterDataDic = new Dictionary<ClassType, CharacterData>();

    public Image characterModelImage;
    public Image characterImage;
    public TMP_InputField nickNameInputfield;
    private ClassType currentClassType;
    public Button createButton;
    public Button closeButton;

    private void Awake()
    {
        foreach (var entry in classDataList)
        {
            if (!characterDataDic.ContainsKey(entry.classType))
            {
                characterDataDic.Add(entry.classType, entry.characterData);
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
        FirebaseManager.Instance.CreateCharacter(nickNameInputfield.text, currentClassType);
    }
}


public enum ClassType
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

[Serializable]
public class ClassNameToCharacterData
{
    public ClassType classType;
    public CharacterData characterData;
}

[Serializable]
public class FireBaseUserData
{
    public string userId { get; set; }
    public int won;

    public FireBaseUserData() { }
    public FireBaseUserData(string userId)
    {
        this.userId = userId;
        won = 0;
    }
    public FireBaseUserData(string userId, int won)
    {
        this.userId = userId;
        this.won = won;
    }
}
[Serializable]
public class FireBaseCharacterData
{
    public string nickName = null;
    public ClassType classType;
    public int level;
    public int exp;
    public int maxHp;
    public int moveSpeed;

    public FireBaseCharacterData() { }
    public FireBaseCharacterData(string nickName, ClassType classType)
    {
        this.classType = classType;
        this.nickName = nickName;
        level = 1;
        exp = 0;
        maxHp = 0;
        moveSpeed = 0;
    }
}