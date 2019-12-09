using UnityEngine;

public abstract class Weapon : MonoBehaviour {
    public abstract Transform OriginPosition { get; }
    public abstract Sprite Sprite { get; }
    public abstract Vector3 ArmAngle { get; }

    public abstract bool FullAuto { get; }

    protected Player player;

    public void Init(Player player) {
        this.player = player;
        Init();
    }

    protected virtual void Init() { }

    public abstract bool CanShoot();

    public abstract Bullet Fire(uint bulletId, Vector2 originPosition, Vector2 targetPosition);
}