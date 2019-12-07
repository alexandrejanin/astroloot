using UnityEngine;
using UnityEngine.UI;

public class PlayerHud : MonoBehaviour {
    private Player player;

    [SerializeField] private Text ammoText;

    private void Awake() {
        Player.onLocalPlayerSpawned += localPlayer => player = localPlayer;
    }

    private void Update() {
        if (!player)
            return;

        var weapon = player.PlayerCombat.Weapon as BulletWeapon;

        ammoText.text = weapon
            ? $"{weapon.Bullets}/{weapon.MaxBullets}"
            : "";
    }
}