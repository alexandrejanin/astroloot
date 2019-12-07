using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour {
    [SerializeField] private Sprite crosshair, reload;

    private Player player;
    private Image image;

    private void Awake() {
        image = GetComponent<Image>();
        Player.onLocalPlayerSpawned += localPlayer => player = localPlayer;
    }

    private void Update() {
        Cursor.visible = false;

        transform.position = Camera.main.WorldToScreenPoint(player.PlayerCombat.TargetPosition);

        var weapon = player.PlayerCombat.Weapon as BulletWeapon;

        if (weapon && weapon.IsReloading) {
            image.sprite = reload;
            image.fillAmount = weapon.ReloadProgress;
            return;
        }

        image.fillAmount = 1;
        image.sprite = crosshair;
    }
}