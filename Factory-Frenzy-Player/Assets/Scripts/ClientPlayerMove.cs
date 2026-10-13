using StarterAssets;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class ClientPlayerMove: NetworkBehaviour
{
    [SerializeField] CharacterController m_CharacterController;
    [SerializeField] ThirdPersonController m_ThirdPersonController;
    [SerializeField] PlayerInput m_PlayerInput;
    [SerializeField] Transform m_CameraFollow;
        
    private void Awake()
    {
        m_PlayerInput.enabled = false;
        m_ThirdPersonController.enabled = false;
        m_CharacterController.enabled = false;
    }
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        enabled = IsClient;
        if (!IsOwner)
        {

            enabled = false;
            m_PlayerInput.enabled = false;
            m_CharacterController.enabled = false;
            m_ThirdPersonController.enabled = false;
            return;
        }

        m_PlayerInput.enabled = true;
        m_CharacterController.enabled = true;
        m_ThirdPersonController.enabled = true;
        
        var vcam = FindAnyObjectByType<CinemachineCamera>();
        if (vcam == null) return;
        
        vcam.Follow = m_CameraFollow;
    }
}