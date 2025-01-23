using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Linq;
using UnityEditor;

public class FirebaseManager : SingletonManager<FirebaseManager>
{
    public FirebaseApp App { get; private set; }
    public FirebaseAuth Auth { get; private set; }
    public FirebaseDatabase DB { get; private set; }

    private DatabaseReference usersRef;

    public FireBaseUserData currentUserData { get; set; }

    public FireBaseCharacterData currentCharacterData;

    private async void Start()
    {
        DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (status == DependencyStatus.Available)
        {
            App = FirebaseApp.DefaultInstance;
            Auth = FirebaseAuth.DefaultInstance;
            DB = FirebaseDatabase.DefaultInstance;
        }
        else
        {
            PanelManager.Instance.PopupOpen<PopupPanel>().SetPopup("Error", "DependencyStatus.UnAvailable.\n");
        }

    }

    public async void Create(string email, string passwd, Action<FirebaseUser> callback = null)
    {
        try
        {
            var result = await Auth.CreateUserWithEmailAndPasswordAsync(email, passwd);
            usersRef = DB.GetReference($"users/{result.User.UserId}");

            // ???쐻??덉굲?醫롫짗???醫롫짗??용쐻??덉굲?醫롫뼟筌뤿슣??Database?醫롫짗???醫롫짗??용쐻??덉굲
            FireBaseUserData userData = new FireBaseUserData(result.User.UserId);

            string userDataJson = JsonConvert.SerializeObject(userData);

            await usersRef.SetRawJsonValueAsync(userDataJson);

            callback?.Invoke(result.User);
        }
        catch (FirebaseException e)
        {
            PanelManager.Instance.PopupOpen<PopupPanel>().SetPopup("Auth Failed", "" + e.Message);
        }
    }
    public async void SignIn(string email, string passwd, Action<FirebaseUser> callback = null)
    {
        try
        {
            var result = await Auth.SignInWithEmailAndPasswordAsync(email, passwd);

            usersRef = DB.GetReference($"users/{result.User.UserId}");

            DataSnapshot userDataValues = await usersRef.GetValueAsync();
            FireBaseUserData userData = null;
            if (userDataValues.Exists)
            {
                string json = userDataValues.GetRawJsonValue();
                userData = JsonConvert.DeserializeObject<FireBaseUserData>(json);
            }
            currentUserData = userData;
            callback?.Invoke(result.User);
        }
        catch (FirebaseException e)
        {
            PanelManager.Instance.PopupOpen<TwoButtonPopupPanel>().SetPopup("Auth Failed", "ID or PW is Wrong.\n" + e.Message,
                ok =>
                {
                    if (ok)
                    {
                        CopyToClipboard(e.Message);
                    }
                    else
                        PanelManager.Instance.PopupClose();
                }
                );
        }
    }

    public async void EmailDuplicationCheck(string email, Action<bool> callback)
    {
        try
        {
            var providers = await Auth.FetchProvidersForEmailAsync(email);

            bool isDuplicate = providers != null && providers.Any();
            callback?.Invoke(isDuplicate);
        }
        catch (FirebaseException e)
        {
            PanelManager.Instance.PopupOpen<TwoButtonPopupPanel>().SetPopup("Error", "Failed to check duplication.\n" + e.Message,
                ok =>
                {
                    if (ok)
                    {
                        CopyToClipboard(e.Message);
                    }
                    else
                        PanelManager.Instance.PopupClose();
                }
                );
            callback?.Invoke(false);
        }
    }

    public async void CharacterDuplicationCheck(string nickName, Action<bool> callback = null)
    {
        try
        {
            DatabaseReference charactersRef = DB.GetReference("characters");
            DataSnapshot snapshot = await charactersRef.GetValueAsync();

            bool isDuplicate = false;
            if (snapshot.Exists)
            {
                foreach (var userSnapshot in snapshot.Children)
                {
                    foreach (var childSnapshot in userSnapshot.Children)
                    {
                        string characterNickName = childSnapshot.GetValue(true).ToString();
                        if (characterNickName.Equals(nickName, StringComparison.OrdinalIgnoreCase))
                        {
                            isDuplicate = true;
                            break;
                        }
                    }
                    if (isDuplicate) break;
                }
            }
            print(isDuplicate);
            callback?.Invoke(isDuplicate);
        }
        catch (Exception e)
        {
            PanelManager.Instance.PopupOpen<TwoButtonPopupPanel>().SetPopup("Error", "Character Duplication Check Failed.\n" + e.Message,
                ok =>
                {
                    if (ok)
                    {
                        CopyToClipboard(e.Message);
                    }
                    else
                        PanelManager.Instance.PopupClose();
                }
                );
            callback?.Invoke(false);
        }
    }

