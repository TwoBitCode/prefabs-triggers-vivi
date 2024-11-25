using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

/**
 * This component spawns the given object at fixed time-intervals at its object position.
 */
public class TimedSpawner : MonoBehaviour
{
    [SerializeField] private Mover prefabToSpawn; // The prefab to spawn (must have a Mover component)
    [SerializeField] private Vector3 normalVelocityOfSpawnedObject = new Vector3(0, -5f, 0); // Default normal velocity
    [SerializeField] private Vector3 aggressiveVelocityOfSpawnedObject = new Vector3(0, -10f, 0); // Default aggressive velocity
    [SerializeField] private float normalSecondsBetweenSpawns = 1f; // Spawn interval in normal mode
    [SerializeField] private float aggressiveSecondsBetweenSpawns = 0.5f; // Spawn interval in aggressive mode

    private Vector3 currentVelocity;
    private float currentSpawnInterval;

    private void Start()
    {
        // Initialize with normal state
        currentVelocity = normalVelocityOfSpawnedObject;
        currentSpawnInterval = normalSecondsBetweenSpawns;

        // Subscribe to aggression mode changes
        AggressionTracker.OnAggressionModeChanged += HandleAggressionMode;

        // Start the spawning routine
        SpawnRoutine();
        Debug.Log("TimedSpawner started successfully.");
    }

    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        AggressionTracker.OnAggressionModeChanged -= HandleAggressionMode;
    }

    private void HandleAggressionMode(bool isAggressive)
    {
        if (isAggressive)
        {
            // Switch to aggressive mode
            currentSpawnInterval = aggressiveSecondsBetweenSpawns;
            currentVelocity = aggressiveVelocityOfSpawnedObject;
            Debug.Log("Aggressive mode activated: Increased spawn rate and velocity.");
        }
        else
        {
            // Switch back to normal mode
            currentSpawnInterval = normalSecondsBetweenSpawns;
            currentVelocity = normalVelocityOfSpawnedObject;
            Debug.Log("Aggressive mode deactivated: Reverted to normal spawn rate and velocity.");
        }
    }

    public void SetSpawnInterval(float minInterval, float maxInterval)
    {
        normalSecondsBetweenSpawns = minInterval;
        aggressiveSecondsBetweenSpawns = maxInterval;
        Debug.Log($"Spawn intervals updated: Normal={minInterval}s, Aggressive={maxInterval}s");
    }

    public void SetSpawnedObjectVelocity(Vector3 normalVelocity, Vector3 aggressiveVelocity)
    {
        normalVelocityOfSpawnedObject = normalVelocity;
        aggressiveVelocityOfSpawnedObject = aggressiveVelocity;
        Debug.Log($"Velocities updated: Normal={normalVelocity}, Aggressive={aggressiveVelocity}");
    }

    private async void SpawnRoutine()
    {
        while (true)
        {
            // Spawn the object
            GameObject newObject = Instantiate(prefabToSpawn.gameObject, transform.position, Quaternion.identity);

            // Assign velocity
            Mover mover = newObject.GetComponent<Mover>();
            if (mover != null)
            {
                mover.SetVelocity(currentVelocity);
                Debug.Log($"Spawned {newObject.name} with velocity: {currentVelocity}");
            }
            else
            {
                Debug.LogError($"Spawned object {newObject.name} does not have a Mover component!");
            }

            // Wait for the current spawn interval
            await Awaitable.WaitForSecondsAsync(currentSpawnInterval);
        }
    }
}
