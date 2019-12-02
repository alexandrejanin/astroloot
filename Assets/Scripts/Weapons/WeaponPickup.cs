using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;

public class WeaponPickup : WeaponPickupBehavior {
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private new BoxCollider2D collider;

    private WeaponManager weaponManager;

    private int weaponIndex = -1;
    private Weapon weapon;

    private void Awake() {
        weaponManager = FindObjectOfType<WeaponManager>();
    }

    protected override void NetworkStart() {
        base.NetworkStart();

        if (networkObject.IsOwner) {
            networkObject.weaponIndex = weaponManager.GetRandomWeaponIndex();
        }
    }

    private void Update() {
        if (networkObject.weaponIndex != weaponIndex) {
            weaponIndex = networkObject.weaponIndex;
            weapon = weaponManager.GetWeapon(weaponIndex);

            spriteRenderer.sprite = weapon.Sprite;
            var bounds = spriteRenderer.sprite.bounds;
            collider.offset = bounds.center;
            collider.size = bounds.size;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!networkObject.IsOwner)
            return;

        var player = other.GetComponent<Player>();
        if (player == null)
            return;

        player.SetWeapon(networkObject.weaponIndex);
        networkObject.Destroy();
    }
}