using UnityEngine;

public abstract class Weapon : MonoBehaviour {
    [SerializeField]
    protected int damage;

    [SerializeField]
    protected float rateOfFire;

    public abstract Transform OriginPosition { get; }
    public abstract Sprite Sprite { get; }

    public abstract bool CanShoot();

    public abstract Bullet Fire(uint bulletId, Vector2 originPosition, Vector2 targetPosition, Player player);
}