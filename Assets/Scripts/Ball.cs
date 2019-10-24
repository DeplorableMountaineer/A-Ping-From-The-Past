using UnityEngine;

/// <summary>
/// Game ball
/// </summary>
public class Ball : MonoBehaviour {
    [Tooltip("Ball speed")]
    [SerializeField] private float speed = 200;
    [Tooltip("Ball radius")]
    [SerializeField] public float radius = 12.5f;
    [Tooltip("Sound when ball bounces off ceiling or floor")]
    [SerializeField] private AudioClip bounce;
    [Tooltip("Sound when ball goes out of bounds")]
    [SerializeField] private AudioClip lose;
    
    /// <summary>
    /// Current ball velocity
    /// </summary>
    public Vector2 velocity = Vector2.zero;
    
    /// <summary>
    /// Score object for the left player
    /// </summary>
    private Score _leftScore;
    
    /// <summary>
    /// Score object for the right player
    /// </summary>
    private Score _rightScore;
    
    /// <summary>
    /// Game being played now
    /// </summary>
    private Game _game;
    
    /// <summary>
    /// Main camera
    /// </summary>
    private Camera _camera;
    
    /// <summary>
    /// Left player's paddle
    /// </summary>
    private Paddle _leftPaddle;
    
    /// <summary>
    /// Right player's paddle
    /// </summary>
    private Paddle _rightPaddle;

    private void Start() {
        //Set up variables
        _camera = Camera.main;
        _game = FindObjectOfType<Game>();
        foreach(Score s in FindObjectsOfType<Score>()) {
            if(s.CompareTag("Left Player")) {
                _leftScore = s;
            }
            else if(s.CompareTag("Right Player")) {
                _rightScore = s;
            }
        }

        foreach(Paddle paddle in FindObjectsOfType<Paddle>()) {
            var transform1 = paddle.transform;
            transform1.position = new Vector3(transform1.position.x, 0);
            if(paddle.CompareTag("Left Player")) {
                _leftPaddle = paddle;
            }
            else if(paddle.CompareTag("Right Player")) {
                _rightPaddle = paddle;
            }
        }
    }

    /// <summary>
    /// Launch the ball in the given direction with a random angle
    /// </summary>
    /// <param name="direction">Direction to launch</param>
    public void Launch(float direction = 1) {
        velocity = (Vector2.right*direction + Mathf.Sign(Random.Range(-1f, 1f))*Random.Range(0.5f, 1f)*Vector2.up)
                   .normalized*speed;
    }

    private void Update() {
        if(_game.state != Game.State.Play) {
            return;
        }

        //Move the ball
        transform.Translate(velocity*Time.deltaTime);
        
        //Bounce ball off floor
        if(velocity.magnitude > 0 && transform.position.y - radius < -384) {
            AudioSource.PlayClipAtPoint(bounce, _camera.transform.position);
            velocity = new Vector2(velocity.x*Random.Range(.99f, 1.05f),
                Mathf.Abs(velocity.y)*Random.Range(.99f, 1.05f));
        }
        //Bounce ball off ceiling
        else if(velocity.magnitude > 0 && transform.position.y + radius > 384) {
            AudioSource.PlayClipAtPoint(bounce, _camera.transform.position);
            velocity = new Vector2(velocity.x*Random.Range(.99f, 1.05f),
                -Mathf.Abs(velocity.y)*Random.Range(.99f, 1.05f));
        }

        //Check out of bounds left
        if(velocity.magnitude > 0 && transform.position.x - radius < -500 &&
           (transform.position.y + radius <= _leftPaddle.transform.position.y - _leftPaddle.halfLength ||
            transform.position.y - radius >= _leftPaddle.transform.position.y + _leftPaddle.halfLength)) {
            AudioSource.PlayClipAtPoint(lose, _camera.transform.position);
            _rightScore.IncrementScore();
            _game.leftServing = true;
            Destroy(gameObject);
        }
        //Check out of bounds right
        else if(velocity.magnitude > 0 && transform.position.x + radius > 500 &&
                (transform.position.y + radius <= _rightPaddle.transform.position.y - _rightPaddle.halfLength ||
                 transform.position.y - radius >= _rightPaddle.transform.position.y + _rightPaddle.halfLength)) {
            AudioSource.PlayClipAtPoint(lose, _camera.transform.position);
            _leftScore.IncrementScore();
            _game.leftServing = false;
            Destroy(gameObject);
        }
    }
}
