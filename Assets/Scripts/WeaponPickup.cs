using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;

public class WeaponPickup : WeaponPickupBehavior {
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private WeaponManager weaponManager;

    private int weaponIndex;

    private Weapon Weapon => weaponManager.GetWeapon(weaponIndex);

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
        spriteRenderer.sprite = Weapon.Sprite;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!networkObject.IsOwner)
            return;

        var player = other.GetComponent<Player>();
        if (player == null)
            return;

        player.SetWeaponIndex(weaponIndex);
        networkObject.Destroy();
    }
}