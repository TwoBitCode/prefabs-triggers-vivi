using UnityEngine;

/**
 * This is a static class that keeps the player scores and tracks the current level between scenes.
 */
public class GAME_STATUS : MonoBehaviour
{
    public static int playerScore = 0; // Keeps track of the player's score
    public static string currentLevel = "level-1"; // Tracks the current level, default is "level-1"
}
