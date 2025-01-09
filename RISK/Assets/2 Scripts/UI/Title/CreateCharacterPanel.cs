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

    public GameObject characterInfoPrefab;
    public Transform characterInfoListTransform;

    public TextMeshProUGUI currentClassNameText;
    public TextMeshProUGUI currentClassDescriptionText;
    public TextMeshProUGUI currentClassPropertyText;


    public Action<ClassType> UpdateInfo;

    private void Awake()
    {
        foreach (var classdata in classDataList)
        {
            if (!characterDataDic.ContainsKey(classdata.classType))
            {
                characterDataDic.Add(classdata.classType, classdata.characterData);

                GameObject characterInfo = Instantiate(characterInfoPrefab, characterInfoListTransform);
                if (characterInfo.TryGetComponent(out CharacterInfoUI characterInfoUI))
                {
                    characterInfoUI.classType = classdata.classType;

                    if (characterInfo.TryGetComponent(out Button button))
                    {
                        button.onClick.AddListener(() =>
                        {
                            currentClassType = characterInfoUI.classType;

                            currentClassNameText.text = currentClassType.ToString();
                        });
                    }
                }
            }
        }
        currentClassType = ClassType.Warrior;
        print(currentClassType.ToString());
        currentClassNameText.text = currentClassType.ToString();

        createButton.onClick.AddListener(OnCreateButtonClick);
        closeButton.onClick.AddListener(OnCloseButtonClick);
        UpdateInfo += SwapInfoText;
    }

    private void SwapInfoText(ClassType classType)
    {
        characterDataDic.TryGetValue(classType, out CharacterData cd);

    }

    private void OnCloseButtonClick()
    {
        PanelManager.Instance.PanelOpen("SelectCharacter");
    }

    private void OnCreateButtonClick()
    {
        if (false == string.IsNullOrEmpty(nickNameInputfield.text))
        {
            FirebaseManager.Instance.CharacterDuplicationCheck(nickNameInputfield.text, (bool result) => FirebaseManager.Instance.CreateCharacter(nickNameInputfield.text, currentClassType));
            return;

        }
        PanelManager.Instance.PopupOpen<PopupPanel>().SetPopup("Error", "Please fill in nickname space.\n");
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
    public float maxHp;
    public int moveSpeed;

    public FireBaseCharacterData() { }
    public FireBaseCharacterData(string nickName, ClassType classType)
    {
        this.classType = classType;
        this.nickName = nickName;
        level = 1;
        exp = 0;
        maxHp = 0f;
        moveSpeed = 0;
    }
}