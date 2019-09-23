using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerCombat : MonoBehaviour {
    private new Camera camera;

    private Player player;
    private PlayerInput input;

    // The position the player is aiming at, in world space.
    public Vector2 AimingPosition { get; private set; }

    private void Awake() {
        camera = Camera.main;
        player = GetComponent<Player>();
        input = GetComponent<PlayerInput>();
    }

    private void Update() {
        if (!player.IsOnline) {
            UpdateCombat();
            return;
        }

        if (!player.IsLocalPlayer) {
            AimingPosition = player.networkObject.aimingPosition;
            return;
        }

        UpdateCombat();
        player.networkObject.aimingPosition = AimingPosition;
    }

    private void UpdateCombat() {
        AimingPosition = camera.ScreenToWorldPoint(input.MousePosition);
    }
}