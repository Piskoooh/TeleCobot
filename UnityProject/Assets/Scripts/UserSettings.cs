using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UserSettings : MonoBehaviour
{
    public UserType userType;
    public TMP_Dropdown userTypeDropdown;
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

    // Start is called before the first frame update
    void Start()
    {
        foreach (UserType Value in Enum.GetValues(typeof(UserType)))
        {
            string name = Enum.GetName(typeof(UserType), Value);
            userTypeList.Add(name);
        }
        userTypeDropdown.ClearOptions();
        userTypeDropdown.AddOptions(userTypeList);
        ChangeDefaultUserName();
    }

    //DropdownのOnValueChangedでコールバック
    public void ChangeDefaultUserName()
    {
        if (userTypeDropdown.captionText.text == "Robot")
            UserName.text = userTypeDropdown.captionText.text;
        else
            UserName.text = userTypeDropdown.captionText.text + "_user";

        userType = (UserType)Enum.ToObject(typeof(UserType), userTypeDropdown.value);
    }

    //StartボタンのOnClickでコールバック
    public void StartConnecting()
    {
#if UNITY_STANDALONE_WIN
        if (userType == UserType.Remote_nonVR)
            StartCoroutine(LoadAsyncScene(remoteSceneBuildIndex));
        else if (userType == UserType.Remote_VR)
            StartCoroutine(LoadAsyncScene(remoteSceneBuildIndex));
        else if (userType == UserType.Robot)
            StartCoroutine(LoadAsyncScene(localSceneBuildIndex));
        else Debug.LogError("It is not allowed to choose current selected user type in your platform.\nPlease select different user type.");

#elif UNITY_STANDALONE_OSX
        if (userType == UserType.Remote_nonVR)
            StartCoroutine(LoadAsyncScene(remoteSceneBuildIndex));
        else if (userType == UserType.Robot)
            StartCoroutine(LoadAsyncScene(localSceneBuildIndex));
        else Debug.LogError("It is not allowed to choose current selected user type in your platform.\nPlease select different user type.");

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
#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WINDOWS || UNITY_IOS || UNITY_ANDROID
        if (UserName.text == "")
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
