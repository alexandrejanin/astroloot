using System.Collections.Generic;
using UnityEngine;

public class PlatformController : RaycastController {
    [SerializeField]
    private LayerMask collisionMask;

    [SerializeField]
    private float speed = 1f;

    [SerializeField]
    private bool loop;

    [SerializeField]
    private Vector3[] waypoints;

    private Vector3 startPosition;
    private int lastWaypointIndex;
    private float waypointProgress;

    private readonly HashSet<Transform> movedTransforms = new HashSet<Transform>();
    private readonly List<PassengerMovement> passengerMovement = new List<PassengerMovement>();
    private readonly Dictionary<Transform, CharacterController2D> passengerControllers = new Dictionary<Transform, CharacterController2D>();

    protected override void Awake() {
        base.Awake();
        startPosition = transform.position;
    }

    private void Update() {
        UpdateRaycastOrigins();

        var velocity = CalculatePlatformMovement();

        CalculatePassengerMovement(velocity);
        MovePassengers(true);
        transform.Translate(velocity);
        MovePassengers(false);
    }

    private Vector3 CalculatePlatformMovement() {
        var nextWaypointIndex = loop
            ? (lastWaypointIndex + 1) % waypoints.Length
            : lastWaypointIndex + 1;

        var waypointLength = Vector3.Distance(waypoints[lastWaypointIndex], waypoints[nextWaypointIndex]);
        waypointProgress += Time.deltaTime * speed / waypointLength;

        var nextPos = Vector3.Lerp(waypoints[lastWaypointIndex] + startPosition, waypoints[nextWaypointIndex] + startPosition, waypointProgress);

        if (waypointProgress >= 1) {
            lastWaypointIndex = nextWaypointIndex;
            waypointProgress = 0;
            if (!loop && lastWaypointIndex >= waypoints.Length - 1) {
                System.Array.Reverse(waypoints);
                lastWaypointIndex = 0;
            }
        }

        return nextPos - transform.position;
    }

    private void MovePassengers(bool beforePlatformMoved) {
        foreach (var passenger in passengerMovement) {
            if (!passengerControllers.ContainsKey(passenger.transform))
                passengerControllers.Add(passenger.transform, passenger.transform.GetComponent<CharacterController2D>());

            if (passenger.moveBeforePlatform == beforePlatformMoved) {
                passengerControllers[passenger.transform].Move(passenger.velocity, passenger.standingOnPlatform);
            }
        }
    }

    private void CalculatePassengerMovement(Vector3 velocity) {
        movedTransforms.Clear();
        passengerMovement.Clear();

        var xDirection = Mathf.Sign(velocity.x);
        var yDirection = Mathf.Sign(velocity.y);

        if (velocity.y != 0) {
            var rayLength = skinWidth + Mathf.Max(Mathf.Abs(velocity.y), skinWidth);

            for (var i = 0; i < verticalRayCount; i++) {
                var rayOrigin = yDirection == -1
                    ? raycastOrigins.bottomLeft
                    : raycastOrigins.topLeft;

                rayOrigin += verticalRaySpacing * i * Vector2.right;

                var hit = Physics2D.Raycast(rayOrigin, yDirection * Vector2.up, rayLength, collisionMask);
                Debug.DrawRay(rayOrigin, rayLength * yDirection * Vector2.up, Color.cyan);

                if (hit) {
                    if (hit.distance == 0)
                        continue;

                    if (movedTransforms.Contains(hit.transform))
                        continue;

                    movedTransforms.Add(hit.transform);
                    var pushX = yDirection == 1 ? velocity.x : 0;
                    var pushY = velocity.y - (hit.distance - skinWidth) * yDirection;

                    passengerMovement.Add(new PassengerMovement(
                        hit.transform,
                        new Vector3(pushX, pushY),
                        yDirection == 1,
                        true
                    ));
                }
            }
        }

        if (velocity.x != 0) {
            var rayLength = skinWidth + Mathf.Max(Mathf.Abs(velocity.x), skinWidth);

            for (var i = 0; i < horizontalRayCount; i++) {
                var rayOrigin = xDirection == -1
                    ? raycastOrigins.bottomLeft
                    : raycastOrigins.bottomRight;

                rayOrigin += horizontalRaySpacing * i * Vector2.up;

                var hit = Physics2D.Raycast(rayOrigin, xDirection * Vector2.right, rayLength, collisionMask);
                Debug.DrawRay(rayOrigin, rayLength * xDirection * Vector2.right, Color.cyan);

                if (hit) {
                    if (hit.distance == 0)
                        continue;

                    if (movedTransforms.Contains(hit.transform))
                        continue;

                    movedTransforms.Add(hit.transform);
                    var pushX = velocity.x - (hit.distance - skinWidth) * xDirection;
                    var pushY = -.01f;

                    passengerMovement.Add(new PassengerMovement(
                        hit.transform,
                        new Vector3(pushX, pushY),
                        false,
                        true
                    ));
                }
            }
        }

        if (velocity.y < 0 || velocity.y == 0 && velocity.x != 0) {
            var rayLength = 2 * skinWidth;

            for (var i = 0; i < verticalRayCount; i++) {
                var rayOrigin = raycastOrigins.topLeft + verticalRaySpacing * i * Vector2.right;
                var hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, collisionMask);
                Debug.DrawRay(rayOrigin, rayLength * Vector2.up, Color.cyan);


                if (hit) {
                    if (hit.distance == 0)
                        continue;

                    if (movedTransforms.Contains(hit.transform))
                        continue;

                    movedTransforms.Add(hit.transform);
                    var pushX = velocity.x;
                    var pushY = velocity.y;

                    passengerMovement.Add(new PassengerMovement(
                        hit.transform,
                        new Vector3(pushX, pushY),
                        true,
                        false
                    ));
                }
            }
        }
    }

    private void OnDrawGizmosSelected() {
        if (waypoints != null) {
            Gizmos.color = Color.red;
            var size = .3f;
            for (var i = 0; i < waypoints.Length; i++) {
                var waypoint = Application.isPlaying
                    ? waypoints[i] + startPosition
                    : waypoints[i] + transform.position;
                Gizmos.DrawLine(waypoint - Vector3.up * size, waypoint + Vector3.up * size);
                Gizmos.DrawLine(waypoint - Vector3.left * size, waypoint + Vector3.left * size);
            }
        }
    }

    struct PassengerMovement {
        public readonly Transform transform;
        public readonly Vector3 velocity;
        public readonly bool standingOnPlatform;
        public readonly bool moveBeforePlatform;

        public PassengerMovement(Transform transform, Vector3 velocity, bool standingOnPlatform, bool moveBeforePlatform) {
            this.transform = transform;
            this.velocity = velocity;
            this.standingOnPlatform = standingOnPlatform;
            this.moveBeforePlatform = moveBeforePlatform;
        }
    }
}