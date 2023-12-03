﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UserSettings : MonoBehaviour
{
    public UserType userType;
    public Role role;
    public TMP_Dropdown userTypeDropdown;
    public TMP_Dropdown roleDropdown;
    public TMP_InputField UserName;
    public Button startBtn;
    public Canvas startCanvas;
    public Camera startCamera;

    //! ローカルユーザー用シーンのインデックス
    [SerializeField]
    private int localSceneBuildIndex = 1;
    //! リモートユーザー用シーンのインデックス
    [SerializeField]
    private int remoteSceneBuildIndex = 2;
    List<string> userTypeList = new List<string>();
    List<string> roleList = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (UserType Value in Enum.GetValues(typeof(UserType)))
        {
            string name = Enum.GetName(typeof(UserType), Value);
            userTypeList.Add(name);
        }
        foreach (Role Value in Enum.GetValues(typeof(Role)))
        {
            string name = Enum.GetName(typeof(Role), Value);
            roleList.Add(name);
        }
        userTypeDropdown.ClearOptions();
        userTypeDropdown.AddOptions(userTypeList);
        roleDropdown.ClearOptions();
        roleDropdown.AddOptions(roleList);
        ChangeDefaultUserName();
    }

    //DropdownのOnValueChangedでコールバック
    public void ChangeDefaultUserName()
    {
        if (userTypeDropdown.captionText.text == "Robot")
        {
            UserName.text = userTypeDropdown.captionText.text;
            roleDropdown.value= 3;
            roleDropdown.interactable = false;
            ChangeRole();
        }
        else
        {
            UserName.text = userTypeDropdown.captionText.text + "_user";
            if(roleDropdown.value == 3)
                roleDropdown.value = 0;
            roleDropdown.interactable = true;
            ChangeRole();
        }

        userType = (UserType)Enum.ToObject(typeof(UserType), userTypeDropdown.value);
    }

    public void ChangeRole()
    {
        role = (Role)Enum.ToObject(typeof(Role), roleDropdown.value);
    }

    //StartボタンのOnClickでコールバック
    public void StartConnecting()
    {
#if UNITY_STANDALONE_WIN
        if(role==Role.Unkown)
            Debug.LogError("Please select your role.");
        else
        {
            if (userType == UserType.Remote_nonVR)
                StartCoroutine(LoadAsyncScene(localSceneBuildIndex));
            else if (userType == UserType.Remote_VR)
                StartCoroutine(LoadAsyncScene(localSceneBuildIndex));
            else if (userType == UserType.Robot)
                StartCoroutine(LoadAsyncScene(localSceneBuildIndex));
            else Debug.LogError("It is not allowed to choose current selected user type in your platform.\nPlease select different user type.");
        }

#elif UNITY_STANDALONE_OSX
        if (role == Role.Unkown)
            Debug.LogError("Please select your role.");
        else
        {
            if (userType == UserType.Remote_nonVR)
                StartCoroutine(LoadAsyncScene(localSceneBuildIndex));
            else if (userType == UserType.Robot)
                StartCoroutine(LoadAsyncScene(localSceneBuildIndex));
            else Debug.LogError("It is not allowed to choose current selected user type in your platform.\nPlease select different user type.");
        }

#elif UNITY_IOS
        if (userType == UserType.Local_AR)
            StartCoroutine(LoadAsyncScene(localSceneBuildIndex));
        else Debug.LogError("It is not allowed to choose current selected user type in your platform.\nPlease select different user type.");

#elif UNITY_ANDROID
        if (userType == UserType.Remote_VR)
            StartCoroutine(LoadAsyncScene(remoteSceneBuildIndex));
        else Debug.LogError("It is not allowed to choose current selected user type in your platform.\nPlease select different user type.");

#else
        Debug.Log("THIS PROJECT DOES NOT SUPPORT YOUR USING PLATFORM." +
            "\nSUPPORTED PLATFORM: WINDOWS, OSX, IOS, ANDROID(Meta Quest).");
#endif
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_IOS || UNITY_ANDROID
        //リモートからロボットの役割は不可能
        bool isImpossible;
        if((role==Role.Robot&&userType==UserType.Remote_nonVR)||(role==Role.Robot&&userType==UserType.Remote_VR))
            isImpossible = true;
        else
            isImpossible = false;

        if (UserName.text == "" || role == Role.Unkown || isImpossible)
            startBtn.interactable = false;
        else
            startBtn.interactable = true;
#endif
    }

    IEnumerator LoadAsyncScene(int sceneIndex)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        if (asyncLoad.isDone)
        {
            startCamera.gameObject.SetActive(false);
            startCanvas.gameObject.SetActive(false);
        }
    }
}