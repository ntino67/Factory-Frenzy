using System.Linq;
using FactoryFrenzy.Singleton;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : Singleton<LobbyManager>
{
    [SerializeField] private string gameSceneName;
    public bool GameStarting { get; private set; }

    void Start()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += _ => CheckAllReady();
    }

    public void CheckAllReady()
    {
        var nm = NetworkManager.Singleton;
        if (!nm.IsServer || GameStarting) return;

        foreach (var client in nm.ConnectedClientsList)
        {
            var ready = client.PlayerObject.GetComponent<PlayerReady>();
            if (ready == null || !ready.IsReady.Value) return;   // someone isn't ready
        }
        StartGame();
    }

    async void StartGame()
    {
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
