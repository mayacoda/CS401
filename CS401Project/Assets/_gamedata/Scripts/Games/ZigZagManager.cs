using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using static GlobalVariables;

public class ZigZagManager : MonoBehaviour
{
    private const int POINTSFORPICKUP = 10;
    private const float SPEEDGAINTIME = 10;
    private const float NUMBER_OF_TILES = 50;

    [SerializeField] private GameObject effect;

    [SerializeField] private GameObject start;

    [SerializeField] private Text scoreText;

    [SerializeField] private GameObject _newScoreEffect;

    [Header("TILES")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject tileParent;

    private Vector3 _dir;

    private float _speed;
    private int _score;
    private bool _isDead;

    private float _currentTime;

    private int _tilesPassed;

    // Use this for initialization
    void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }

        Reset();

        for (int i = 0; i < NUMBER_OF_TILES; i++)
        {
            GameObject tile = Instantiate(tilePrefab,
                tileParent.transform.GetChild(tileParent.transform.childCount - 1).transform.GetChild(Random.Range(0, 2)).position, // take last child, and take random attach point
                Quaternion.identity, tileParent.transform);

            // ACTIVATE DIAMOND (20% chance)
            if (Random.Range(0, 5) == 0)
            {
                tile.transform.GetChild(3).gameObject.SetActive(true);
            }

        }
    }

    private void ChangeTilePosition()
    {
        GameObject tile = tileParent.transform.GetChild(0).gameObject;
        tile.transform.position = tileParent.transform.GetChild(tileParent.transform.childCount - 1).transform.GetChild(Random.Range(0, 2)).position;
        tile.transform.SetSiblingIndex(tileParent.transform.childCount - 1);

        if (Random.Range(0, 5) == 0)
        {
            tile.transform.GetChild(3).gameObject.SetActive(true);
        }
    }

    private void Reset()
    {
        _currentTime = 0;
        _speed = 15;
        _score = 0;
        _dir = Vector3.zero;
        _isDead = false;
        _tilesPassed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isDead)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {

            if (_dir == Vector3.back)
            {
                _dir = Vector3.right;
            }
            else
            {
                _dir = Vector3.back;
            }
        }

        if (transform.position.y < 3.3)
        {
            dead();
        }

        if (SPEEDGAINTIME < _currentTime)
        {
            _speed++;
            _currentTime = 0;
        }
        else
        {
            _currentTime += Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        if (_isDead)
        {
            if (_dir == Vector3.back)
            {
                _dir = Vector3.forward;
            }
            else if (_dir == Vector3.right)
            {
                _dir = Vector3.left;
            }

            transform.Translate(_dir * _speed * Time.deltaTime);

            return;
        }

        tileParent.transform.Translate(_dir * _speed * Time.deltaTime);
        start.transform.Translate(_dir * _speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {

        if (_isDead)
        {
            return;
        }

        if (other.tag == "Point")
        {
            IncreaseScore(POINTSFORPICKUP);

            // deactivate point
            other.gameObject.SetActive(false);

            // show particle
            StartCoroutine(showAndDestroyEffect((GameObject)Instantiate(effect, other.gameObject.transform.position, Quaternion.identity, other.gameObject.transform.parent)));
        }
    }

    private void IncreaseScore(int value)
    {
        _score += value;
        scoreText.text = "Score: " + _score;
    }

    private IEnumerator showAndDestroyEffect(GameObject effect)
    {
        yield return new WaitForSeconds(effect.GetComponent<ParticleSystem>().main.duration);
        Destroy(effect);
    }

    void OnTriggerExit(Collider other)
    {
        if (_isDead)
        {
            return;
        }

        if (other.tag == "Tile")
        {
            _tilesPassed++;

            // for tile "pooling", don't create new tile, just use existing one which is already passed and put it at end
            if (_tilesPassed > NUMBER_OF_TILES / 2)
            {
                ChangeTilePosition();
            }

            RaycastHit hit;
            Ray downRay = new Ray(transform.position, Vector3.down);

            if (!Physics.Raycast(downRay, out hit))
            {
                // KILL PLAYER!!!
                dead();

            }
            else
            {
                IncreaseScore(1);
            }
        }
    }

    private void dead()
    {
        _isDead = true;

        string endPanelText = string.Empty;

        scoreText.gameObject.SetActive(false);


        if (SaveLoad.isNew(GAMES.ZIG_ZAG, null, _score, true))
        {
            SaveLoad.set(GAMES.ZIG_ZAG, null, _score);
            endPanelText = "New high score\n " + _score + "\nBest score\n  " + SaveLoad.get(GAMES.ZIG_ZAG, null, true);
            StartCoroutine(destroyEffect((GameObject)Instantiate(_newScoreEffect)));

        }
        else
            endPanelText = "Current score\n " + _score + "\nBest score\n  " + SaveLoad.get(GAMES.ZIG_ZAG, null, true);

        scoreText.text = endPanelText;

        scoreText.transform.SetSiblingIndex(30);

        FindObjectOfType<EndPanel>().ShowEndPanel(endPanelText);


    }

    private IEnumerator destroyEffect(GameObject effect)
    {
        yield return new WaitForSeconds(3);
        Destroy(effect);
    }

}
