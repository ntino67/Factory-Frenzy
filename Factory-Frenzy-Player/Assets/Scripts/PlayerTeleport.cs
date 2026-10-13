using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class PlayerTeleport : NetworkBehaviour
{
    [Rpc(SendTo.Owner)]
    public void TeleportRpc(Vector3 position, Quaternion rotation)
    {
        var cc = GetComponent<CharacterController>();
        cc.enabled = false;
        GetComponent<NetworkTransform>().Teleport(position, rotation, transform.localScale);
        cc.enabled = true;
    }
}