    public async void CreateCharacter(string nickName, ClassType classType)
    {
        try
        {
            Debug.Log($"Creating character: {nickName}, Class: {classType}");
            FireBaseCharacterData characterData = new FireBaseCharacterData(nickName, classType);

            string prefabPath = classType.ToString();
            Debug.Log($"Loading prefab from path: {prefabPath}");

            // 프리팹에서 기본 스탯 가져오기
            GameObject prefab = Resources.Load<GameObject>(prefabPath);

            if (prefab != null)
            {
                Debug.Log($"Failed to load prefab for class type: {classType}");

                Debug.Log($"Prefab loaded successfully: {prefab.name}");
                switch (classType)
                {
                    case ClassType.Warrior:
                        Warrior warrior = prefab.GetComponent<Warrior>();
                        if (warrior != null)
                        {
                            Debug.Log($"Warrior component found. MaxHealth: {warrior.baseMaxHealth}");
                            characterData.maxHp = warrior.baseMaxHealth;
                            characterData.atk = warrior.baseAttackPower;
                            characterData.cri = warrior.baseCriticalChance;
                            characterData.criDmg = warrior.baseCriticalDamage;
                            characterData.dmgRed = warrior.baseDamageReduction;
                            characterData.hpReg = warrior.baseHealthRegen;
                            characterData.coolRed = warrior.baseCooldownReduction;
                        }
                        break;

                    case ClassType.Mage:
                        Mage mage = prefab.GetComponent<Mage>();
                        if (mage != null)
                        {
                            characterData.maxHp = mage.baseMaxHealth;
                            characterData.atk = mage.baseAttackPower;
                            characterData.cri = mage.baseCriticalChance;
                            characterData.criDmg = mage.baseCriticalDamage;
                            characterData.dmgRed = mage.baseDamageReduction;
                            characterData.hpReg = mage.baseHealthRegen;
                            characterData.coolRed = mage.baseCooldownReduction;
                        }
                        break;

                    case ClassType.Healer:
                        Healer healer = prefab.GetComponent<Healer>();
                        if (healer != null)
                        {
                            characterData.maxHp = healer.baseMaxHealth;
                            characterData.atk = healer.baseAttackPower;
                            characterData.cri = healer.baseCriticalChance;
                            characterData.criDmg = healer.baseCriticalDamage;
                            characterData.dmgRed = healer.baseDamageReduction;
                            characterData.hpReg = healer.baseHealthRegen;
                            characterData.coolRed = healer.baseCooldownReduction;
                        }
                        break;

                    case ClassType.Destroyer:
                        Destroyer destroyer = prefab.GetComponent<Destroyer>();
                        if (destroyer != null)
                        {
                            characterData.maxHp = destroyer.baseMaxHealth;
                            characterData.atk = destroyer.baseAttackPower;
                            characterData.cri = destroyer.baseCriticalChance;
                            characterData.criDmg = destroyer.baseCriticalDamage;
                            characterData.dmgRed = destroyer.baseDamageReduction;
                            characterData.hpReg = destroyer.baseHealthRegen;
                            characterData.coolRed = destroyer.baseCooldownReduction;
                        }
                        break;
                }
            }

            string characterDataJson = JsonConvert.SerializeObject(characterData);
            DatabaseReference charactersRef = DB.GetReference($"characters/{Auth.CurrentUser.UserId}");
            await charactersRef.Child(nickName).SetRawJsonValueAsync(characterDataJson);
            PanelManager.Instance.PopupOpen<PopupPanel>().SetPopup("Success", "character creation successed.", () => PanelManager.Instance.PanelOpen("SelectCharacter"));
        }
        catch (Exception e)
        {
            PanelManager.Instance.PopupOpen<TwoButtonPopupPanel>().SetPopup("Error", "character creation failed.\n" + e.Message,
               ok =>
               {
                   if (ok)
                       CopyToClipboard(e.Message);
                   else
                       PanelManager.Instance.PopupClose();
               }
               );
        }
    }

