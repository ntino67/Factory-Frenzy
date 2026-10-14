using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LobbyUI : MonoBehaviour
    {
        [SerializeField] private Button createButton, joinButton;
        [SerializeField] private TMP_InputField codeInput, nameInput;
        [SerializeField] private TMP_Text statusLabel, readyLabel;
        [SerializeField] private ReadyManager readyManager;

        private void Start()
        {
            createButton.onClick.AddListener(OnCreateClicked);
            joinButton.onClick.AddListener(OnJoinClicked);

            if (SessionManager.Instance is { IsReady: true } sessionManager)
            {
                OnSessionManagerReady(sessionManager);
            }
            else
            {
                SessionManager.Instance.OnInitialized += OnSessionManagerReady;
            }
        }


        private void OnSessionManagerReady(SessionManager sessionManager)
        {
            SessionManager.Instance.OnInitialized -= OnSessionManagerReady;

            createButton.interactable = true;
            joinButton.interactable = true;
        }

        private async void OnCreateClicked()
        {
            try
            {
                SessionManager.Instance.PlayerName = nameInput.text;

                ShowStatus("Creating...");

                await SessionManager.Instance.CreateSession();

                ShowStatus($"Join code: {SessionManager.Instance.Session.Code}");

                gameObject.SetActive(false);
            }
            catch (Exception e)
            {
                ShowStatus($"Error: {e.Message}");
            }
        }

        private async void OnJoinClicked()
        {
            try
            {
                SessionManager.Instance.PlayerName = nameInput.text;

                ShowStatus("Joining...");

                await SessionManager.Instance.JoinSession(codeInput.text.Trim());

                ShowStatus("Connected!");

                gameObject.SetActive(false);
            }
            catch (Exception e)
            {
                ShowStatus($"Error: {e.Message}");
            }
        }

        private void ShowStatus(string statusText)
        {
            statusLabel.text = statusText;
        }

        private void OnDestroy()
        {
            SessionManager.Instance.OnInitialized -= OnSessionManagerReady;
        }

        private void OnEnable() => readyManager.ReadyStateChanged += RefreshReady;
        private void OnDisable() => readyManager.ReadyStateChanged -= RefreshReady;

        void RefreshReady() => readyLabel.text = $"{readyManager.ReadyCount}/{readyManager.PlayerCount}";
    }
}