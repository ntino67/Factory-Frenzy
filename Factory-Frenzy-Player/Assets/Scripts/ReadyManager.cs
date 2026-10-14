using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class ReadyManager : MonoBehaviour
{
    public event Action ReadyStateChanged;
    public event Action AllReady;

    public int PlayerCount => _players.Count;
    public int ReadyCount => _players.Count(p => p.IsReady.Value);

    private readonly List<PlayerReady> _players = new();

    private void OnEnable()
    {
        PlayerReady.Spawned += Track;
        PlayerReady.Despawned += Untrack;

        var nm = NetworkManager.Singleton;
        if (nm == null || nm.SpawnManager == null) return;
        foreach (var obj in nm.SpawnManager.SpawnedObjectsList)
            if (obj.TryGetComponent(out PlayerReady p)) Track(p);
    }

    private void OnDisable()
    {
        PlayerReady.Spawned -= Track;
        PlayerReady.Despawned -= Untrack;
        foreach (var p in _players) p.IsReady.OnValueChanged -= OnReadyChanged;
        _players.Clear();
    }

    void Track(PlayerReady p)
    {
        if (_players.Contains(p)) return;
        _players.Add(p);
        p.IsReady.OnValueChanged += OnReadyChanged;
        Evaluate();
    }
    
    void Untrack(PlayerReady p)
    {
        if (!_players.Remove(p)) return;
        p.IsReady.OnValueChanged -= OnReadyChanged;
        Evaluate();
    }
    
    void OnReadyChanged(bool oldValue, bool newValue) => Evaluate();
    
    void Evaluate()
    {
        ReadyStateChanged?.Invoke();

        var nm = NetworkManager.Singleton;
        if (nm == null || !nm.IsServer) return;
        
        if (_players.Count > 0 && _players.All(p => p.IsReady.Value)) AllReady?.Invoke();
    }
}
