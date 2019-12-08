using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour {
    public Vector2 MovementInput { get; private set; }
    public Vector2 MousePosition { get; private set; }

    public Action onPrimaryFireDown;
    public Action onPrimaryFireHeld;
    public Action onJumpDown, onJumpUp;

    private Player player;

    private readonly Dictionary<KeyCode, Action> keybinds = new Dictionary<KeyCode, Action>();

    private void Awake() {
        player = GetComponent<Player>();
    }

    private void Update() {
        if (!Application.isFocused || !player.networkObject.IsOwner)
            return;

        MovementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        MousePosition = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
            onPrimaryFireDown?.Invoke();

        if (Input.GetMouseButton(0))
            onPrimaryFireHeld?.Invoke();

        if (Input.GetKeyDown(KeyCode.Space))
            onJumpDown?.Invoke();

        if (Input.GetKeyUp(KeyCode.Space))
            onJumpUp?.Invoke();

        foreach (var keybind in keybinds)
            if (Input.GetKeyDown(keybind.Key))
                keybind.Value?.Invoke();
    }

    public void AddKeybind(KeyCode keyCode, Action action) {
        if (keybinds.ContainsKey(keyCode))
            keybinds[keyCode] += action;
        else
            keybinds.Add(keyCode, action);
    }
}