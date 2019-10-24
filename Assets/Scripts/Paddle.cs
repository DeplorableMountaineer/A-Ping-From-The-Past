using UnityEngine;

/// <summary>
/// Game paddle; there are two (left and right)
/// </summary>
public class Paddle : MonoBehaviour {
    [Tooltip("Movement Keys for this Paddle")]
    [SerializeField] public KeyCode[] keys;
    [Tooltip("Movement Direction Corresponding to Movement Keys")]
    [SerializeField] private float[] direction;
    [Tooltip("Paddle Speed")]
    [SerializeField] private float speed = 200;
    [Tooltip("Half the length of the paddle")]
    [SerializeField] public float halfLength = 50;
    [Tooltip("Half the width of the paddle")]
    [SerializeField] public float halfWidth = 5;
    [Tooltip("Sound when paddle hits ball")]
    [SerializeField] private AudioClip bat;

    //Ball that is in play
    private Ball _ball;
    //Game being played
    private Game _game;
    //Main camera
    private Camera _camera;
    //If true, autoplay this paddle
    public bool autoPlay;

    private void Start() {
        //Set _ball to the ball in play
        SetBall();
        //Set _game to the game being played
        _game = FindObjectOfType<Game>();
        //Set _camera to the main camera
        _camera = Camera.main;
    }

    /// <summary>
    /// Set ball to the current ball in play.
    /// </summary>
    public void SetBall() {
        _ball = FindObjectOfType<Ball>();
    }

    private void Update() {
        if(_ball && _game.state == Game.State.Play) {
            // Bounce ball off left paddle
            if(transform.position.x < 0 &&
               _ball.transform.position.x - _ball.radius <= transform.position.x + halfWidth &&
               _ball.transform.position.y + _ball.radius > transform.position.y - halfLength &&
               _ball.transform.position.y - _ball.radius < transform.position.y + halfLength) {
                _ball.velocity = new Vector2(Mathf.Abs(_ball.velocity.x)*Random.Range(.99f, 1.05f),
                    _ball.velocity.y*Random.Range(.99f, 1.05f));
                AudioSource.PlayClipAtPoint(bat, _camera.transform.position);
            }
            //Bounce ball off right paddle
            else if(transform.position.x > 0 &&
                    _ball.transform.position.x + _ball.radius >= transform.position.x - halfWidth &&
                    _ball.transform.position.y + _ball.radius > transform.position.y - halfLength &&
                    _ball.transform.position.y - _ball.radius < transform.position.y + halfLength) {
                _ball.velocity = new Vector2(-Mathf.Abs(_ball.velocity.x)*Random.Range(.99f, 1.05f),
                    _ball.velocity.y*Random.Range(.99f, 1.05f));
                AudioSource.PlayClipAtPoint(bat, _camera.transform.position);
            }
        }

        if(autoPlay) { //Move this paddle toward current height of ball
            if(_ball && _game.state == Game.State.Play) {
                float dist = _ball.transform.position.y - transform.position.y;
                if(Mathf.Abs(dist) > 3) {
                    float d = Mathf.Sign(dist);
                    MovePaddle(d);
                }

                return;
            }
        }

        if(_game.state != Game.State.Play) return;
        //Move paddle according to movement keys being pressed
        for(int i = 0; i < keys.Length; i++) {
            if(!Input.GetKey(keys[i])) continue;
            MovePaddle(direction[i]);
            break;
        }
    }

    /// <summary>
    /// Move paddle in specified direction
    /// </summary>
    /// <param name="moveDirection">Direction to move the paddle</param>
    private void MovePaddle(float moveDirection) {
        Vector2 delta = Time.deltaTime*moveDirection*speed*Vector3.up;
        if(((Vector2)transform.position + delta).y > -350 && ((Vector2)transform.position + delta).y < 350) {
            transform.Translate(delta);
        }
    }
}