    private float GetSerializedPropertyValue(SerializedObject serializedObject, string propertyName)
    {
        var property = serializedObject.FindProperty(propertyName);
        return property != null ? property.floatValue : 0f;
    }


    public async Task<List<FireBaseCharacterData>> LoadCharacterDataList()
    {
        List<FireBaseCharacterData> result = new List<FireBaseCharacterData>();
        try
        {
            DatabaseReference charactersRef = DB.GetReference($"characters/{Auth.CurrentUser.UserId}");

            DataSnapshot snapshot = await charactersRef.GetValueAsync();

            if (snapshot.Exists)
            {
                foreach (var childSnapshot in snapshot.Children)
                {
                    string json = childSnapshot.GetRawJsonValue();
                    FireBaseCharacterData characterData = JsonConvert.DeserializeObject<FireBaseCharacterData>(json);
                    result.Add(characterData);
                }
            }
        }
        catch (Exception e)
        {
            PanelManager.Instance.PopupOpen<TwoButtonPopupPanel>().SetPopup("Error", "LoadCharacterDataList failed.\n" + e.Message,
                ok =>
                {
                    if (ok)
                        CopyToClipboard(e.Message);
                    else
                        PanelManager.Instance.PopupClose();
                }
                );
        }

        return result;
    }

    public async void UpgradeCharacter(string dataName,  float statIncrease, Action onSuccess)
    {
        if (currentCharacterData != null)
        {
            try
            {
                DatabaseReference characterRef = FirebaseManager.Instance.DB.GetReference($"characters/{FirebaseManager.Instance.Auth.CurrentUser.UserId}/{currentCharacterData.nickName}");

                switch (dataName)
                {
                    case "maxHp":
                        Debug.Log($"MaxHp 강화 {dataName}, {statIncrease}, {currentCharacterData.maxHpUpgradeLevel}");
                        await characterRef.Child(dataName).SetValueAsync(currentCharacterData.maxHp + statIncrease);
                        Debug.Log($"데이터 업데이트 완료: {dataName} -> {currentCharacterData.maxHp + statIncrease}");
                        currentCharacterData.maxHp += statIncrease;
                        currentCharacterData.maxHpUpgradeLevel++;
                        await characterRef.Child("maxHpUpgradeLevel").SetValueAsync(currentCharacterData.maxHpUpgradeLevel);
                        Debug.Log($"{currentCharacterData.maxHpUpgradeLevel}");
                        break;
                    case "atk":
                        Debug.Log($"atk 강화 {dataName}, {statIncrease}, {currentCharacterData.atkUpgradeLevel}");
                        await characterRef.Child(dataName).SetValueAsync(currentCharacterData.atk + statIncrease);
                        Debug.Log($"데이터 업데이트 완료: {dataName} -> {currentCharacterData.atk + statIncrease}");
                        currentCharacterData.atk += statIncrease;
                        currentCharacterData.atkUpgradeLevel++;
                        await characterRef.Child("atkUpgradeLevel").SetValueAsync(currentCharacterData.atkUpgradeLevel);
                        Debug.Log($"{currentCharacterData.atkUpgradeLevel}");
                        break;
                    case "cri":
                        Debug.Log($"cri 강화 {dataName}, {statIncrease}, {currentCharacterData.criUpgradeLevel}");
                        await characterRef.Child(dataName).SetValueAsync(currentCharacterData.cri + statIncrease);
                        Debug.Log($"데이터 업데이트 완료: {dataName} -> {currentCharacterData.cri + statIncrease}");
                        currentCharacterData.cri += statIncrease;
                        currentCharacterData.criUpgradeLevel++;
                        await characterRef.Child("criUpgradeLevel").SetValueAsync(currentCharacterData.criUpgradeLevel);
                        Debug.Log($"{currentCharacterData.criUpgradeLevel}");
                        break;
                    case "criDmg":
                        Debug.Log($"criDmg 강화 {dataName}, {statIncrease}, {currentCharacterData.criDmgUpgradeLevel}");
                        await characterRef.Child(dataName).SetValueAsync(currentCharacterData.criDmg + statIncrease);
                        Debug.Log($"데이터 업데이트 완료: {dataName} -> {currentCharacterData.criDmg + statIncrease}");
                        currentCharacterData.criDmg += statIncrease;
                        currentCharacterData.criDmgUpgradeLevel++;
                        await characterRef.Child("criDmgUpgradeLevel").SetValueAsync(currentCharacterData.criDmgUpgradeLevel);
                        Debug.Log($"{currentCharacterData.criDmgUpgradeLevel}");
                        break;
                    case "hpReg":
                        Debug.Log($"hpReg 강화 {dataName}, {statIncrease}, {currentCharacterData.hpRegUpgradeLevel}");
                        await characterRef.Child(dataName).SetValueAsync(currentCharacterData.hpReg + statIncrease);
                        Debug.Log($"데이터 업데이트 완료: {dataName} -> {currentCharacterData.hpReg + statIncrease}");
                        currentCharacterData.hpReg += statIncrease;
                        currentCharacterData.hpRegUpgradeLevel++;
                        await characterRef.Child("hpRegUpgradeLevel").SetValueAsync(currentCharacterData.hpRegUpgradeLevel);
                        Debug.Log($"{currentCharacterData.hpRegUpgradeLevel}");
                        break;
                    case "coolRed":
                        Debug.Log($"coolRed 강화 {dataName}, {statIncrease}, {currentCharacterData.coolRedUpgradeLevel}");
                        await characterRef.Child(dataName).SetValueAsync(currentCharacterData.coolRed + statIncrease);
                        Debug.Log($"데이터 업데이트 완료: {dataName} -> {currentCharacterData.coolRed + statIncrease}");
                        currentCharacterData.coolRed += statIncrease;
                        currentCharacterData.coolRedUpgradeLevel++;
                        await characterRef.Child("coolRedUpgradeLevel").SetValueAsync(currentCharacterData.coolRedUpgradeLevel);
                        Debug.Log($"{currentCharacterData.coolRedUpgradeLevel}");
                        break;
                    default:
                        Debug.LogWarning($"Unknown dataName: {dataName}");
                        break;
                }

                await usersRef.Child("won").SetValueAsync(currentUserData.won);

                var snapshot = await characterRef.Child(dataName).GetValueAsync();
                Debug.Log($"저장된 값 확인: {snapshot.Value}");

                // 성공 후 콜백 호출
                onSuccess?.Invoke();  // 여기서 UpdateStatsFromServer을 콜백으로 호출합니다.

                PanelManager.Instance.PopupOpen<PopupPanel>().SetPopup("Success", "Character upgraded successfully.");
            }
            catch (Exception e)
            {
                PanelManager.Instance.PopupOpen<TwoButtonPopupPanel>().SetPopup("Error", "Character upgrade failed.\n" + e.Message,
               ok =>
               {
                   if (ok)
                       CopyToClipboard(e.Message);
                   else
                       PanelManager.Instance.PopupClose();
               }
               );
            }
        }
    }
    public void SignOut()
    {
        Auth.SignOut();
    }

    public async void DeleteCharacterData(string nickName, Action callback)
    {
        try
        {
            DatabaseReference characterRef = DB.GetReference($"characters/{Auth.CurrentUser.UserId}/{nickName}");
            await characterRef.RemoveValueAsync();
            callback?.Invoke();
        }
        catch (Exception e)
        {
            PanelManager.Instance.PopupOpen<TwoButtonPopupPanel>().SetPopup("Error", "Character deletion failed.\n" + e.Message,
              ok =>
              {
                  if (ok)
                      CopyToClipboard(e.Message);
                  else
                      PanelManager.Instance.PopupClose();
              }
              );

        }
    }

    public void CopyToClipboard(string textToCopy)
    {
        GUIUtility.systemCopyBuffer = textToCopy;
    }
}
