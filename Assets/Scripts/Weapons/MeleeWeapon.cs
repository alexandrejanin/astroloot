using System;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon {
    [SerializeField] private int startupFrames, activeFrames, endingFrames;
    [SerializeField] private float startupAngle, activeAngle, endingAngle;

    public override Transform OriginPosition => transform;
    public override Sprite Sprite => SpriteRenderer.sprite;

    private SpriteRenderer SpriteRenderer =>
        spriteRenderer ? spriteRenderer : spriteRenderer = GetComponent<SpriteRenderer>();

    private SpriteRenderer spriteRenderer;

    private readonly HashSet<Player> playersHit = new HashSet<Player>();

    private int frame;

    private WeaponState State {
        get {
            if (frame == 0)
                return WeaponState.Idle;

            if (frame <= startupFrames)
                return WeaponState.Startup;

            if (frame <= startupFrames + activeFrames)
                return WeaponState.Active;

            return WeaponState.Ending;
        }
    }

    public override Vector3 ArmAngle {
        get {
            switch (State) {
                case WeaponState.Idle:
                    return new Vector3(0, 0, 0);
                case WeaponState.Startup:
                    return new Vector3(0, 0, startupAngle);
                case WeaponState.Active:
                    return new Vector3(0, 0, activeAngle);
                case WeaponState.Ending:
                    return new Vector3(0, 0, endingAngle);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private void FixedUpdate() {
        if (frame > 0)
            frame++;

        if (frame > startupFrames + activeFrames + endingFrames)
            frame = 0;
    }

    public override bool CanShoot() => State == WeaponState.Idle;

    public override Bullet Fire(uint bulletId, Vector2 originPosition, Vector2 targetPosition) {
        playersHit.Clear();
        frame = 1;
        return null;
    }

    public void OnHit(MeleeHitbox hitbox, Collider2D other) {
        if (!Player.IsLocalPlayer || State != WeaponState.Active)
            return;

        var hurtBox = other.gameObject.GetComponent<HurtBox>();

        if (!hurtBox || hurtBox.Player == Player || playersHit.Contains(hurtBox.Player))
            return;

        Debug.Log(hitbox.name);
        playersHit.Add(hurtBox.Player);
        hurtBox.Hit(hitbox);
    }
}

public enum WeaponState {
    Idle,
    Startup,
    Active,
    Ending
}