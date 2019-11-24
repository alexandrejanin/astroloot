using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour {
    [SerializeField]
    private Text text;

    public PlayerHealth PlayerHealth { private get; set; }
    public Transform TargetTransform { private get; set; }

    private new Camera camera;

    private void Awake() {
        camera = Camera.main;
    }

    private void Update() {
        if (!PlayerHealth.IsAlive) {
            text.text = "";
            return;
        }

        text.text = $"{PlayerHealth.Health}/{PlayerHealth.MaxHealth}";
        transform.position = camera.WorldToScreenPoint(TargetTransform.position);
    }
}