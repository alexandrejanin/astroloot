using System;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerCombat))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerGraphics))]
public class Player : PlayerBehavior {
    public static Action<Player> onLocalPlayerSpawned;

    public bool IsOnline => networkObject != null;
    public bool IsLocalPlayer => networkObject.IsOwner;

    protected override void NetworkStart() {
        base.NetworkStart();
        networkObject.UpdateInterval = 40;
    }

    // called on server when the player spawns
    public void SetOwner(NetworkingPlayer player) {
        networkObject.AssignOwnership(player);
        networkObject.SendRpc(player, RPC_LOCAL_PLAYER_SPAWNED);
    }

    // called on client when the player of that client spawned
    public override void LocalPlayerSpawned(RpcArgs args) {
        onLocalPlayerSpawned?.Invoke(this);
    }
}