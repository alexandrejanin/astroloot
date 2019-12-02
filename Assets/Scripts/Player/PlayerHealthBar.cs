using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour {
    [SerializeField] private Image healthBarFill;

    [SerializeField] private Color allyColor, enemyColor;

    public Player Player {
        set {
            player = value;
            playerHealth = value.PlayerHealth;
        }
    }

    private Player player;
    private PlayerHealth playerHealth;
    public Transform TargetTransform { private get; set; }

    private new Camera camera;

    private void Awake() {
        camera = Camera.main;
    }

    private void Update() {
        transform.position = camera.WorldToScreenPoint(TargetTransform.position);

        healthBarFill.color = player.IsLocalPlayer ? allyColor : enemyColor;
        healthBarFill.fillAmount = (float) playerHealth.Health / playerHealth.MaxHealth;
    }
}