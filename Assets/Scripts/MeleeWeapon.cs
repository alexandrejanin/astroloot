using UnityEngine;

public class MeleeWeapon : Weapon
{
    public override Transform OriginPosition { get; }
    public override Sprite Sprite { get; }
    public override bool CanShoot() {
        throw new System.NotImplementedException();
    }

    public override Bullet Fire(uint bulletId, Vector2 originPosition, Vector2 targetPosition, Player player) {
        throw new System.NotImplementedException();
    }
}
