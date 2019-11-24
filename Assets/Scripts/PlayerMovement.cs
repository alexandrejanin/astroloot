using System;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CharacterController2D))]
public class PlayerMovement : MonoBehaviour {
    [SerializeField, MinValue(0)]
    private float runSpeed = 5f, wallSlideSpeed = 2f;

    [SerializeField, MinValue(0)]
    private float accelerationTimeGrounded = .1f, accelerationTimeAirborne = .2f;

    [SerializeField, MinValue(0)]
    private float minJumpHeight = 1f, maxJumpHeight = 5f, timeToApex = .4f;

    [SerializeField]
    private Vector2 wallJumpTowards, wallJumpNeutral, wallJumpAway;

    [SerializeField, MinValue(0)]
    private float wallStickTime = .25f;

    private float timeToWallUnstick;

    private Vector3 velocity;
    public Vector3 Velocity => velocity;

    private float minJumpVelocity, maxJumpVelocity;
    private float gravity;

    private float speedSmoothing;

    private bool wallSliding;
    private float wallDirX;

    private Player player;
    private PlayerInput input;

    private CharacterController2D controller;
    public CharacterController2D Controller => controller;

    private void Awake() {
        player = GetComponent<Player>();
        input = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController2D>();

        input.onJumpDown += OnJumpDown;
        input.onJumpUp += OnJumpUp;

        player.onDeath += () => velocity = Vector3.zero;
        player.onRespawn += () => {
            var spawnPoint = FindObjectOfType<SpawnController>().GetSpawnPoint();
            transform.position = spawnPoint.position;
        };
    }

    private void Update() {
        if (!player.IsOnline) {
            UpdateMovement();
            return;
        }

        if (!player.IsLocalPlayer) {
            transform.position = player.networkObject.position;
            return;
        }

        if (!player.IsAlive)
            return;

        UpdateMovement();
        player.networkObject.position = transform.position;
    }

    private void UpdateMovement() {
        UpdateGravity();
        UpdateJumpForces();

        UpdateVelocity();
        UpdateWallSliding();

        controller.Move(velocity * Time.deltaTime, input.MovementInput);

        if (controller.Collisions.above || controller.Collisions.below) {
            if (controller.Collisions.slidingDownSlope) {
                velocity.y += controller.Collisions.slopeNormal.y * -gravity * Time.deltaTime;
            } else {
                velocity.y = 0;
            }
        }
    }

    private void UpdateGravity() {
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
    }

    private void UpdateJumpForces() {
        maxJumpVelocity = Mathf.Abs(gravity) * timeToApex;
        minJumpVelocity = Mathf.Sqrt(2 * Math.Abs(gravity) * minJumpHeight);
    }

    private void UpdateVelocity() {
        velocity.y += gravity * Time.deltaTime;
        velocity.x = Mathf.SmoothDamp(
            velocity.x,
            input.MovementInput.x * runSpeed,
            ref speedSmoothing,
            controller.Collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne
        );
    }

    private void UpdateWallSliding() {
        wallDirX = controller.Collisions.left ? -1 : 1;
        wallSliding = false;

        if ((controller.Collisions.left || controller.Collisions.right) && !controller.Collisions.below && velocity.y < 0) {
            wallSliding = true;

            if (input.MovementInput.x == wallDirX || timeToWallUnstick > 0) {
                if (velocity.y < -wallSlideSpeed)
                    velocity.y = -wallSlideSpeed;

                velocity.x = 0;
                speedSmoothing = 0;
                if (input.MovementInput.x == wallDirX) {
                    timeToWallUnstick = wallStickTime;
                } else {
                    timeToWallUnstick -= Time.deltaTime;
                }
            }
        } else {
            timeToWallUnstick = wallStickTime;
        }
    }

    public void OnJumpDown() {
        if (wallSliding) {
            if (wallDirX == input.MovementInput.x) {
                velocity.x = -wallDirX * wallJumpTowards.x;
                velocity.y = wallJumpTowards.y;
            } else if (input.MovementInput.x == 0) {
                velocity.x = -wallDirX * wallJumpNeutral.x;
                velocity.y = wallJumpNeutral.y;
            } else {
                velocity.x = -wallDirX * wallJumpAway.x;
                velocity.y = wallJumpAway.y;
            }
        }

        if (controller.Collisions.below)
            velocity.y = maxJumpVelocity;
    }

    public void OnJumpUp() {
        if (velocity.y > minJumpVelocity)
            velocity.y = minJumpVelocity;
    }
}