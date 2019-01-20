using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using static GlobalVariables;

public class SudokuController : MonoBehaviour
{

    public enum SUDOKU_DIFFICULTY
    {
        VERY_EASY, EASY, MEDIUM, HARD, VERY_HARD
    }

    private SUDOKU_DIFFICULTY currentLevel = SUDOKU_DIFFICULTY.VERY_EASY;

    private Image CurrentButtonPressedImage;

    public Text CurrentButtonPressedText
    {
        get;
        private set;
    }
    [SerializeField] private Color _pressedColor;
    [SerializeField] private GameObject numpad;
    [SerializeField] private Text timer;
    [SerializeField] private GameObject wrongCombination;
    private Timer timerTimer;

    private Text[,] areasTexts;

    private bool isGameOver;
    private bool isFirstTime;

    private void Awake()
    {
        wrongCombination.SetActive(false);
    }

    void Start()
    {
        timerTimer = FindObjectOfType<Timer>();
        timerTimer.startTimer = false;
                
        isFirstTime = true;
        isGameOver = false;
        areasTexts = new Text[9, 9];

        // for each outer square
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject square = transform.GetChild(i).gameObject;

            // for each inner square
            for (int j = 0; j < square.transform.childCount; j++)
            {
                Button btn = square.transform.GetChild(j).GetComponent<Button>();
                btn.onClick.AddListener(() => ButtonPressed());

                Text txt = square.transform.GetChild(j).GetChild(0).GetComponent<Text>();

                // Debug.Log(i / 3 * 3 + j / 3);
                // Debug.Log(i % 3 * 3 + j % 3);

                areasTexts[i / 3 * 3 + j / 3, i % 3 * 3 + j % 3] = txt;
                

            }
        }

        loadLevelAndFillData();

    }


    private void loadLevelAndFillData()
    {

        string stringOfCurrentLevel = LoadFromFile("SudokuLevels/" + currentLevel.ToString());

        Debug.Log(stringOfCurrentLevel);

        for (int i = 0; i < 81; i++)
        {

            /*int columnForOuterSquare = i / 27 * 3;
            int rowForOuterSquare = i / 3 % 3;

            int columnForInnerSquare = i % 3;
            int rowForInnerSquare = i / 9 % 3 * 3;
            
            Text currentElement = areasTexts[columnForOuterSquare + rowForOuterSquare, columnForInnerSquare + rowForInnerSquare];*/

            Text currentElement = areasTexts[i / 9, i % 9];

            if (stringOfCurrentLevel[i] == '0')
            {
                currentElement.text = string.Empty;
            }
            else
            {
                currentElement.text = stringOfCurrentLevel[i].ToString();
                Button btn = currentElement.transform.parent.gameObject.GetComponent<Button>();
                btn.interactable = false;
            }
        }
    }

    private string LoadFromFile(string txtFile)
    {

        // BROJ POLJA ZA SUDOKO
        int sudokuFields = 9 * 9;

        // OFFSET JER NA KRAJU JE \n 2 karaktera
        int newLine = 2;

        // POVEZIVANJE SA FAJLOM U RESOURCE FOLDERU
        TextAsset txtAssets = (TextAsset)Resources.Load(txtFile);

        // TEXT CELOG FAJLA U OVOM STRING-u
        string text = (txtAssets.text);

        // RANDOM BROJ ZA BROJ IGRE NA OSNOVU UKUPNOG TEXT-a
        int randomLine = Random.Range(0, (text.Length + newLine) / (sudokuFields + newLine) - 1);

        // VRACANJE ODGOVARAJUCEG STRINGA 
        return text.Substring(randomLine * sudokuFields + newLine * randomLine, sudokuFields);

    }


    public void ButtonPressed()
    {

        if (isFirstTime)
        {
            timerTimer.startTimer = true;
            isFirstTime = false;
        }

        if (enabled == false)
        {
            return;
        }

        GameObject go = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
       

        if(CurrentButtonPressedImage != null)
        {
            CurrentButtonPressedImage.color = Color.white;
        }

        CurrentButtonPressedImage = go.GetComponent<Image>();
        CurrentButtonPressedImage.color = _pressedColor;

        CurrentButtonPressedText = go.transform.GetChild(0).GetComponent<Text>();
        // enabled = false;
        // numpad.SetActive(true);

    }

    public void CheckForEnd()
    {

        bool isAllFields = !isAnyFieldEmpty();

        if(isAllFields)
        {
            if (checkHorizontal() && checkVertical() && checkSquare())
            {
                gameOver();
            }
            else
            {
                wrongCombination.SetActive(true);
            }
        }

        

    }
    
    private bool isAnyFieldEmpty()
    {
        for (int x = 0; x < areasTexts.GetLength(0); ++x)
        {
            for (int y = 0; y < areasTexts.GetLength(1); ++y)
            {
                if (areasTexts[x, y].text == string.Empty)
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    private bool checkHorizontal()
    {
        HashSet<int> values = new HashSet<int>();

        for (int x = 0; x < areasTexts.GetLength(0); ++x)
        {
            values.Clear();

            for (int y = 0; y < areasTexts.GetLength(1); ++y)
            {
                int num = int.Parse(areasTexts[x, y].text);

                if(values.Add(num) == false)
                {
                    return false;
                }
            }
        }

        return true;
    }
    
    private bool checkVertical()
    {
        HashSet<int> values = new HashSet<int>();

        for (int x = 0; x < areasTexts.GetLength(1); ++x)
        {
            values.Clear();

            for (int y = 0; y < areasTexts.GetLength(0); ++y)
            {
                int num = int.Parse(areasTexts[x, y].text);

                if (values.Add(num) == false)
                {
                    return false;
                }
            }
        }

        return true;
    }
    
    private bool checkSquare()
    {

        HashSet<int> values = new HashSet<int>();

        int yStart = 0;
        int yEnd = 3;

        for (int i = 0; i < 3; i++)
        {
            yStart = i * 3;
            yEnd = i * 3 + 3;

            values.Clear();

            for (int x = 0; x < areasTexts.GetLength(0); ++x)
            {
                for (int y = yStart; y < yEnd; ++y)
                {
                    int num = int.Parse(areasTexts[x, y].text);

                    if (values.Add(num) == false)
                    {
                        return false;
                    }
                }

                if (x % 3 == 2)
                    values.Clear();
            }
        }
        
        return true;
    }

    private void gameOver()
    {

        isGameOver = true;

        Timer t = timer.GetComponent<Timer>();
        t.enabled = false;
        int time = (int)t.getTime();

        bool isNewScore = SaveLoad.isNew(GAMES.SUDOKU, currentLevel.ToString(), time, false);

        string endPanelText = string.Empty;

        timer.gameObject.SetActive(false);
        
        if (isNewScore)
        {
            SaveLoad.set(GAMES.SUDOKU, currentLevel.ToString(), time);
            endPanelText = "NEW BEST SCORE\n" + time + "\nBEST SCORE\n" + SaveLoad.get(GAMES.SUDOKU, currentLevel.ToString(), false);
        }
        else
            endPanelText = "YOUR SCORE\n" + time + "\nBEST SCORE\n" + SaveLoad.get(GAMES.SUDOKU, currentLevel.ToString(), false);

        timer.text = endPanelText;

        FindObjectOfType<EndPanel>().ShowEndPanel(endPanelText);

        enabled = false;


    }

}
