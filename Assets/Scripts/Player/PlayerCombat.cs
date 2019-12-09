using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerCombat : MonoBehaviour {
    [SerializeField] private Transform weaponParent;

    // The position the player is aiming at, in world space.
    public Vector2 TargetPosition { get; private set; }

    public Weapon Weapon { get; private set; }
    public Gadget Gadget { get; private set; }

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

        playerInput.AddKeybind(KeyCode.R, () => {
            var bulletWeapon = Weapon as BulletWeapon;
            if (bulletWeapon)
                bulletWeapon.Reload();
        });

        SetGadget(weaponManager.GetRandomGadgetIndex());
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

        if (!player.IsAlive)
            return;

        if (Weapon) {
            var weaponInput = Weapon.FullAuto
                ? playerInput.PrimaryFireHeld
                : playerInput.PrimaryFireDown;
            if (weaponInput && Weapon.CanShoot()) {
                player.Shoot(nextBulletId, Weapon.OriginPosition.position, TargetPosition);
                nextBulletId++;
            }
        }

        if (Gadget) {
            var gadgetInput = Gadget.FullAuto
                ? playerInput.SecondaryFireHeld
                : playerInput.SecondaryFireDown;
            if (gadgetInput)
                Gadget.Use();
        }
    }

    public void Reset() {
        player.networkObject.weaponIndex = -1;
    }

    public void SetWeapon(int index) {
        if (Weapon)
            Destroy(Weapon.gameObject);

        Weapon = Instantiate(weaponManager.GetWeapon(index), weaponParent);
        Weapon.Init(player);
    }

    public void SetGadget(int index) {
        if (Gadget)
            Destroy(Gadget.gameObject);

        Gadget = Instantiate(weaponManager.GetGadget(index), weaponParent);
        Gadget.Init(player);
    }

    // Runs on all clients when this player shoots
    public void Shoot(uint bulletId, Vector2 originPosition, Vector2 targetPosition) {
        var bullet = Weapon.Fire(bulletId, originPosition, targetPosition);
        if (bullet != null)
            bullets.Add(bulletId, bullet);
    }

    public void DestroyBullet(uint bulletId) {
        bullets[bulletId].Destroy();
        bullets.Remove(bulletId);
    }
}