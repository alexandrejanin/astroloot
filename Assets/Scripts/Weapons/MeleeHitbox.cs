using UnityEngine;

public class MeleeHitbox : MonoBehaviour {
    [SerializeField] private int damage;
    [SerializeField] private float knockback;

    public int Damage => damage;
    public float Knockback => knockback;

    private MeleeWeapon weapon;

    private void Awake() {
        weapon = GetComponentInParent<MeleeWeapon>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        weapon.OnHit(this, other);
    }
}