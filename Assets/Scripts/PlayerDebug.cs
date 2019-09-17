using UnityEngine;
using UnityEngine.UI;

public class PlayerDebug : MonoBehaviour {
    [SerializeField]
    private Player player;

    [SerializeField]
    private Text text;

    private void Update() {
        text.text = "";
        text.text += $"velocity: {player.Velocity}\n";
        text.text += $"left: {player.Controller.Collisions.left}\n";
        text.text += $"right: {player.Controller.Collisions.right}\n";
        text.text += $"above: {player.Controller.Collisions.above}\n";
        text.text += $"below: {player.Controller.Collisions.below}\n";
        text.text += $"fallingThroughPlatform: {player.Controller.Collisions.fallingThroughPlatform}\n";
    }
}