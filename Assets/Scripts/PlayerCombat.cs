using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerCombat : PlayerCombatBehavior {
    private new Camera camera;
    private PlayerInput input;

    // The position the player is aiming at with their mouse, in world space.
    public Vector2 AimingPosition { get; private set; }

    private void Awake() {
        camera = Camera.main;
        input = GetComponent<PlayerInput>();
    }

    protected override void NetworkStart() {
        networkObject.UpdateInterval = 50;
    }

    private void Update() {
        if (networkObject != null) {
            NetworkUpdate();
        } else {
            UpdateCombat();
        }
    }

    private void NetworkUpdate() {
        if (networkObject.IsOwner) {
            UpdateCombat();
            networkObject.aimingPosition = AimingPosition;
        } else {
            AimingPosition = networkObject.aimingPosition;
        }
    }

    private void UpdateCombat() {
        AimingPosition = camera.ScreenToWorldPoint(input.MousePosition);
    }
}