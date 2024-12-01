using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

/**
 * This component instantiates a given prefab at random time intervals and random bias from its object position.
 */
public class TimedSpawnerRandom : MonoBehaviour
{
    [SerializeField] Mover prefabToSpawn;
    [SerializeField] Vector3 normalVelocityOfSpawnedObject;
    [SerializeField] Vector3 aggressiveVelocityOfSpawnedObject;
    [Tooltip("Minimum time between consecutive spawns, in seconds")] [SerializeField] float minTimeBetweenSpawns = 0.2f;
    [Tooltip("Maximum time between consecutive spawns, in seconds")] [SerializeField] float maxTimeBetweenSpawns = 1.0f;
    [Tooltip("Maximum distance in X between spawner and spawned objects, in meters")] [SerializeField] float maxXDistance = 1.5f;
    [SerializeField] Transform parentOfAllInstances;

    private float currentMinTime;
    private float currentMaxTime;
    private Vector3 currentVelocity;

    private void Start()
    {
        // Initialize with normal state
        currentMinTime = minTimeBetweenSpawns;
        currentMaxTime = maxTimeBetweenSpawns;
        currentVelocity = normalVelocityOfSpawnedObject;

        // Subscribe to aggression mode changes
        AggressionTracker.OnAggressionModeChanged += HandleAggressionMode;

        // Start the spawning routine
        SpawnRoutine();
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
            // Debug.Log("Aggression Mode Activated: Faster spawns and enemies.");
            currentMinTime = minTimeBetweenSpawns * 0.5f;
            currentMaxTime = maxTimeBetweenSpawns * 0.5f;
            currentVelocity = aggressiveVelocityOfSpawnedObject;
        }
        else
        {
            // Debug.Log("Aggression Mode Deactivated: Normal spawns and enemies.");
            currentMinTime = minTimeBetweenSpawns;
            currentMaxTime = maxTimeBetweenSpawns;
            currentVelocity = normalVelocityOfSpawnedObject;
        }
    }

    private async void SpawnRoutine()
    {
        while (true)
        {
            float timeBetweenSpawnsInSeconds = Random.Range(currentMinTime, currentMaxTime);
            await Awaitable.WaitForSecondsAsync(timeBetweenSpawnsInSeconds); // Coroutine
            if (!this) break; // Might be destroyed when moving to a new scene
            Vector3 positionOfSpawnedObject = new Vector3(
                transform.position.x + Random.Range(-maxXDistance, +maxXDistance),
                transform.position.y,
                transform.position.z);
            GameObject newObject = Instantiate(prefabToSpawn.gameObject, positionOfSpawnedObject, Quaternion.identity);

            // Set velocity dynamically based on current mode
            Mover mover = newObject.GetComponent<Mover>();
            if (mover != null)
            {
                mover.SetVelocity(currentVelocity);
                ///Debug.Log($"Enemy spawned with velocity: {currentVelocity}");
            }
            else
            {
                //Debug.LogError("Spawned object is missing a Mover component!");
            }

            newObject.transform.parent = parentOfAllInstances;
        }
    }

    // Expose methods to modify spawn settings dynamically
    public void SetSpawnInterval(float minTime, float maxTime)
    {
        currentMinTime = minTime;
        currentMaxTime = maxTime;
        // Debug.Log($"Spawn interval updated: Min={minTime}, Max={maxTime}");
    }

    public void SetSpawnedObjectVelocity(Vector3 velocity)
    {
        currentVelocity = velocity;
        // Debug.Log($"Spawned object velocity updated to: {velocity}");
    }
}
