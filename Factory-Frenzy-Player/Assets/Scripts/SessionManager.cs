using System.Threading.Tasks;
using FactoryFrenzy.Singleton;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;

public class SessionManager : Singleton<SessionManager>
{
    public ISession Session { get; private set; }
    public bool IsReady { get; private set; }
    public string PlayerName { get; set; } = "";

    [SerializeField] private int maxPlayers;

    protected override void Awake()
    {
        base.Awake();
        if (Instance == this) DontDestroyOnLoad(gameObject);
    }

    async void Start()
    {
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SwitchProfile("p" + Random.Range(0, 100000));
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        IsReady = true;
    }

    public async Task CreateSession()
    {
        var options = new SessionOptions { MaxPlayers = maxPlayers }.WithRelayNetwork();
        Session = await MultiplayerService.Instance.CreateSessionAsync(options);
    }

    public async Task JoinSession(string code)
    {
        Session = await MultiplayerService.Instance.JoinSessionByCodeAsync(code);
    }

    public async Task LockSession()
    {
        var host = Session.AsHost();
        host.IsLocked = true;
        await host.SavePropertiesAsync();
    }
}