using UnityEngine;

public class MeleeHitbox : MonoBehaviour {
    [SerializeField] private int damage;
    public int Damage => damage;
    private MeleeWeapon weapon;

    private void Awake() {
        weapon = GetComponentInParent<MeleeWeapon>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        weapon.OnHit(this, other);
    }
}