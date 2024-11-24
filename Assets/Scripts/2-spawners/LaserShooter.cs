using UnityEngine;
using System.Collections;

public class LaserShooter : ClickSpawner
{
    [SerializeField]
    [Tooltip("How many points to add to the shooter, if the laser hits its target")]
    private int pointsToAdd = 1;

    private NumberField scoreField;

    [Header("Key Combination Settings")]
    [SerializeField] private float comboTimeout = 2f; // Timeout for key combination
    private string currentInput = ""; // Stores the current key sequence
    private float comboTimer;

    // State tracking for temporary effects
    private bool isEffectActive = false;

    // Store original properties
    private Vector3 originalVelocity;
    private Vector3 originalScale;

    private void Start()
    {
        scoreField = GetComponentInChildren<NumberField>();
        if (!scoreField)
        {
            Debug.LogError($"No child of {gameObject.name} has a NumberField component!");
        }

        // Save the original properties for resetting
        originalVelocity = velocityOfSpawnedObject;
        originalScale = prefabToSpawn.transform.localScale;
    }

    protected override void Update()
    {
        base.Update();

        // Capture all key inputs from the user
        foreach (char key in Input.inputString)
        {
            currentInput += key;
            comboTimer = comboTimeout;

            // Check if the current input matches any predefined combinations
            ActivateWeapon();
        }

        // Reset the input if the timer expires
        if (currentInput.Length > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0)
            {
                Debug.Log("Input Reset: Timeout");
                currentInput = "";
            }
        }
    }

    private void ActivateWeapon()
    {
        Debug.Log($"Checking key combination: {currentInput}");

        if (currentInput == "123" && !isEffectActive)
        {
            StartCoroutine(ApplyOneTimeEffect(() =>
            {
                Debug.Log("Triple shot activated for '123'!");
                StartCoroutine(FireTripleShot());
            }));
            currentInput = ""; // Reset after activation
        }
        else if (currentInput == "xy" && !isEffectActive)
        {
            StartCoroutine(ApplyOneTimeEffect(() =>
            {
                Debug.Log("Laser speed increased for 'xy'!");
                velocityOfSpawnedObject = new Vector3(0, 20f, 0); // Increase laser speed for visibility
            }));
            currentInput = ""; // Reset after activation
        }
        else if (currentInput == "az" && !isEffectActive)
        {
            StartCoroutine(ApplyOneTimeEffect(() =>
            {
                Debug.Log("Laser size significantly increased for 'az'!");
                prefabToSpawn.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); // Very large size
            }));
            currentInput = ""; // Reset after activation
        }
        else if (currentInput.Length > 1)
        {
            Debug.Log($"Unrecognized key combination: {currentInput}");
        }
    }

    private IEnumerator ApplyOneTimeEffect(System.Action effectAction)
    {
        isEffectActive = true;

        // Apply the effect
        effectAction.Invoke();

        // Wait for the effect to "apply" (e.g., one shot)
        yield return new WaitForSeconds(1f);

        // Revert to original settings
        ResetToOriginalState();
        isEffectActive = false;
    }

    private void ResetToOriginalState()
    {
        Debug.Log("Reverting to original state.");

        // Reset velocity and scale to original values
        velocityOfSpawnedObject = originalVelocity;
        prefabToSpawn.transform.localScale = originalScale; // Reset scale to original size

        Debug.Log($"Reset complete. Velocity and scale restored to: {prefabToSpawn.transform.localScale}");
    }

    private IEnumerator FireTripleShot()
    {
        for (int i = 0; i < 3; i++)
        {
            spawnObject();
            yield return new WaitForSeconds(0.2f);
        }
    }

    protected override GameObject spawnObject()
    {
        GameObject newObject = base.spawnObject();

        // Ensure the laser spawns at the spaceship's position
        newObject.transform.position = this.transform.position;

        // Apply the prefab's current scale to the spawned object
        newObject.transform.localScale = prefabToSpawn.transform.localScale;

        // Debug log to confirm the scale
        Debug.Log($"Laser spawned at position: {newObject.transform.position} with scale: {newObject.transform.localScale}");

        ScoreAdder newObjectScoreAdder = newObject.GetComponent<ScoreAdder>();
        if (newObjectScoreAdder)
        {
            newObjectScoreAdder.SetScoreField(scoreField).SetPointsToAdd(pointsToAdd);
        }
        return newObject;
    }
}
