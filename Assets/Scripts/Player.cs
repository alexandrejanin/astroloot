using System;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerCombat))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerGraphics))]
public class Player : PlayerBehavior {
    [SerializeField]
    private PlayerHealthBar healthBarPrefab;

    [SerializeField]
    private Transform healthBarTransform;


    public PlayerInput PlayerInput { get; private set; }
    public PlayerCombat PlayerCombat { get; private set; }
    public PlayerHealth PlayerHealth { get; private set; }
    public PlayerMovement PlayerMovement { get; private set; }

    public bool IsOnline => networkObject != null;
    public bool IsLocalPlayer => networkObject.IsOwner;
    public bool IsAlive => PlayerHealth.IsAlive;

    public Action onDeath;
    public Action onRespawn;

    public static Action<Player> onLocalPlayerSpawned;

    private void Awake() {
        PlayerInput = GetComponent<PlayerInput>();
        PlayerCombat = GetComponent<PlayerCombat>();
        PlayerHealth = GetComponent<PlayerHealth>();
        PlayerMovement = GetComponent<PlayerMovement>();

        var canvas = FindObjectOfType<Canvas>();
        var playerHealthBar = Instantiate(healthBarPrefab, canvas.transform);
        playerHealthBar.PlayerHealth = PlayerHealth;
        playerHealthBar.TargetTransform = healthBarTransform;
    }

    protected override void NetworkStart() {
        base.NetworkStart();
        networkObject.UpdateInterval = 40;

        PlayerHealth.Reset();
    }

    // Called on the server when the player spawns
    public void SetOwner(NetworkingPlayer player) {
        networkObject.AssignOwnership(player);
        networkObject.SendRpc(player, RPC_LOCAL_PLAYER_SPAWNED);
    }

    // Called on a client when the player of that client spawned
    public override void LocalPlayerSpawned(RpcArgs args) {
        onLocalPlayerSpawned?.Invoke(this);
    }

    public void Damage(int damage) {
        networkObject.SendRpc(networkObject.Owner, RPC_DAMAGE, damage);
    }

    public override void Damage(RpcArgs args) {
        var damage = args.GetNext<int>();

        MainThreadManager.Run(() =>
            PlayerHealth.Damage(damage)
        );
    }

    public void Shoot(uint bulletId, Vector2 position, Vector2 direction) {
        networkObject.SendRpc(RPC_SHOOT, Receivers.All, bulletId, position, direction);
    }

    public override void Shoot(RpcArgs args) {
        var bulletId = args.GetNext<uint>();
        var position = args.GetNext<Vector2>();
        var direction = args.GetNext<Vector2>();

        MainThreadManager.Run(() =>
            PlayerCombat.Shoot(bulletId, position, direction)
        );
    }

    public void DestroyBullet(uint bulletId) {
        networkObject.SendRpc(RPC_DESTROY_BULLET, Receivers.All, bulletId);
    }

    public override void DestroyBullet(RpcArgs args) {
        var bulletId = args.GetNext<uint>();

        MainThreadManager.Run(() =>
            PlayerCombat.DestroyBullet(bulletId)
        );
    }

    public void OnDeath() {
        networkObject.SendRpc(RPC_ON_DEATH, Receivers.All);
    }

    public override void OnDeath(RpcArgs args) {
        MainThreadManager.Run(() => onDeath?.Invoke());
    }

    public void OnRespawn() {
        networkObject.SendRpc(RPC_ON_RESPAWN, Receivers.All);
    }

    public override void OnRespawn(RpcArgs args) {
        MainThreadManager.Run(() => onRespawn?.Invoke());
    }
}