using System;
using Unity.Netcode;

public class PlayerReady : NetworkBehaviour
{
    public static event Action<PlayerReady> Spawned; 
    public static event Action<PlayerReady> Despawned; 
    
    public NetworkVariable<bool> IsReady = new();

    public override void OnNetworkSpawn() => Spawned?.Invoke(this);
    public override void OnNetworkDespawn() => Despawned?.Invoke(this);

    [Rpc(SendTo.Server)]
    public void ToggleReadyRpc() => IsReady.Value = !IsReady.Value;
}