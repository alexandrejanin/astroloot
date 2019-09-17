using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public abstract class RaycastController : MonoBehaviour {
    [SerializeField, MinValue(0)]
    private float raySpacing = .1f;

    [SerializeField, MinValue(0)]
    protected float skinWidth = .015f;

    protected int horizontalRayCount;
    protected int verticalRayCount;

    protected float horizontalRaySpacing;
    protected float verticalRaySpacing;

    protected BoxCollider2D collider;
    public BoxCollider2D Collider => collider;

    protected RaycastOrigins raycastOrigins;

    protected virtual void Awake() {
        collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    protected void UpdateRaycastOrigins() {
        var bounds = collider.bounds;
        bounds.Expand(-2 * skinWidth);

        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
    }

    protected void CalculateRaySpacing() {
        var bounds = collider.bounds;
        bounds.Expand(-2 * skinWidth);

        horizontalRayCount = Mathf.CeilToInt(bounds.size.y / raySpacing);
        verticalRayCount = Mathf.CeilToInt(bounds.size.x / raySpacing);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    protected struct RaycastOrigins {
        public Vector2 topLeft, topRight, bottomLeft, bottomRight;
    }
}