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

            // ?뚦뜝?숈삕?좎룞???좎룞?쇿뜝?숈삕?좎떢紐뚯삕 Database?좎룞???좎룞?쇿뜝?숈삕
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

            bool isDuplicate = providers == null || !providers.Any();
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
            FireBaseCharacterData characterData = new FireBaseCharacterData(nickName, classType);

            // 직업별 초기 스탯 설정
            GameObject prefab = Resources.Load<GameObject>(classType.ToString());

            if (prefab != null)
            {
                var playerComponent = prefab.GetComponent<Player>();
                if (playerComponent != null)
                {
                    // 각 직업별 컴포넌트에서 기본 스탯 가져오기
                    SerializedObject serializedObject = new SerializedObject(playerComponent);
                    characterData.maxHp = GetSerializedPropertyValue(serializedObject, "baseMaxHealth");
                    characterData.atk = GetSerializedPropertyValue(serializedObject, "baseAttackPower");
                    characterData.cri = GetSerializedPropertyValue(serializedObject, "baseCriticalChance");
                    characterData.criDmg = GetSerializedPropertyValue(serializedObject, "baseCriticalDamage");
                    characterData.dmgRed = GetSerializedPropertyValue(serializedObject, "baseDamageReduction");
                    characterData.hpReg = GetSerializedPropertyValue(serializedObject, "baseHealthRegen");
                    characterData.coolRed = GetSerializedPropertyValue(serializedObject, "baseCooldownReduction");
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

    public async void UpgradeCharacter(string dataName)
    {
        if (currentCharacterData != null)
        {
            try
            {
                DatabaseReference characterRef = FirebaseManager.Instance.DB.GetReference($"characters/{FirebaseManager.Instance.Auth.CurrentUser.UserId}/{currentCharacterData.nickName}");

                switch (dataName)
                {
                    case "maxHp":
                        await characterRef.Child(dataName).SetValueAsync(currentCharacterData.maxHp + 1);
                        currentCharacterData.maxHp += 1;
                        break;
                    case "atk":
                        await characterRef.Child(dataName).SetValueAsync(currentCharacterData.atk + 1);
                        currentCharacterData.atk += 1;
                        break;
                    case "cri":
                        await characterRef.Child(dataName).SetValueAsync(currentCharacterData.cri + 1);
                        currentCharacterData.cri += 1;
                        break;
                    case "criDmg":
                        await characterRef.Child(dataName).SetValueAsync(currentCharacterData.criDmg + 1);
                        currentCharacterData.criDmg += 1;
                        break;
                    case "dmgRed":
                        await characterRef.Child(dataName).SetValueAsync(currentCharacterData.dmgRed + 1);
                        currentCharacterData.dmgRed += 1;
                        break;
                    case "hpReg":
                        await characterRef.Child(dataName).SetValueAsync(currentCharacterData.hpReg + 1);
                        currentCharacterData.hpReg += 1;
                        break;
                    case "coolRed":
                        await characterRef.Child(dataName).SetValueAsync(currentCharacterData.coolRed + 1);
                        currentCharacterData.coolRed += 1;
                        break;
                    default:
                        Debug.LogWarning($"Unknown dataName: {dataName}");
                        break;
                }

                LobbyUI.Instance.PopupOpen<PopupPanel>().SetPopup("Success", "Character upgraded successfully.");
            }
            catch (Exception e)
            {
                LobbyUI.Instance.PopupOpen<TwoButtonPopupPanel>().SetPopup("Error", "Character upgrade failed.\n" + e.Message,
               ok =>
               {
                   if (ok)
                       CopyToClipboard(e.Message);
                   else
                       LobbyUI.Instance.PopupClose();
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
