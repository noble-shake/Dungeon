using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

// 얘가 하는 일
// 1. 네트워크 씬 로드 시 호스트, 클라이언트 구분 없이 일어나야 하는 일들을 처리한다.
// 2. 일반적인 씬 로드 시 일어나야 하는 일들을 처리한다.

public class SceneLoadManager : MonoBehaviour
{
    public static SceneLoadManager Instance { get => instance; set => instance = value; }
    private static SceneLoadManager instance;
    public float loadingProgress { get; private set; }
    public Action OnLoadingStarted;
    public Action OnLoadingFinished;
    [Header("씬 이름")]
    public string Scene_Booting = "Scene_Booting";
    public string Scene_Lobby = "Scene_Lobby";
    public string Scene_Dungeon_01 = "Scene_Dungeon_01";
    public string Scene_Loading = "Scene_Loading";


    [Header("화면 페이드 인 / 아웃")]
    [SerializeField] CanvasGroup fadeCanvasGroup;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    // 네트워크 씬 전환을 할 때 호출되는 함수임.
    // 이 함수는 무조건 호스트에 의해서만 호출된다.
    // 호스트가 네트워크 씬을 로드하게 되면
    // 클라이언트들은 자동으로 동기화에 의해 씬을 로드하게 된다.
    // 이 때 로딩 씬을 어떻게 보여주느냐가 관건이다.
    public void LoadNetworkScene(string sceneName)
    {
        StartCoroutine(NetworkSceneLoading(sceneName));
    }

    private IEnumerator NetworkSceneLoading(string sceneName)
    {
        // UI RPC 호출해서 페이드 아웃 시킨다.
        yield return null;
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        // Onloadcomplete에 델리게이트 걸어줘서 페이드 인 처리 한다.

    }

    // 일반 씬 전환을 할 때 호출되는 함수임.

    // 일반 씬 <-> 일반 씬
    // 네트워크 씬 -> 일반 씬 (로비 -> 메인 메뉴)
    // 호스트 클라이언트 구분없이 사용 됨.
    public void LoadRegularScene(string sceneName, bool useLoadScreen = true)
    {
        StartCoroutine(ProcessRegularSceneLoading(sceneName, useLoadScreen));
    }

    public IEnumerator FadeOut(float time = 1f)
    {
        fadeCanvasGroup.blocksRaycasts = true;
        yield return fadeCanvasGroup.DOFade(1f, time).WaitForCompletion();
    }

    public IEnumerator FadeIn(float time = 1f)
    {
        // yield return new WaitForSeconds(5f);
        yield return fadeCanvasGroup.DOFade(0f, time).WaitForCompletion();
        fadeCanvasGroup.blocksRaycasts = false;
    }

    // 
    private IEnumerator ProcessRegularSceneLoading(string sceneToLoad, bool useLoadScene = true)
    {
        yield return FadeOut();
        // if (useLoadScene)
        // SceneManager.LoadScene(Scene_Loading);

        loadingProgress = 0f;

        // NOTE : 네트워크 씬 로딩은 비동기 작업인데, 유니티에서 비동기 씬 로딩은 90%에서 멈추는 것처럼 보이는 경향이 있음.
        // NOTE : 그건 로딩을 다해놓고 그 씬의 활성화, 비활성화에 따라 90%에서 멈출 수도 있기 때문.
        // NOTE : 따라서 0.9 즉, 90% 이상일 때 그냥 강제로 1로 만들어줌.

        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneToLoad);
        ao.allowSceneActivation = true;

        while (!ao.isDone)
        {
            loadingProgress = ao.progress;

            if (ao.progress > 0.9f)
            {
                loadingProgress = 1;
            }
            yield return 0;
        }

        yield return FadeIn();
    }

    // 시퀀스 흐름
    // 호스트가 네트워크씬 로드 실행 -> 서버와 클라이언트 모두에게 OnLoad 실행
    // 호스트, 클라이언트 모두 각자 씬 로딩을 완료했을 때 -> OnLoadComplete 실행
    // 클라이언트 입장에서 네트워크 오브젝트 메모리에 올릴 때 -> OnSynchronize
    // 클라이언트 입장에서 네트워크 오브젝트들을 다 생성했고 이제 스폰할 때 -> OnNetworkSpawn

    // 네트워크 씬 로드 전에 RPC 호출로 UI 페이드 아웃
    // 로딩바 처리는 어떻게?
    // 온로드 시 sceneLoadOperation을 받아서 진행도와 로딩 바를 연동한다.
    // 온로드 컴플리트 시 페이드인 해준다. 
    public void SubscribeOnNetworkSceneEvents()
    {
        // 네트워크 씬 로드가 발생할 때 호출된다.
        // 따라서 호스트, 클라이언트 모두에게 실행된다. 

        // NetworkManager.Singleton.SceneManager.OnLoad += (clientId, sceneName, mode, sceneLoadOperation) =>
        // {
        //     if (InputManager.instance.player != null)
        //     {
        //         print("내가 과연 여기서 실행되고 있을까? 1");
        //         InputManager.instance.player.GetComponent<NetworkObject>().Despawn(true);
        //     }

        //     StartCoroutine(ProcessNetworkSceneLoading(sceneLoadOperation));
        // };
        // if (NetworkManager.Singleton.SceneManager.OnLoadComplete == null)
        {
            NetworkManager.Singleton.SceneManager.OnLoadComplete += (clientId, sceneName, mode) =>
            {
                // 씬 로딩이 완료되면 화면을 페이드 인 해준다.

                Debug.Log("씬 로딩 완료됨.");
                StartCoroutine(FadeIn(1f));
                // SceneManager.UnloadSceneAsync(Scene_Loading);
            };
        }



    }

    // UI 함수, 기존의 함수를 UI용으로 이름붙여 씀. 구분을 편리하게끔 하기 위함.
    // TODO : 추후 아예 분리할 것.
    public void UILoadRegularScene(string sceneName)
    {
        LoadRegularScene(sceneName);
    }

    public string GetSceneName(SceneIndex sceneIndex)
    {
        int index = (int)sceneIndex;
        string sceneName = SceneManager.GetSceneByBuildIndex(index).name;
        return sceneName;
    }

    public void FadeInScene(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        Debug.Log("FadeInScene 호출됨.");
    }
}
