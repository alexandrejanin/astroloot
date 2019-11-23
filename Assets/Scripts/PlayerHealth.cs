using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerHealth : MonoBehaviour {
    [SerializeField]
    private int maxHealth;

    public int MaxHealth => maxHealth;
    public int Health => player.networkObject.health;

    private Player player;

    private void Awake() {
        player = GetComponent<Player>();
    }

    public void Reset() {
        player.networkObject.health = maxHealth;
    }

    public void Damage(int damage) {
        player.networkObject.health -= damage;

        if (player.networkObject.health <= 0)
            Die();
    }

    private void Die() {
    }
}