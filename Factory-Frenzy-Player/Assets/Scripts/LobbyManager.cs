using System.Linq;
using FactoryFrenzy.Singleton;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : Singleton<LobbyManager>
{
    [SerializeField] private string gameSceneName;
    [SerializeField] private ReadyManager readyManager;
    
    public bool GameStarting { get; private set; }
    
    void OnEnable() => readyManager.AllReady += StartGame;
    void OnDisable() => readyManager.AllReady -= StartGame;

    async void StartGame()
    {
        if (GameStarting) return;
        GameStarting = true;
        CleanupLobbyObjects();
        await SessionManager.Instance.LockSession();
        NetworkManager.Singleton.SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
    }

    void CleanupLobbyObjects()
    {
        foreach (var obj in NetworkManager.Singleton.SpawnManager.SpawnedObjectsList.ToList())
        {
            if (obj.IsPlayerObject) continue;
            if (obj.InScenePlaced == true) continue;
            obj.Despawn(true);
        }
    }
}
