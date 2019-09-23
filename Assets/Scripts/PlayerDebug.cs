using UnityEngine;
using UnityEngine.UI;

public class PlayerDebug : MonoBehaviour {
    [SerializeField]
    private Text text;

    private PlayerMovement playerMovement;

    private void Awake() {
        Player.onLocalPlayerSpawned += player => playerMovement = player.GetComponent<PlayerMovement>();
    }

    private void Update() {
        if (!playerMovement || !text)
            return;

        text.text = "";
        text.text += $"velocity: {playerMovement.Velocity}\n";
        text.text += $"left: {playerMovement.Controller.Collisions.left}\n";
        text.text += $"right: {playerMovement.Controller.Collisions.right}\n";
        text.text += $"above: {playerMovement.Controller.Collisions.above}\n";
        text.text += $"below: {playerMovement.Controller.Collisions.below}\n";
        text.text += $"fallingThroughPlatform: {playerMovement.Controller.Collisions.fallingThroughPlatform}\n";
    }
}