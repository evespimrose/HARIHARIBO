using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Photon.Pun;
using System.Linq;

public class FirebaseManager : SingletonManager<FirebaseManager>
{
    public FirebaseApp App { get; private set; }
    public FirebaseAuth Auth { get; private set; }
    public FirebaseDatabase DB { get; private set; }

    private DatabaseReference usersRef;

    //public UserData currentUserData { get; private set; }

    private async void Start()
    {
        //���̾�̽� �ʱ�ȭ ���� üũ. �񵿱�(Async)�Լ��̹Ƿ� �Ϸ�� �� ���� ���
        DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync();
        //�ʱ�ȭ ����
        if (status == DependencyStatus.Available)
        {
            App = FirebaseApp.DefaultInstance;
            Auth = FirebaseAuth.DefaultInstance;
            DB = FirebaseDatabase.DefaultInstance;
        }
        //�ʱ�ȭ ����
        else
        {
            Debug.LogWarning($"���̾�̽� �ʱ�ȭ ���� : {status}");
        }

    }

    public async void Create(string email, string passwd, Action<FirebaseUser> callback = null)
    {
        try
        {
            var result = await Auth.CreateUserWithEmailAndPasswordAsync(email, passwd);
            usersRef = DB.GetReference($"users/{result.User.UserId}");

            // ȸ���� �����͸� Database�� ����
            //UserData userData = new UserData(result.User.UserId);

            //string userDataJson = JsonConvert.SerializeObject(userData);

            //await usersRef.SetRawJsonValueAsync(userDataJson);

            callback?.Invoke(result.User);
        }
        catch (FirebaseException e)
        {
            PanelManager.Instance.PopupOpen<PopupPanel>().SetPopup("Auth Failed", "" + e.Message);
        }
    }

    //�α���
    public async void SignIn(string email, string passwd, Action<FirebaseUser> callback = null)
    {
        try
        {
            var result = await Auth.SignInWithEmailAndPasswordAsync(email, passwd);

            usersRef = DB.GetReference($"users/{result.User.UserId}");

            // ������ �ҷ�����
            //DataSnapshot userDataValues = await usersRef.GetValueAsync();
            //UserData userData = null;
            //if (userDataValues.Exists)
            //{
            //    string json = userDataValues.GetRawJsonValue();
            //    userData = JsonConvert.DeserializeObject<UserData>(json);
            //}
            //currentUserData = userData;
            callback?.Invoke(result.User);
        }
        catch (FirebaseException e)
        {
            PanelManager.Instance.PopupOpen<PopupPanel>().SetPopup("Auth Failed", "ID or PW is Wrong.\n" + e.Message);
        }
    }

    public async void DuplicationCheck(string email, Action<bool> callback)
    {
        try
        {
            var providers = await Auth.FetchProvidersForEmailAsync(email);

            bool isDuplicate = providers != null && providers.Any();
            callback?.Invoke(isDuplicate);
        }
        catch (FirebaseException e)
        {
            Debug.LogError($"Duplication Check Failed: {e.Message}");
            PanelManager.Instance.PopupOpen<PopupPanel>().SetPopup("Error", "Failed to check duplication.\n" + e.Message);
            callback?.Invoke(false); // �⺻������ �ߺ� �ƴ� ó��
        }
    }

}
