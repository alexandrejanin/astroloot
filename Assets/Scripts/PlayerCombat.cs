using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerCombat : MonoBehaviour {
    [SerializeField]
    private Transform bulletOriginPoint;

    [SerializeField]
    private Bullet bulletPrefab;

    // The position the bullets are shooting from
    public Vector2 OriginPosition => bulletOriginPoint.position;

    // The position the player is aiming at, in world space.
    public Vector2 TargetPosition { get; private set; }

    private new Camera camera;

    private Player player;
    private PlayerInput playerInput;

    private readonly Dictionary<uint, Bullet> bullets = new Dictionary<uint, Bullet>();
    private uint bulletId;

    private void Awake() {
        camera = Camera.main;
        player = GetComponent<Player>();
        playerInput = GetComponent<PlayerInput>();

        playerInput.onPrimaryFire += OnPrimaryFire;
    }

    private void Update() {
        if (!player.IsOnline) {
            UpdateCombat();
            return;
        }

        if (!player.IsLocalPlayer) {
            TargetPosition = player.networkObject.aimingPosition;
            return;
        }

        UpdateCombat();
        player.networkObject.aimingPosition = TargetPosition;
    }

    private void UpdateCombat() {
        TargetPosition = camera.ScreenToWorldPoint(playerInput.MousePosition);
    }

    public void OnPrimaryFire() {
        player.Shoot(bulletId, OriginPosition, TargetPosition - OriginPosition);

        bulletId++;
    }

    // Runs on all clients when this player shoots
    public void Shoot(uint bulletId, Vector2 position, Vector2 direction) {
        var bullet = Instantiate(bulletPrefab, position, Quaternion.FromToRotation(Vector2.right, direction));
        bullet.Player = player;
        bullet.BulletId = bulletId;
        bullets.Add(bulletId, bullet);
    }

    public void DestroyBullet(uint bulletId) {
        bullets[bulletId].Destroy();
        bullets.Remove(bulletId);
    }
}