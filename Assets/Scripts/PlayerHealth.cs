using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerHealth : MonoBehaviour {
    [SerializeField]
    private int maxHealth;

    public int MaxHealth => maxHealth;
    public int Health => player.networkObject.health;

    public bool IsAlive => player.networkObject.alive;

    private Player player;

    private void Awake() {
        player = GetComponent<Player>();
    }

    public void Reset() {
        player.networkObject.alive = true;
        player.networkObject.health = maxHealth;
    }

    private void Update() {
        if (!player.IsLocalPlayer || !IsAlive)
            return;

        if (transform.position.y < -20)
            StartCoroutine(Die());
    }

    public void Damage(int damage) {
        if (!player.IsLocalPlayer || !IsAlive)
            return;

        player.networkObject.health -= damage;

        if (player.networkObject.health <= 0)
            StartCoroutine(Die());
    }

    private IEnumerator Die() {
        player.networkObject.alive = false;
        player.OnDeath();

        yield return new WaitForSeconds(3);
        Reset();
        player.OnRespawn();
    }
}