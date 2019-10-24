using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Score keeper; two instances, one for the left player, one for the right player
/// </summary>
public class Score : MonoBehaviour {
    [Tooltip("The current score")]
    [SerializeField] private int score;
    [Tooltip("UI element to display the score")]
    [SerializeField] private Text scoreText;

    //The game being played
    private Game _game;

    private void Start() {
        //Set _game variable and set score to zero
        _game = FindObjectOfType<Game>();
        ResetScore();
    }

    /// <summary>
    /// Set score to 0 and update UI text
    /// </summary>
    public void ResetScore() {
        score = 0;
        UpdateScore();
    }

    /// <summary>
    /// Add to the score and update UI test, and notify _game to change state.
    /// </summary>
    /// <param name="amount"></param>
    public void IncrementScore(int amount = 1) {
        score += amount;
        UpdateScore();
        _game.PointScored(tag, score);
    }

    /// <summary>
    /// Return the current score
    /// </summary>
    /// <returns>The score</returns>
    public int GetScore() {
        return score;
    }

    /// <summary>
    /// Update the UI text to reflect the current score
    /// </summary>
    private void UpdateScore() {
        scoreText.text = score.ToString();
    }
}
