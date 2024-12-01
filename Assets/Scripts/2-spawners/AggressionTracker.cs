using UnityEngine;

public class AggressionTracker : MonoBehaviour
{
    [SerializeField] private int shotsThreshold = 5; // Number of shots to trigger aggression
    [SerializeField] private int scoreThreshold = 10; // Points scored to trigger aggression
    [SerializeField] private float aggressionDuration = 5f; // Duration of aggressive mode

    private int shotCount = 0;
    private int currentScore = 0;
    private float aggressionTimer = 0f;

    public delegate void AggressionModeHandler(bool isAggressive);
    public static event AggressionModeHandler OnAggressionModeChanged;

    void Update()
    {
        // Monitor aggression timer
        if (aggressionTimer > 0)
        {
            aggressionTimer -= Time.deltaTime;
            if (aggressionTimer <= 0)
            {
                SetAggressionMode(false);
            }
        }
    }

    public void RegisterShot()
    {
        shotCount++;
        Debug.Log($"Shots fired: {shotCount}");

        if (shotCount >= shotsThreshold && aggressionTimer <= 0)
        {
            TriggerAggression();
        }
    }

    public void RegisterScore(int points)
    {
        currentScore += points;
        Debug.Log($"Score updated: {currentScore}");

        if (currentScore >= scoreThreshold && aggressionTimer <= 0)
        {
            TriggerAggression();
        }
    }

    private void TriggerAggression()
    {
        Debug.Log("Aggression Mode Triggered!");
        SetAggressionMode(true);
        aggressionTimer = aggressionDuration;

        // Reset counters
        shotCount = 0;
        currentScore = 0;
    }

    private void SetAggressionMode(bool isAggressive)
    {
        Debug.Log($"Aggression Mode: {(isAggressive ? "Activated" : "Deactivated")}");
        OnAggressionModeChanged?.Invoke(isAggressive);
    }
}
