using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using static GlobalVariables;

public class StackManager : MonoBehaviour
{

    // how far we tile go 
    private const float BOUNDS_SIZE = 3.5f;

    private const float FAR_TO_GO = 5f;

    // start speed
    private const float STACK_MOVING_SPEED = 5.0f;
    
    // how much we want to extend cube of combo
    private const float STACK_BOUND_GAIN = 0.25f;
    
    // speed gain on combo
    private const float SPEED_GAIN = 1.05f;

    // when combo starts
    private const int COMBO_START_GAIN = 4;

    [Header("Settings")]
    // use to detect how precisy we are for combo
    [SerializeField] private float ERROR_MARGIN = 0.25f;

    [Header("UI")]
    [SerializeField] private Text scoreText;
    // public GameObject endPanel;

    [Header("Particles")]
    [SerializeField] private GameObject _comboEffect;
    [SerializeField] private float _comboEffectLength = 4f;
    [SerializeField] private GameObject _newScoreEffect;

    [Header("Tiles")]
    [SerializeField] private Color32[] gameColors = new Color32[4];

    [SerializeField] private Material stackMat;
    
    private GameObject[] theStack;
    private Vector2 stackBounds;

    private int _stackIndex;
    private int _currentScore = 0;
    private int _comboCounter = 0;

    private float tileTransition = 0.0f;
    private float tileSpeed = 2.5f;
    private float secondaryPosition;

    private bool _isMovingOnX = true;
    private bool _isGameOver;

    private Vector3 desiredStackPosition;
    private Vector3 lastTilePosition;

