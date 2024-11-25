using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

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

    [Header("Shooting Limit Settings")]
    [SerializeField] private int maxShotsPerSecond = 3; // Max number of shots allowed per second
    private int shotsFired = 0;
    private float shotCooldownTimer = 0f;

    [SerializeField] private Vector3 velocityOfSpawnedObject; // Velocity for the laser
    private Vector3 originalVelocity;
    private Vector3 originalScale;

    private void Start()
    {
        scoreField = GetComponentInChildren<NumberField>();
        if (!scoreField)
        {
            Debug.LogError($"No child of {gameObject.name} has a NumberField component!");
        }

        originalVelocity = velocityOfSpawnedObject;
        if (prefabToSpawn != null)
        {
            originalScale = prefabToSpawn.transform.localScale;
        }

        AggressionTracker.OnAggressionModeChanged += HandleAggressionMode;

        Debug.Log($"LaserShooter initialized with velocity: {velocityOfSpawnedObject}");
    }

    private void OnDestroy()
    {
        AggressionTracker.OnAggressionModeChanged -= HandleAggressionMode;
    }

    protected override void Update()
    {
        base.Update();

        // Update cooldown timer for shot limit
        if (shotCooldownTimer > 0)
        {
            shotCooldownTimer -= Time.deltaTime;
            if (shotCooldownTimer <= 0)
            {
                shotsFired = 0; // Reset shot counter after cooldown
            }
        }

        // Check for space key press for shooting
        if (Keyboard.current.spaceKey.wasPressedThisFrame && shotsFired < maxShotsPerSecond)
        {
            FireLaser();
            shotsFired++;
            if (shotsFired == maxShotsPerSecond)
            {
                shotCooldownTimer = 1f; // Set cooldown timer (1 second)
                Debug.Log("Shot limit reached. Cooldown started.");
            }
        }

        // Capture key inputs for combos
        foreach (char key in Input.inputString)
        {
            currentInput += key;
            comboTimer = comboTimeout;

            Debug.Log($"Combo input detected: {currentInput}");
            ActivateWeapon();
        }

        // Reset combo input if timer expires
        if (currentInput.Length > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0)
            {
                Debug.Log("Combo input reset due to timeout.");
                currentInput = "";
            }
        }
    }

    private void FireLaser()
    {
        spawnObject();

        // Register the shot with AggressionTracker
        AggressionTracker tracker = Object.FindAnyObjectByType<AggressionTracker>();
        if (tracker != null)
        {
            tracker.RegisterShot();
        }

        Debug.Log("Laser fired.");
    }

    private void ActivateWeapon()
    {
        if (isEffectActive) return;

        if (currentInput == "123")
        {
            StartCoroutine(ApplyOneTimeEffect(() =>
            {
                Debug.Log("Triple shot activated for '123'!");
                StartCoroutine(FireTripleShot());
            }));
            currentInput = ""; // Reset combo
        }
        else if (currentInput == "xy")
        {
            StartCoroutine(ApplyOneTimeEffect(() =>
            {
                Debug.Log("Laser speed increased for 'xy'!");
                velocityOfSpawnedObject = new Vector3(0, 20f, 0);
            }));
            currentInput = ""; // Reset combo
        }
        else if (currentInput == "az")
        {
            StartCoroutine(ApplyOneTimeEffect(() =>
            {
                Debug.Log("Laser size significantly increased for 'az'!");
                prefabToSpawn.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            }));
            currentInput = ""; // Reset combo
        }
        else if (currentInput.Length > 1)
        {
            Debug.Log($"Unrecognized key combination: {currentInput}");
        }
    }

    private IEnumerator ApplyOneTimeEffect(System.Action effectAction)
    {
        isEffectActive = true;

        effectAction.Invoke();

        yield return new WaitForSeconds(1f);

        ResetToOriginalState();
        isEffectActive = false;
    }

    private void ResetToOriginalState()
    {
        velocityOfSpawnedObject = originalVelocity;
        if (prefabToSpawn != null)
        {
            prefabToSpawn.transform.localScale = originalScale;
        }
    }

    private IEnumerator FireTripleShot()
    {
        for (int i = 0; i < 3; i++)
        {
            spawnObject();
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void HandleAggressionMode(bool isAggressive)
    {
        TimedSpawnerRandom spawner = Object.FindAnyObjectByType<TimedSpawnerRandom>();
        if (spawner != null)
        {
            if (isAggressive)
            {
                spawner.SetSpawnInterval(0.3f, 0.5f); // Adjust spawn rate for Aggression Mode
                Debug.Log("Aggressive mode activated: Faster enemy spawns.");
            }
            else
            {
                spawner.SetSpawnInterval(1.0f, 1.5f); // Reset to normal spawn rate
                Debug.Log("Aggressive mode deactivated: Normal enemy spawns.");
            }
        }
        else
        {
            Debug.LogWarning("No TimedSpawnerRandom found in the scene. Aggression mode changes will not take effect.");
        }
    }

    protected override GameObject spawnObject()
    {
        GameObject newObject = base.spawnObject();
        newObject.transform.position = this.transform.position;
        newObject.transform.localScale = prefabToSpawn.transform.localScale;

        Mover mover = newObject.GetComponent<Mover>();
        if (mover != null)
        {
            mover.SetVelocity(velocityOfSpawnedObject);
        }

        ScoreAdder newObjectScoreAdder = newObject.GetComponent<ScoreAdder>();
        if (newObjectScoreAdder != null)
        {
            newObjectScoreAdder.SetScoreField(scoreField).SetPointsToAdd(pointsToAdd);
        }

        return newObject;
    }
}
