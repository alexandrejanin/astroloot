using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerCombat))]
public class PlayerGraphics : MonoBehaviour {
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private PlayerCombat playerCombat;

    private void Awake() {
        playerCombat = GetComponent<PlayerCombat>();
    }

    private void Update() {
        var facingLeft = playerCombat.AimingPosition.x < playerCombat.transform.position.x;

        var rotation = spriteRenderer.transform.localRotation;
        rotation.y = facingLeft ? 180 : 0;
        spriteRenderer.transform.localRotation = rotation;
    }
}