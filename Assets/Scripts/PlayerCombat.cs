using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerCombat : MonoBehaviour {
    [SerializeField]
    private Transform bulletOriginPoint;

    public Weapon Weapon => weaponManager.GetWeapon(player.networkObject.weaponIndex);

    // The position the bullets are shooting from
    public Vector2 OriginPosition => bulletOriginPoint.position;

    // The position the player is aiming at, in world space.
    public Vector2 TargetPosition { get; private set; }

    private new Camera camera;
    private WeaponManager weaponManager;

    private Player player;
    private PlayerInput playerInput;

    private readonly Dictionary<uint, Bullet> bullets = new Dictionary<uint, Bullet>();
    private uint nextBulletId;

    private float timeSinceLastShot;

    private void Awake() {
        camera = Camera.main;
        weaponManager = FindObjectOfType<WeaponManager>();

        player = GetComponent<Player>();
        playerInput = GetComponent<PlayerInput>();

        player.onRespawn += Reset;

        playerInput.onPrimaryFireDown += OnPrimaryFireDown;
        playerInput.onPrimaryFireHeld += OnPrimaryFireHeld;
    }

    public void Reset() {
        player.networkObject.weaponIndex = -1;
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
        timeSinceLastShot += Time.deltaTime;
        TargetPosition = camera.ScreenToWorldPoint(playerInput.MousePosition);
    }

    public void OnPrimaryFireDown() {
        if (Weapon && !Weapon.FullAuto)
            Fire();
    }

    public void OnPrimaryFireHeld() {
        if (Weapon && Weapon.FullAuto)
            Fire();
    }

    private void Fire() {
        if (!player.IsAlive)
            return;

        if (timeSinceLastShot < 1f / Weapon.RateOfFire)
            return;

        player.Shoot(nextBulletId, OriginPosition, TargetPosition - OriginPosition);
        timeSinceLastShot = 0f;

        nextBulletId++;
    }

    // Runs on all clients when this player shoots
    public void Shoot(uint bulletId, Vector2 position, Vector2 direction) {
        var bullet = Instantiate(Weapon.Bullet, position, Quaternion.FromToRotation(Vector2.right, direction));
        bullet.Damage = Weapon.Damage;
        bullet.Player = player;
        bullet.BulletId = bulletId;
        bullets.Add(bulletId, bullet);
    }

    public void DestroyBullet(uint bulletId) {
        bullets[bulletId].Destroy();
        bullets.Remove(bulletId);
    }
}