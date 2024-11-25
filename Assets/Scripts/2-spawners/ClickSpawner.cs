using UnityEngine;
using UnityEngine.InputSystem;

/**
 * This component spawns the given object whenever the player clicks a given key.
 */
public class ClickSpawner : MonoBehaviour
{
    [SerializeField] protected InputAction spawnAction = new InputAction(type: InputActionType.Button);
    [SerializeField] protected GameObject prefabToSpawn;
    [SerializeField] protected Vector3 normalVelocityOfSpawnedObject;
    [SerializeField] protected Vector3 aggressiveVelocityOfSpawnedObject;

    protected Vector3 currentVelocity;

    private void Start()
    {
        // Initialize with normal velocity
        currentVelocity = normalVelocityOfSpawnedObject;

        // Subscribe to aggression mode changes
        AggressionTracker.OnAggressionModeChanged += HandleAggressionMode;
    }

    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        AggressionTracker.OnAggressionModeChanged -= HandleAggressionMode;
    }

    private void HandleAggressionMode(bool isAggressive)
    {
        // Adjust velocity based on aggression mode
        currentVelocity = isAggressive ? aggressiveVelocityOfSpawnedObject : normalVelocityOfSpawnedObject;
    }

    void OnEnable()
    {
        spawnAction.Enable();
    }

    void OnDisable()
    {
        spawnAction.Disable();
    }

    protected virtual GameObject spawnObject()
    {
        // Step 1: spawn the new object.
        Vector3 positionOfSpawnedObject = transform.position;  // Spawn at the containing object position.
        Quaternion rotationOfSpawnedObject = Quaternion.identity;  // No rotation.
        GameObject newObject = Instantiate(prefabToSpawn, positionOfSpawnedObject, rotationOfSpawnedObject);

        // Step 2: modify the velocity of the new object.
        Mover newObjectMover = newObject.GetComponent<Mover>();
        if (newObjectMover)
        {
            newObjectMover.SetVelocity(currentVelocity);
        }

        return newObject;
    }

    protected virtual void Update()
    {
        if (spawnAction.triggered && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            spawnObject();
        }
    }
}
