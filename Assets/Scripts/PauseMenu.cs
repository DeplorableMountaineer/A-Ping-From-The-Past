using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Menu shown at beginning of game, and when game is paused; contains settings and instructions.
/// </summary>
public class PauseMenu : MonoBehaviour {
    [Tooltip("UI element to select autoplay for left player")]
    [SerializeField] Toggle leftAutoplayToggle;
    [Tooltip("UI element to select autoplay for right player")]
    [SerializeField] Toggle rightAutoplayToggle;
    [Tooltip("UI element to set volume")]
    [SerializeField] Slider volumeSlider;

    private void Start() {
        //Restore preferences from last run
        LoadPrefs();
    }

    /// <summary>
    /// Set left paddle to autoplay according to value of UI element
    /// </summary>
    public void SetLeftAutoPlay() {
        foreach(Paddle paddle in FindObjectsOfType<Paddle>()) {
            if(!paddle.CompareTag("Left Player")) continue;
            paddle.autoPlay = leftAutoplayToggle.isOn;
            break;
        }
    }

    /// <summary>
    /// Set right paddle to autoplay according to value of UI element
    /// </summary>
    public void SetRightAutoPlay() {
        foreach(Paddle paddle in FindObjectsOfType<Paddle>()) {
            if(!paddle.CompareTag("Right Player")) continue;
            paddle.autoPlay = rightAutoplayToggle.isOn;
            break;
        }
    }

    /// <summary>
    /// Resume game after pausing.  Save preferences to disk first.
    /// </summary>
    public void Resume() {
        SavePrefs();
        FindObjectOfType<Game>().Resume();
    }

    /// <summary>
    /// Quit the game, after saving preferences
    /// </summary>
    public void Quit() {
        SavePrefs();
        Application.Quit();
    }

    /// <summary>
    /// Set game volume according to UI volume slider setting.
    /// </summary>
    public void ChangeVolume() {
        AudioListener.volume = volumeSlider.value;
    }

    /// <summary>
    /// Load preferences from disk
    /// </summary>
    private void LoadPrefs() {
        if(PlayerPrefs.HasKey("Volume")) {
            AudioListener.volume = PlayerPrefs.GetFloat("Volume");
        }
        else {
            AudioListener.volume = 0.5f;
        }

        volumeSlider.SetValueWithoutNotify(AudioListener.volume);

        foreach(Paddle paddle in FindObjectsOfType<Paddle>()) {
            if(paddle.CompareTag("Left Player")) {
                if(PlayerPrefs.HasKey("LeftAutoplay")) {
                    paddle.autoPlay = PlayerPrefs.GetInt("LeftAutoplay") > 0;
                }
                else {
                    paddle.autoPlay = false;
                }

                leftAutoplayToggle.SetIsOnWithoutNotify(paddle.autoPlay);
            }
            else if(paddle.CompareTag("Right Player")) {
                if(PlayerPrefs.HasKey("RightAutoplay")) {
                    paddle.autoPlay = PlayerPrefs.GetInt("RightAutoplay") > 0;
                }
                else {
                    paddle.autoPlay = false;
                }

                rightAutoplayToggle.SetIsOnWithoutNotify(paddle.autoPlay);
            }
        }
    }

    /// <summary>
    /// Save preferences to disk
    /// </summary>
    private void SavePrefs() {
        PlayerPrefs.SetFloat("Volume", AudioListener.volume);
        foreach(Paddle paddle in FindObjectsOfType<Paddle>()) {
            if(paddle.CompareTag("Left Player")) {
                PlayerPrefs.SetInt("LeftAutoplay", paddle.autoPlay ? 1 : 0);
            }
            else if(paddle.CompareTag("Right Player")) {
                PlayerPrefs.SetInt("RightAutoplay", paddle.autoPlay ? 1 : 0);
            }
        }

        PlayerPrefs.Save();
    }
}
