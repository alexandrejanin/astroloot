using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour {
    public Vector2 MovementInput { get; private set; }
    public Vector2 MousePosition { get; private set; }

    public bool PrimaryFireDown { get; private set; }
    public bool PrimaryFireHeld { get; private set; }

    public bool SecondaryFireDown { get; private set; }
    public bool SecondaryFireHeld { get; private set; }

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

        PrimaryFireDown = Input.GetMouseButtonDown(0);
        PrimaryFireHeld = Input.GetMouseButton(0);

        SecondaryFireDown = Input.GetMouseButtonDown(1);
        SecondaryFireHeld = Input.GetMouseButton(1);

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