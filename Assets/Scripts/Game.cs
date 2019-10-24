using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// The game being played
/// </summary>
public class Game : MonoBehaviour {
    /// <summary>
    /// State of the game
    /// </summary>
    public enum State {
        Start,
        LeftPlayerServe,
        RightPlayerServe,
        Play,
        Point,
        Reset,
        Win,
        Pause,
    }

    [Tooltip("Current game state")]
    [SerializeField] public State state = State.Start;
    [Tooltip("Prefab to instantiate the ball")]
    [SerializeField] private GameObject ballPrefab;
    [Tooltip("UI text shown when it is left player's serve")]
    [SerializeField] private Text leftPlayerServe;
    [Tooltip("UI text shown when it is right player's serve")]
    [SerializeField] private Text rightPlayerServe;
    [Tooltip("UI text shown when left player wins")]
    [SerializeField] private Text leftPlayerWin;
    [Tooltip("UI text shown when right player wins")]
    [SerializeField] private Text rightPlayerWin;
    [Tooltip("Game is over then one player reaches this score")]
    [SerializeField] private int playTo = 10;
    [Tooltip("Adjust the speed of the game")]
    [SerializeField] private float timeScale = 2;
    [Tooltip("Sound to play when somebody wins")]
    [SerializeField] private AudioClip win;

    //Left player's paddle
    private Paddle _leftPaddle;
    //Right player's paddle
    private Paddle _rightPaddle;
    //Ball that is in play
    private Ball _ball;
    //If true, the left player serves next, otherwise the right player serves next
    public bool leftServing;
    //keep track of time for timed state changes
    private long _time;
    //Score objects for left and right player
    private Score[] _scores;

    //Save state when bringing up pause menu
    private State _resumeState = State.Play;
    //Pause menu object
    private PauseMenu _pauseMenu;
    //Main camera
    private Camera _camera;
    //Save score color to restore; it is highlighted when someone wins
    private Color _scoreColor;
    //Both game paddles
    private Paddle[] _paddles;

    private void Start() {
        //Set game variables
        _paddles = FindObjectsOfType<Paddle>();
        _pauseMenu = FindObjectOfType<PauseMenu>();
        _scores = FindObjectsOfType<Score>();
        _scoreColor = _scores[0].GetComponent<Text>().color;
        _camera = Camera.main;
        state = State.Start;
        //Seed random number generator
        Random.InitState((int)DateTime.Now.Ticks);
        //Hide win/serve text
        rightPlayerWin.enabled = false;
        leftPlayerWin.enabled = false;
        rightPlayerServe.enabled = false;
        leftPlayerServe.enabled = false;
        //Start with main (pause) menu
        Pause();
    }

    /// <summary>
    /// Called when point has been scored.
    /// </summary>
    /// <param name="t">Tag of the player who scored</param>
    /// <param name="s">new score</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void PointScored(string t, int s) {
        //change state
        state = State.Point;
        if(s < playTo) return;
        //if somebody won....
        AudioSource.PlayClipAtPoint(win, _camera.transform.position);
        state = State.Win;
        //Highlight winning score
        foreach(Score score in _scores) {
            if(!score.CompareTag(t)) {
                score.GetComponent<Text>().color = new Color(.608f, .522f, .49f, 0.5f);
                break;
            }
        }

