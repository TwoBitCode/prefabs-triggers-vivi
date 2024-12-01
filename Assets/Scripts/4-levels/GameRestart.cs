using UnityEngine;
using UnityEngine.SceneManagement;

public class GameRestart : MonoBehaviour
{
    public NumberField scoreDisplay; // Reference to the NumberField component
    public Vector3 playerStartPosition = new Vector3(0, 0, 0); // Default start position for the player

    private void Start()
    {
        AssignScoreField();
    }

    private void AssignScoreField()
    {
        if (scoreDisplay == null)
        {
            scoreDisplay = Object.FindFirstObjectByType<NumberField>();

            if (scoreDisplay != null)
            {
                Debug.Log("Automatically assigned the NumberField.");
            }
            else
            {
                Debug.LogWarning("No NumberField found. Ensure it's assigned in the Inspector.");
            }
        }
    }

    public void RestartGame()
    {
        Debug.Log("Restarting the game from level-1 with 0 points.");

        // Reset the player's score
        GAME_STATUS.playerScore = 0;

        // Reset time scale in case the game was paused
        Time.timeScale = 1;

        // Reload level-1
        SceneManager.LoadScene("level-1");

        // Reassign the NumberField and reset player position after the scene loads
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "level-1")
        {
            AssignScoreField(); // Reassign the score display
            if (scoreDisplay != null)
            {
                scoreDisplay.SetNumber(0); // Reset the score display
                Debug.Log("ScoreField found and reset after scene reload.");
            }
            else
            {
                Debug.LogWarning("ScoreField still not found after scene reload.");
            }

            ResetPlayerPosition(); // Reset the player's position

            // Unsubscribe to avoid repeated calls
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void ResetPlayerPosition()
    {
        // Find the player object by its name
        GameObject player = GameObject.Find("PlayerSpaceship1"); // Match the name in the hierarchy
        if (player != null)
        {
            player.transform.position = playerStartPosition;
            Debug.Log($"Player position reset to: {playerStartPosition}");
        }
        else
        {
            Debug.LogWarning("Player object not found to reset position.");
        }
    }
}
