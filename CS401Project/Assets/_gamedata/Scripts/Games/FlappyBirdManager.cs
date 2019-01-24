using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using static GlobalVariables;

public class FlappyBirdManager : MonoBehaviour
{
    [Header("Bird setup")]
    [SerializeField] private float flapSpeed = 100f;
    [SerializeField] private float forwardSpeed = .5f;

    [Header("UI")]
    [SerializeField] private Text scoreText;

    [Space(10f)]
    [SerializeField] private GameObject _backgroundGround;
    [SerializeField] private GameObject _backgroundSky;
    [SerializeField] private GameObject _pipesParent;

    [Header("Speeds setup")]
    [SerializeField] private float _backgroundGroundSpeed = 1f;
    [SerializeField] private float _backgroundSkySpeed = 1f;
    [SerializeField] private float _pipesSpeed = 1f;

    public bool IsDead
    {
        get;
        private set;
    }

    private BoxCollider2D _boxCollider2D;
    private CircleCollider2D _circleCollider2D;
    private Animator _animator;
    private Rigidbody2D _rigidBody;
    
    private bool _didFlap;
    private int _score;

    private void Awake()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _circleCollider2D = GetComponent<CircleCollider2D>();
        _animator = GetComponentInChildren<Animator>();
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    // Use this for initialization
    void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            Screen.orientation = ScreenOrientation.Landscape;
        }

        Reset();
    }

    private void Reset()
    {
        _didFlap = false;
        IsDead = false;

        Time.timeScale = 0;

        _circleCollider2D.enabled = true;
        _boxCollider2D.enabled = false;

        _score = 0;

        for (int i = 0; i < _pipesParent.transform.childCount; i++)
        {
            _pipesParent.transform.GetChild(i).GetChild(3).gameObject.SetActive(i == 0);
        }
    }

    void Update()
    {
        if (IsDead)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            _didFlap = true;
        }

        if (Time.timeScale == 0 && Input.GetMouseButtonDown(0))
        {
            Time.timeScale = 1;
        }
    }

    void FixedUpdate()
    {

        if (IsDead)
        { 
            return;
        }

        _backgroundGround.transform.position += Vector3.left * _backgroundGroundSpeed * Time.deltaTime;
        _backgroundSky.transform.position += Vector3.left * _backgroundSkySpeed * Time.deltaTime;
        _pipesParent.transform.position += Vector3.left * _pipesSpeed * Time.deltaTime;

        if (_didFlap)
        {
            _rigidBody.AddForce(Vector2.up * flapSpeed);
            _animator.SetTrigger("DoFlap");
            _didFlap = false;
        }

        if (_rigidBody.velocity.y > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, -90, (-_rigidBody.velocity.y / 3f)));
        }
            
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsDead)
        {
            return;
        }

        _animator.SetTrigger("Death");
        dead();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (IsDead)
        {
            return;
        }

        if (collider.tag == "PipeScore")
        {
            
            int index = collider.transform.parent.GetSiblingIndex();
            Transform parent = collider.transform.parent.parent;

            index++;
            if (index >= 6)
            {
                index = 0;
            }
            
            parent.GetChild(index).GetChild(3).gameObject.SetActive(true);

            IncreaseScore(1);

            StartCoroutine(delayedDisableOfParticle(collider.gameObject.transform.parent.GetChild(3).gameObject));
            // collider.gameObject.transform.parent.GetChild(3).gameObject.SetActive(false);
        }

    }
    
    private IEnumerator delayedDisableOfParticle(GameObject go)
    {
        yield return new WaitForSeconds(.2f);
        go.SetActive(false);
    }


    private void IncreaseScore(int value)
    {
        _score += value;
        scoreText.text = "Score: " + _score;
    }

    private void dead()
    {
        IsDead = true;
        StartCoroutine(delayEnd());
    
    }

    IEnumerator delayEnd()
    {
        yield return new WaitForSecondsRealtime(1f);

        _circleCollider2D.enabled = false;
        _boxCollider2D.enabled = true;

        scoreText.gameObject.SetActive(false);

        string endPanelText = string.Empty;

        if (SaveLoad.isNew(GAMES.FLAPPY_BIRD, null, _score, true))
        {
            SaveLoad.set(GAMES.FLAPPY_BIRD, null, _score);
            endPanelText = "New high score:\n" + _score + "\nBest score:\n" + SaveLoad.get(GAMES.FLAPPY_BIRD, null, true);

        }
        else
            endPanelText = "Current score: " + _score + "\nBest score:  " + SaveLoad.get(GAMES.FLAPPY_BIRD, null, true);

        scoreText.text = endPanelText;

        FindObjectOfType<EndPanel>().ShowEndPanel(endPanelText);

    }
}
