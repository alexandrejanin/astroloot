using System;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour {
    private Player player;

    public Vector2 MovementInput { get; private set; }
    public Vector2 MousePosition { get; private set; }

    public Action onJumpDown, onJumpUp;

    private void Awake() {
        player = GetComponent<Player>();
    }

    private void Update() {
        if (player.networkObject != null && !player.networkObject.IsOwner)
            return;

        MovementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        MousePosition = Input.mousePosition;

        if (Input.GetKeyDown(KeyCode.Space))
            onJumpDown?.Invoke();

        if (Input.GetKeyUp(KeyCode.Space))
            onJumpUp?.Invoke();
    }
}