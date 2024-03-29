﻿using Sirenix.OdinInspector;
using UnityEngine;

public class HurtBox : MonoBehaviour {
    [SerializeField, MinValue(1)] private float multiplier = 1f;
    public Player Player { get; private set; }

    private void Awake() {
        Player = GetComponentInParent<Player>();
    }

    public void Hit(Bullet bullet) {
        Player.Damage(Mathf.RoundToInt(multiplier * bullet.Damage));
        Player.Knockback(bullet.transform.right * bullet.Knockback);
    }

    public void Hit(MeleeHitbox hitbox) {
        Player.Damage(Mathf.RoundToInt(multiplier * hitbox.Damage));
        Player.Knockback(hitbox.transform.up * hitbox.Knockback);
    }
}