        //Enable win message
        switch(t) {
            case "Left Player":
                leftPlayerWin.enabled = true;
                break;
            case "Right Player":
                rightPlayerWin.enabled = true;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// Pause the game and bring up the menu
    /// </summary>
    private void Pause() {
        _resumeState = state;
        state = State.Pause;
        _pauseMenu.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    /// <summary>
    /// Close the menu and resume the game
    /// </summary>
    public void Resume() {
        _pauseMenu.gameObject.SetActive(false);
        state = _resumeState;
        Time.timeScale = timeScale;
    }

    private void Update() {
        //If escape is pressed, pause or resume
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if(state == State.Pause) {
                Resume();
            }
            else {
                Pause();
            }

            return;
        }

        //Handle the game states
        switch(state) {
            case State.Start:
                //At start, reset score, randomly decide who serves, hide win messages, then go to reset state 
                foreach(Score score in _scores) {
                    score.ResetScore();
                    score.GetComponent<Text>().color = _scoreColor;
                }

                state = State.Reset;
                leftServing = (Random.Range(0f, 1f) > 0.5f);
                rightPlayerWin.enabled = false;
                leftPlayerWin.enabled = false;
                break;
            case State.LeftPlayerServe:
                //Show left player serve message, wait till move key pressed or, if autoplay, 3 seconds has passed,
                //then hide serve message and launch ball, transitioning to Play state
                if(_time == 0) {
                    _time = DateTime.Now.Ticks;
                }

                rightPlayerServe.enabled = false;
                leftPlayerServe.enabled = true;
                foreach(KeyCode key in _leftPaddle.keys) {
                    if(Input.GetKeyDown(key) ||
                       (_leftPaddle.autoPlay && (DateTime.Now.Ticks - _time)/10000000 >= 3)) {
                        rightPlayerServe.enabled = false;
                        leftPlayerServe.enabled = false;
                        // ReSharper disable once RedundantArgumentDefaultValue
                        _ball.Launch(1);
                        _time = 0;
                        state = State.Play;
                        break;
                    }
                }

                break;
            case State.RightPlayerServe:
                //Show Right player serve message, wait till move key pressed or, if autoplay, 3 seconds has passed,
                //then hide serve message and launch ball, transitioning to Play state
                if(_time == 0) {
                    _time = DateTime.Now.Ticks;
                }

                rightPlayerServe.enabled = true;
                leftPlayerServe.enabled = false;
                foreach(KeyCode key in _rightPaddle.keys) {
                    if(Input.GetKeyDown(key) ||
                       (_rightPaddle.autoPlay && (DateTime.Now.Ticks - _time)/10000000 >= 3)) {
                        rightPlayerServe.enabled = false;
                        leftPlayerServe.enabled = false;
                        _ball.Launch(-1);
                        state = State.Play;
                        _time = 0;
                        break;
                    }
                }

                break;
            case State.Play:
                //Game is being played
                break;
            case State.Point:
                //A point was scored
                state = State.Reset;
                break;
            case State.Win:
                //Somebody won.  Restart game when a key is pressed
                if(Input.anyKeyDown) {
                    state = State.Start;
                }

                break;
            case State.Pause:
                //Game is paused with menu showing
                break;
            case State.Reset:
                //Reset paddles, create new ball,  and prepare to serve
                foreach(Paddle paddle in _paddles) {
                    var transform1 = paddle.transform;
                    transform1.position = new Vector3(transform1.position.x, 0);
                    if(paddle.CompareTag("Left Player")) {
                        _leftPaddle = paddle;
                    }
                    else if(paddle.CompareTag("Right Player")) {
                        _rightPaddle = paddle;
                    }
                }

                if(leftServing) {
                    GameObject go = Instantiate(ballPrefab,
                        _leftPaddle.transform.position + Vector3.right*(_leftPaddle.halfWidth + 12.5f),
                        Quaternion.identity);
                    _ball = go.GetComponent<Ball>();
                    _ball.velocity = Vector2.zero;
                    state = State.LeftPlayerServe;
                }
                else {
                    GameObject go = Instantiate(ballPrefab,
                        _rightPaddle.transform.position + Vector3.left*(_rightPaddle.halfWidth + 12.5f),
                        Quaternion.identity);
                    _ball = go.GetComponent<Ball>();
                    _ball.velocity = Vector2.zero;
                    state = State.RightPlayerServe;
                }

                _leftPaddle.SetBall();
                _rightPaddle.SetBall();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
