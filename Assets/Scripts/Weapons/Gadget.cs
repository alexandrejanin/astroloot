using UnityEngine;

public abstract class Gadget : MonoBehaviour {
    public abstract bool FullAuto { get; }
    protected Player player;

    public void Init(Player player) {
        this.player = player;
        Init();
    }

    protected virtual void Init() { }

    public abstract void Use();
}