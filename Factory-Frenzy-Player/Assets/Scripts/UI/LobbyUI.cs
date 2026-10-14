using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] GameObject connectPanel;
    [SerializeField] Button createButton, joinButton, readyButton;
    [SerializeField] TMP_InputField codeInput, nameInput;
    [SerializeField] TMP_Text statusLabel, readyLabel;
    [SerializeField] private ReadyManager _readyManager;

    void Awake()
    {
        createButton.onClick.AddListener(OnCreateClicked);
        joinButton.onClick.AddListener(OnJoinClicked);
        readyButton.onClick.AddListener(() =>
            NetworkManager.Singleton.LocalClient.PlayerObject
                .GetComponent<PlayerReady>().ToggleReadyRpc());
    }

    void Update()
    {
        // Buttons stay grey until sign-in to Unity Services is done
        bool ready = SessionManager.Instance != null && SessionManager.Instance.IsReady;
        createButton.interactable = joinButton.interactable = ready;
        var player = NetworkManager.Singleton?.LocalClient?.PlayerObject;
        readyButton.gameObject.SetActive(player != null);
        if (player != null)
            readyButton.GetComponentInChildren<TMP_Text>().text =
                player.GetComponent<PlayerReady>().IsReady.Value ? "Not ready" : "Ready";
    }

    async void OnCreateClicked()
    {
        SessionManager.Instance.PlayerName = nameInput.text;
        try
        {
            statusLabel.text = "Creating...";
            await SessionManager.Instance.CreateSession();
            statusLabel.text = $"Join code: {SessionManager.Instance.Session.Code}";
            connectPanel.SetActive(false);
        }
        catch (Exception e) { statusLabel.text = $"Error: {e.Message}"; }
    }

    async void OnJoinClicked()
    {
        SessionManager.Instance.PlayerName = nameInput.text;
        try
        {
            statusLabel.text = "Joining...";
            await SessionManager.Instance.JoinSession(codeInput.text.Trim());
            statusLabel.text = "Connected!";
            connectPanel.SetActive(false);
        }
        catch (Exception e) { statusLabel.text = $"Error: {e.Message}"; }
    }

    private void OnEnable() => _readyManager.ReadyStateChanged += RefreshReady;
    private void OnDisable() => _readyManager.ReadyStateChanged -= RefreshReady;

    void RefreshReady() => readyLabel.text = $"{_readyManager.ReadyCount}/{_readyManager.PlayerCount}";
}