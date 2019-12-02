using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerCombat : MonoBehaviour {
    [SerializeField] private Transform weaponParent;

    // The position the player is aiming at, in world space.
    public Vector2 TargetPosition { get; private set; }

    private Weapon weapon;

    public Weapon Weapon => weapon;

    private new Camera camera;
    private WeaponManager weaponManager;

    private Player player;
    private PlayerInput playerInput;

    private readonly Dictionary<uint, Bullet> bullets = new Dictionary<uint, Bullet>();
    private uint nextBulletId;

    private void Awake() {
        camera = Camera.main;
        weaponManager = FindObjectOfType<WeaponManager>();

        player = GetComponent<Player>();
        playerInput = GetComponent<PlayerInput>();

        player.onRespawn += Reset;

        playerInput.onPrimaryFireHeld += OnPrimaryFireHeld;
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

    public void Reset() {
        player.networkObject.weaponIndex = -1;
    }

    public void SetWeapon(int index) {
        if (weapon)
            Destroy(weapon.gameObject);

        weapon = Instantiate(weaponManager.GetWeapon(index), weaponParent);
        weapon.Player = player;
    }


    public void OnPrimaryFireHeld() {
        if (!weapon || !player.IsAlive)
            return;

        if (weapon.CanShoot()) {
            player.Shoot(nextBulletId, weapon.OriginPosition.position, TargetPosition);
            nextBulletId++;
        }
    }

    // Runs on all clients when this player shoots
    public void Shoot(uint bulletId, Vector2 originPosition, Vector2 targetPosition) {
        var bullet = weapon.Fire(bulletId, originPosition, targetPosition);
        if (bullet != null)
            bullets.Add(bulletId, bullet);
    }

    public void DestroyBullet(uint bulletId) {
        bullets[bulletId].Destroy();
        bullets.Remove(bulletId);
    }
}