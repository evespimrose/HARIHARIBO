using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
    public TextMeshProUGUI currentClassCharacteristicText;


    public Action<ClassType> UpdateInfo;

    private void Awake()
    {
        UpdateInfo += SwapInfoText;

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

                            UpdateInfo?.Invoke(currentClassType);
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
    }

    private void SwapInfoText(ClassType classType)
    {
        characterDataDic.TryGetValue(classType, out CharacterData cd);
        currentClassDescriptionText.text = cd.description;
        currentClassCharacteristicText.text = cd.characteristic;
        currentClassNameText.text = classType.ToString();
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
    Destroyer,
    Healer,
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
    public int hPperLv;
    public int atkperLv;
    public float maxExp;
    public float currExp;

    public float maxHp;
    public float moveSpeed;
    public float atk;
    public float dmgRed;
    public float hpReg;
    public float regInt;
    public float cri;
    public float criDmg;
    public float coolRed;

    public int maxHpUpgradeLevel;
    public int atkUpgradeLevel;
    public int criUpgradeLevel;
    public int criDmgUpgradeLevel;
    public int hpRegUpgradeLevel;
    public int coolRedUpgradeLevel;

    public FireBaseCharacterData() { }
    public FireBaseCharacterData(string nickName, ClassType classType)
    {
        this.classType = classType;
        this.nickName = nickName;
        switch (classType)
        {
            case ClassType.Warrior:
                {
                    hPperLv = 0;
                    atkperLv = 0;
                    moveSpeed = 2f;
                    atk = 10f;
                    dmgRed = 0;
                    hpReg = 0;
                    regInt = 0;
                    break;
                }
            case ClassType.Destroyer:
                {
                    hPperLv = 0;
                    atkperLv = 0;
                    moveSpeed = 2f;
                    atk = 10f;
                    dmgRed = 0;
                    hpReg = 0;
                    regInt = 0;
                    break;
                }
            case ClassType.Healer:
                {
                    hPperLv = 0;
                    atkperLv = 0;
                    moveSpeed = 2f;
                    atk = 10f;
                    dmgRed = 0;
                    hpReg = 0;
                    regInt = 0;
                    break;
                }
            case ClassType.Mage:
                {
                    hPperLv = 0;
                    atkperLv = 0;
                    moveSpeed = 2f;
                    atk = 10f;
                    dmgRed = 0;
                    hpReg = 0;
                    regInt = 0;
                    break;
                }
        }
        level = 1;
        maxExp = 100f;
        currExp = 0f;
        maxHp = 0f;
        cri = 0f;
        criDmg = 0f;
        coolRed = 0f;

        maxHpUpgradeLevel = 0;
        atkUpgradeLevel = 0;
        criUpgradeLevel = 0;
        criDmgUpgradeLevel = 0;
        hpRegUpgradeLevel = 0;
        coolRedUpgradeLevel = 0;
    }
}
