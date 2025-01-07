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
        //파이어베이스 초기화 상태 체크. 비동기(Async)함수이므로 완료될 때 까지 대기
        DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync();
        //초기화 성공
        if (status == DependencyStatus.Available)
        {
            App = FirebaseApp.DefaultInstance;
            Auth = FirebaseAuth.DefaultInstance;
            DB = FirebaseDatabase.DefaultInstance;
        }
        //초기화 실패
        else
        {
            Debug.LogWarning($"파이어베이스 초기화 실패 : {status}");
        }

    }

    public async void Create(string email, string passwd, Action<FirebaseUser> callback = null)
    {
        try
        {
            var result = await Auth.CreateUserWithEmailAndPasswordAsync(email, passwd);
            usersRef = DB.GetReference($"users/{result.User.UserId}");

            // 회원의 데이터를 Database에 생성
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

    //로그인
    public async void SignIn(string email, string passwd, Action<FirebaseUser> callback = null)
    {
        try
        {
            var result = await Auth.SignInWithEmailAndPasswordAsync(email, passwd);

            usersRef = DB.GetReference($"users/{result.User.UserId}");

            // 데이터 불러오기
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
            callback?.Invoke(false); // 기본적으로 중복 아님 처리
        }
    }

}
