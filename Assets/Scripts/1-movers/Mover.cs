using UnityEngine;

/**
 * This component moves its object in a fixed velocity.
 * NOTE: velocity is defined as speed + direction.
 *       Speed is a number; velocity is a vector.
 */
public class Mover : MonoBehaviour
{
    [Tooltip("Movement vector in meters per second")]
    [SerializeField] private Vector3 velocity = new Vector3(0, 10f, 0); // Default upward velocity

    [Tooltip("Enable boundary checks to destroy or wrap the object if it goes out of bounds")]
    [SerializeField] private bool enableBoundaryCheck = false;

    [Tooltip("Enable wrapping instead of destroying the object when it goes out of bounds")]
    [SerializeField] private bool enableWrapping = false;

    [Tooltip("Boundary limits for the object's movement")]
    [SerializeField] private Vector2 xBoundaries = new Vector2(-10f, 10f);
    [SerializeField] private Vector2 yBoundaries = new Vector2(-5f, 5f);

    [Tooltip("Velocity multiplier for dynamic speed adjustments (e.g., Aggressive Mode)")]
    [SerializeField] private float velocityMultiplier = 1f;

    void Update()
    {
        // Move the object based on its velocity and multiplier
        transform.position += velocity * velocityMultiplier * Time.deltaTime;
        // Debug.Log($"Moving {gameObject.name} to position {transform.position} with velocity {velocity}");

        // Optional: Handle boundaries (destroy or wrap)
        if (enableBoundaryCheck)
        {
            if (enableWrapping)
            {
                WrapBounds();
            }
            else
            {
                CheckBoundsAndDestroy();
            }
        }
    }

    public void SetVelocity(Vector3 newVelocity)
    {
        this.velocity = newVelocity;
        //Debug.Log($"{gameObject.name} velocity set to {newVelocity}");
    }

    public void SetVelocityMultiplier(float multiplier)
    {
        this.velocityMultiplier = multiplier;
        // Debug.Log($"{gameObject.name} velocity multiplier set to {multiplier}");
    }

    private void CheckBoundsAndDestroy()
    {
        if (transform.position.x > xBoundaries.y || transform.position.x < xBoundaries.x ||
            transform.position.y > yBoundaries.y || transform.position.y < yBoundaries.x)
        {
            //Debug.Log($"{gameObject.name} went out of bounds and was destroyed.");
            Destroy(gameObject);
        }
    }

    private void WrapBounds()
    {
        Vector3 position = transform.position;

        // Wrap horizontally
        if (position.x > xBoundaries.y)
        {
            position.x = xBoundaries.x;
        }
        else if (position.x < xBoundaries.x)
        {
            position.x = xBoundaries.y;
        }

        // Wrap vertically
        if (position.y > yBoundaries.y)
        {
            position.y = yBoundaries.x;
        }
        else if (position.y < yBoundaries.x)
        {
            position.y = yBoundaries.y;
        }

        transform.position = position;
    }
}
