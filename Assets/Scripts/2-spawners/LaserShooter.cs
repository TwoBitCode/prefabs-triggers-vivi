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
    private string currentInput = ""; // Tracks current key sequence
    private float comboTimer;

    [Header("Weapon Key Combinations")]
    [SerializeField] private string comboForLargeLaser = "az";   // Combo for large laser
    [SerializeField] private string comboForFastLaser = "ty";   // Combo for fast laser
    [SerializeField] private string comboForTripleShot = "qwer"; // Combo for triple shot

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
            Debug.LogWarning($"No child of {gameObject.name} has a NumberField component!");
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
        // Update cooldown timer for spacebar rate limiting
        if (shotCooldownTimer > 0)
        {
            shotCooldownTimer -= Time.deltaTime;
        }

        // Check for spacebar input
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (shotsFired < maxShotsPerSecond && shotCooldownTimer <= 0)
            {
                FireLaser(); // Fire laser
                shotsFired++;

                if (shotsFired >= maxShotsPerSecond)
                {
                    shotCooldownTimer = 1f; // Disable spacebar for 1 second
                    Debug.Log("Spacebar firing disabled: Cooldown started.");
                }
            }
        }

        // Reset shotsFired after cooldown
        if (shotCooldownTimer <= 0 && shotsFired > 0)
        {
            shotsFired = 0; // Reset shot counter
        }

        // Handle keyword combinations
        foreach (char key in Input.inputString)
        {
            currentInput += key;
            comboTimer = comboTimeout;

            Debug.Log($"Combo input detected: {currentInput}");

            if (currentInput.EndsWith(comboForLargeLaser))
            {
                ActivateWeapon(comboForLargeLaser);
                currentInput = ""; // Reset combo
            }
            else if (currentInput.EndsWith(comboForFastLaser))
            {
                ActivateWeapon(comboForFastLaser);
                currentInput = ""; // Reset combo
            }
            else if (currentInput.EndsWith(comboForTripleShot))
            {
                ActivateWeapon(comboForTripleShot);
                currentInput = ""; // Reset combo
            }
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

    private void ActivateWeapon(string combination)
    {
        if (isEffectActive) return;

        if (combination == comboForTripleShot)
        {
            Debug.Log("Triple shot activated!");
            StartCoroutine(FireTripleShot());
        }
        else if (combination == comboForFastLaser)
        {
            Debug.Log("Laser speed increased!");
            StartCoroutine(ApplyOneTimeEffect(() =>
            {
                velocityOfSpawnedObject = new Vector3(0, 20f, 0);
            }));
        }
        else if (combination == comboForLargeLaser)
        {
            Debug.Log("Laser size increased!");
            StartCoroutine(ApplyOneTimeEffect(() =>
            {
                prefabToSpawn.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            }));
        }

        FireLaser(); // Fire laser immediately after effect
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