    void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }

        _isGameOver = false;
        
        stackBounds = new Vector2(BOUNDS_SIZE, BOUNDS_SIZE);

        theStack = new GameObject[transform.childCount];
        for (int i = 0; i < theStack.Length; i++)
        {
            theStack[i] = transform.GetChild(i).gameObject;
            ColorMesh(theStack[i].GetComponent<MeshFilter>().mesh);
        }

        // we start with first tile on top
        _stackIndex = theStack.Length - 1;

    }

    void Update()
    {
        if (_isGameOver)
        {
            return;
        }
            
        if (Input.GetMouseButtonDown(0))
        {
            if (PlaceTile())
            {
                SpawnTile();
                scoreText.text = _currentScore.ToString();
            }
            else
            {
                EndGame();
            }

        }

        MoveTile();

        // Move whole stack down to stay in camera range
        transform.position = Vector3.Lerp(transform.position, desiredStackPosition, STACK_MOVING_SPEED * Time.deltaTime);

    }

    private void CreateRubble(Vector3 pos, Vector3 scale)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        go.AddComponent<Rigidbody>();

        go.GetComponent<MeshRenderer>().material = stackMat;
        ColorMesh(go.GetComponent<MeshFilter>().mesh);
    }

    private void SpawnTile()
    {

        lastTilePosition = theStack[_stackIndex].transform.localPosition;

        _stackIndex--;

        if (_stackIndex < 0)
        {
            _stackIndex = theStack.Length - 1;
        }
            
        desiredStackPosition = Vector3.down * _currentScore;
        theStack[_stackIndex].transform.localPosition = new Vector3(_isMovingOnX ? FAR_TO_GO : 0f, _currentScore, !_isMovingOnX ? FAR_TO_GO : 0f);
        theStack[_stackIndex].transform.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

        ColorMesh(theStack[_stackIndex].GetComponent<MeshFilter>().mesh);

        _currentScore++;

        // tileTransition = 1f;
    }

    private void MoveTile()
    {
        tileTransition += Time.deltaTime * tileSpeed;

        if (_isMovingOnX)
        {
            theStack[_stackIndex].transform.localPosition = new Vector3(Mathf.Sin(tileTransition) * FAR_TO_GO, _currentScore, secondaryPosition);

        }
        else
        { 
            theStack[_stackIndex].transform.localPosition = new Vector3(secondaryPosition, _currentScore, Mathf.Sin(tileTransition) * FAR_TO_GO);
        }
    }

    private bool PlaceTile()
    {
        Transform currentTranform = theStack[_stackIndex].transform;

        if (_isMovingOnX)
        {
            float deltaX = lastTilePosition.x - currentTranform.position.x;
            
            // check if position is centered
            if (Mathf.Abs(deltaX) > ERROR_MARGIN)
            {
                // reset combo
                _comboCounter = 0;


                tileSpeed = 2.5f + ((float)_currentScore / 100f);

                stackBounds.x -= Mathf.Abs(deltaX);

                if (stackBounds.x <= 0)
                {
                    return false;
                }

                float middle = lastTilePosition.x + currentTranform.localPosition.x / 2;

                currentTranform.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                CreateRubble(
                    new Vector3(currentTranform.position.x > 0 ? currentTranform.position.x + currentTranform.localScale.x / 2 : currentTranform.position.x - currentTranform.localScale.x / 2, currentTranform.position.y, currentTranform.position.z),
                    new Vector3(Mathf.Abs(deltaX), 1, currentTranform.localScale.z));
                currentTranform.localPosition = new Vector3(middle - lastTilePosition.x / 2, _currentScore, lastTilePosition.z);


            }
            else
            {
                if (_comboCounter > COMBO_START_GAIN)
                {
                    stackBounds.x += STACK_BOUND_GAIN;

                    if (stackBounds.x <= BOUNDS_SIZE)
                    {
                        StartCoroutine(destroyEffect(Instantiate(_comboEffect)));

                        float middle = lastTilePosition.x + currentTranform.localPosition.x / 2;
                        currentTranform.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                        currentTranform.localPosition = new Vector3(middle - lastTilePosition.x / 2, _currentScore, lastTilePosition.z);

                    }
                    else
                        stackBounds.x = BOUNDS_SIZE;

                }

                _comboCounter++;
                tileSpeed *= SPEED_GAIN;
                currentTranform.localPosition = new Vector3(lastTilePosition.x, _currentScore, lastTilePosition.z);


            }

        }
        else
        {

            float deltaZ = lastTilePosition.z - currentTranform.position.z;

            if (Mathf.Abs(deltaZ) > ERROR_MARGIN)
            {
                _comboCounter = 0;
                tileSpeed = 2.5f + ((float)_currentScore / 100f);

                stackBounds.y -= Mathf.Abs(deltaZ);

                if (stackBounds.y <= 0)
                    return false;


                float middle = lastTilePosition.z + currentTranform.localPosition.z / 2;

                currentTranform.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                CreateRubble(
                    new Vector3(currentTranform.position.x, currentTranform.position.y, currentTranform.position.z > 0 ? currentTranform.position.z + currentTranform.localScale.z / 2 : currentTranform.position.z - currentTranform.localScale.z / 2),
                    new Vector3(currentTranform.localScale.x, 1, Mathf.Abs(deltaZ)));
                currentTranform.localPosition = new Vector3(lastTilePosition.x, _currentScore, middle - lastTilePosition.z / 2);


            }
            else
            {

                if (_comboCounter > COMBO_START_GAIN)
                {
                    stackBounds.y += STACK_BOUND_GAIN;

                    if (stackBounds.y <= BOUNDS_SIZE)
                    {
                        StartCoroutine(destroyEffect(Instantiate(_comboEffect)));

                        float middle = lastTilePosition.z + currentTranform.localPosition.z / 2;
                        currentTranform.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                        currentTranform.localPosition = new Vector3(lastTilePosition.x, _currentScore, middle - lastTilePosition.z / 2);

                    }
                    else
                    {
                        stackBounds.y = BOUNDS_SIZE;
                    }
                }

                _comboCounter++;
                tileSpeed *= SPEED_GAIN;
                currentTranform.localPosition = new Vector3(lastTilePosition.x, _currentScore, lastTilePosition.z);
            }

        }


        secondaryPosition = _isMovingOnX ? currentTranform.localPosition.x : currentTranform.localPosition.z;

        _isMovingOnX = !_isMovingOnX;

        return true;

    }

    private IEnumerator destroyEffect(GameObject effect)
    {
        yield return new WaitForSeconds(_comboEffectLength);
        Destroy(effect);
    }

    private void EndGame()
    {
        _isGameOver = true;
        theStack[_stackIndex].AddComponent<Rigidbody>();

        bool isNewScore = SaveLoad.isNew(GAMES.STACK, null, _currentScore, true);

        string endPanelText = string.Empty;

        scoreText.gameObject.SetActive(false);
        
        if (isNewScore)
        {
            SaveLoad.set(GAMES.STACK, null, _currentScore);
            endPanelText = "NEW BEST SCORE\n" + _currentScore + "\nBEST SCORE\n" + SaveLoad.get(GAMES.STACK, null, true);
            StartCoroutine(destroyEffect((GameObject)Instantiate(_newScoreEffect)));
        }
        else
            endPanelText = "YOUR SCORE\n" + _currentScore + "\nBEST SCORE\n" + SaveLoad.get(GAMES.STACK, null, true);

        scoreText.text = endPanelText;

        FindObjectOfType<EndPanel>().ShowEndPanel(endPanelText);

    }

    private void ColorMesh(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        Color32[] colors = new Color32[vertices.Length];

        float f = Mathf.Sin(_currentScore * 0.25f);

        for (int i = 0; i < vertices.Length; i++)
        {
            colors[i] = Lerp4(gameColors[0], gameColors[1], gameColors[2], gameColors[3], f);
        }

        mesh.colors32 = colors;

    }

    private Color32 Lerp4(Color a, Color b, Color c, Color d, float transition)
    {
        if (transition < -0.33f)
            return Color.Lerp(a, b, transition / 0.33f);
        else if (transition < 0.33f)
            return Color.Lerp(b, c, (transition - 0.33f) / 0.33f);
        else
            return Color.Lerp(c, d, (transition - 0.33f) / 0.66f);
    }
}
