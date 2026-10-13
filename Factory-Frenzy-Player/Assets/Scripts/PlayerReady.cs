using Unity.Netcode;

public class PlayerReady : NetworkBehaviour
{
    public NetworkVariable<bool> IsReady = new();

    [Rpc(SendTo.Server)]
    public void ToggleReadyRpc()
    {
        IsReady.Value = !IsReady.Value;
        LobbyManager.Instance?.CheckAllReady();
    }
}