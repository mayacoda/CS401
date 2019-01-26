using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using static GlobalVariables;

// [ExecuteInEditMode]
public class YambController : MonoBehaviour
{

    [SerializeField] private Button _btnPrefab;

    [SerializeField] private GameObject _dicesParent;
    [SerializeField] Text scoreText;

    /// <summary>
    /// spritovi za polja obratiti paznju da su isti indexi
    /// 0 fill, 1 pressable, 2 sum, 3 AD, 4 free, 5 AU, 6 N, 7 empty
    /// </summary>
    [SerializeField] private Sprite[] _sprites;

    private const int spriteFILLEDindex = 0;
    private const int spritePRESSABLEindex = 1;
    private const int spriteSUMindex = 2;
    private const int spriteADindex = 3;
    private const int spriteFREEindex = 4;
    private const int spriteAUindex = 5;
    private const int spriteNindex = 6;
    private const int spriteNOCOLORorEMPTYindex = 7;


    private const int NUMBER_OF_COLUMNS = 6;        // 4 playable + 1 for numbers/sum + 1 for total sum
    private const int NUMBER_OF_ROWS = 18;          // 13 playable + 3 for sum + 1 total sum + 1 top panel with arrows

    private const int MAXindex = 8;
    private const int MINindex = 9;
    private const int KENTAindex = 11;
    private const int TRILINGindex = 12;
    private const int FULLHOUSEindex = 13;
    private const int POKERindex = 14;
    private const int YAMBindex = 15;

    private const int NUMBER_GAINER_BOUNDRY = 60;
    private const int SCORE_FOR_ABOVE_NUMBER_GAINER_BOUNDRY = 30;
    private const int KENTApointLow = 40;
    private const int KENTApointHigh = 50;
    private const int TRILINGpoint = 30;
    private const int FULLHOUSEpoint = 40;
    private const int POKERpoint = 50;
    private const int YAMBpoint = 70;

    private const int ARROWDOWNcolumnIndex = 1;
    private const int FREEcolumnIndex = 2;
    private const int ARROWUPcolumnIndex = 3;
    private const int NAJAVAcolumnIndex = 4;

    private int[] SUMrowIndices = new int[] { 7, 10, 16, 17 };

    private RollDices _rollDices;

    /// <summary>
    /// Buttons by ROW/COLUMN
    /// </summary>
    private Button[,] buttonsArray;
    private Button najavaButton;

    private int countForFillFields;

    private bool _isGameOver;

    public bool _isNajava
    {
        get;
        private set;
    }

    public bool _onlyNajavaRemained
    {
        get;
        private set;
    }

    private int _counterForNoNajavaFields;

    // Use this for initialization
    void Start()
    {
        scoreText.gameObject.SetActive(false);

        _rollDices = FindObjectOfType<RollDices>();

        _counterForNoNajavaFields = 0;
        _onlyNajavaRemained = false;
        countForFillFields = 0;
        _isNajava = false;
        _isGameOver = false;

        buttonsArray = new Button[NUMBER_OF_ROWS, NUMBER_OF_COLUMNS];

        for (int i = 0; i < NUMBER_OF_ROWS; i++)
        {
            for (int j = 0; j < NUMBER_OF_COLUMNS; j++)
            {
                // SVAKO DUGME IMA TAG Touchable koji mi menjamo kada nije touchable
                Button button = Instantiate(_btnPrefab, this.transform, false) as Button;

                button.transform.GetChild(0).GetComponent<Text>().color = Color.black;

                // important for knowing which row and column we are
                button.name = i + "-" + j;

                button.GetComponent<Button>().onClick.AddListener(canWeAddOnThisCell);

                buttonsArray[i, j] = button;

                //////// SET IMAGE! ////////

                // pressable
                button.GetComponent<Image>().sprite = _sprites[spritePRESSABLEindex];

                if(j == 0 || SUMrowIndices.Contains(i))
                {
                    button.GetComponentInChildren<Text>().color = Color.white;
                }

                // EMPTY CELL
                 if ((j == 0 && i == 0) || (i == NUMBER_OF_ROWS - 1 && j != NUMBER_OF_COLUMNS - 1) ||
                     (j == NUMBER_OF_COLUMNS - 1 && !(i == SUMrowIndices[0] || i == SUMrowIndices[1] || i == SUMrowIndices[2] || i == SUMrowIndices[3])))
                {
                    button.GetComponent<Image>().sprite = _sprites[spriteNOCOLORorEMPTYindex];
                    //  button.GetComponentInChildren<Text>().text = "P";
                    button.tag = "Untagged";
                }

                // filled (za brojeve, slike, zbir, ..)
                else if (j == 0 || j == NUMBER_OF_COLUMNS - 1 || i == 0 || i == SUMrowIndices[0] || i == SUMrowIndices[1] || i == SUMrowIndices[2])
                {
                    button.enabled = false;
                    button.GetComponent<Image>().sprite = _sprites[spriteFILLEDindex];
                    // button.GetComponentInChildren<Text>().text = "P";
                    button.tag = "Untagged";
                }
            }
        }

        // postaviti text za brojeve i za ostale

        for (int i = 1; i < 7; i++)
        {
            buttonsArray[i, 0].transform.GetChild(0).GetComponent<Text>().text = i.ToString();
            buttonsArray[i, 0].transform.GetChild(0).GetComponent<Text>().color = Color.white;
        }

        buttonsArray[MAXindex, 0].transform.GetChild(0).GetComponent<Text>().text = "Max";
        buttonsArray[MAXindex, 0].transform.GetChild(0).GetComponent<Text>().color = Color.white;
        buttonsArray[MINindex, 0].transform.GetChild(0).GetComponent<Text>().text = "Min";
        buttonsArray[MINindex, 0].transform.GetChild(0).GetComponent<Text>().color = Color.white;
        buttonsArray[KENTAindex, 0].transform.GetChild(0).GetComponent<Text>().text = "Kenta";
        buttonsArray[KENTAindex, 0].transform.GetChild(0).GetComponent<Text>().color = Color.white;
        buttonsArray[TRILINGindex, 0].transform.GetChild(0).GetComponent<Text>().text = "Triling";
        buttonsArray[TRILINGindex, 0].transform.GetChild(0).GetComponent<Text>().color = Color.white;
        buttonsArray[FULLHOUSEindex, 0].transform.GetChild(0).GetComponent<Text>().text = "Ful";
        buttonsArray[FULLHOUSEindex, 0].transform.GetChild(0).GetComponent<Text>().color = Color.white;
        buttonsArray[POKERindex, 0].transform.GetChild(0).GetComponent<Text>().text = "Poker";
        buttonsArray[POKERindex, 0].transform.GetChild(0).GetComponent<Text>().color = Color.white;
        buttonsArray[YAMBindex, 0].transform.GetChild(0).GetComponent<Text>().text = "Yamb";
        buttonsArray[YAMBindex, 0].transform.GetChild(0).GetComponent<Text>().color = Color.white;

        for (int i = 0; i < SUMrowIndices.Length; i++)
        {
            buttonsArray[SUMrowIndices[i], 0].GetComponent<Image>().sprite = _sprites[spriteSUMindex];
            buttonsArray[SUMrowIndices[i], 0].name = "sum " + i;
        }

        buttonsArray[0, ARROWDOWNcolumnIndex].GetComponent<Image>().sprite = _sprites[spriteADindex];
        buttonsArray[0, ARROWDOWNcolumnIndex].name = "arrowdown";

        buttonsArray[0, FREEcolumnIndex].GetComponent<Image>().sprite = _sprites[spriteFREEindex];
        buttonsArray[0, FREEcolumnIndex].name = "free";

        buttonsArray[0, ARROWUPcolumnIndex].GetComponent<Image>().sprite = _sprites[spriteAUindex];
        buttonsArray[0, ARROWUPcolumnIndex].name = "arrowup";

        buttonsArray[0, NAJAVAcolumnIndex].GetComponent<Image>().sprite = _sprites[spriteNindex];
        buttonsArray[0, NAJAVAcolumnIndex].name = "najava";


        // JUST FOR TESTING
        /*
		for (int i = 1; i < 6; i++) {
			for (int j = 1; j < 5; j++) {
				buttonsArray [i, j].transform.GetChild (0).GetComponent<Text> ().text = i.ToString ();
				countForFillFields++;
				_counterForNoNajavaFields++;
			}
		}

        for (int i = 9; i <= 9; i++)
        {
            for (int j = 1; j < 5; j++)
            {
                buttonsArray[i, j].transform.GetChild(0).GetComponent<Text>().text = i.ToString();
                countForFillFields++;
                _counterForNoNajavaFields++;
            }
        }

        for (int i = 11; i < 15; i++) {
			for (int j = 1; j < 5; j++) {
				buttonsArray [i, j].transform.GetChild (0).GetComponent<Text> ().text = i.ToString ();
				countForFillFields++;
                _counterForNoNajavaFields++;
			}
		}*/
		Debug.Log (_counterForNoNajavaFields.ToString () + " OVDE ");
		
    }


    private void canWeAddOnThisCell()
    {
        if (_rollDices.currentNumberOfRoll == 0)
        {
            Debug.Log("PRVO MORATE ROLATI");
            return;

        }

        // da vec dugme nije popunjeno
        if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>().text != string.Empty)
        {
            Debug.Log("POLJE VEC POPUNJENO");
            return;
        }
            
        // IME DUGMETA
        string buttonName = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name;

        // da se ugasi dok smo u prvom roll-u a ipak smo se predomislili za polje koje hocemo da najavimo
        if (buttonName == "najava")
        {

            if(_isNajava && _rollDices.currentNumberOfRoll == 1)
            {
                UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color = Color.white;
                _isNajava = false;
                najavaButton.GetComponent<Image>().color = Color.white;
                najavaButton = null;
            }

            return;
        }
        
        // PODELITI IME DUGMETA SA - zato sto je u imenu oznaceno koja je pozicija tog dugmeta
        string[] splitForRowColumn = buttonName.Split('-');

        int currentCellRow = int.Parse(splitForRowColumn[0]);
        int currentCellColumn = int.Parse(splitForRowColumn[1]);

        if (!_isNajava)
        {
            if (currentCellColumn == ARROWDOWNcolumnIndex)
            {
                for (int i = 1; i <= YAMBindex; i++)
                {

                    if (buttonsArray[i, currentCellColumn].tag != "Touchable")
                    {
                        continue;
                    }

                    // LOGIC FOR ARROWDOWN
                    if (buttonsArray[i, currentCellColumn].transform.GetChild(0).GetComponent<Text>().text == string.Empty && i != currentCellRow)
                    {
                        Debug.Log("Prvo popuniti prethodna polja");
                        break;
                    }
                    else if (i == currentCellRow)
                    {
                        buttonsArray[i, currentCellColumn].transform.GetChild(0).GetComponent<Text>().text = resultCountForCell(currentCellRow);
                        _rollDices.ResetDices();
                        countForFillFields++;
                        _counterForNoNajavaFields++;
                        break;
                    }
                }


            }
            else if (currentCellColumn == FREEcolumnIndex)
            {
                if (buttonsArray[currentCellRow, currentCellColumn].transform.GetChild(0).GetComponent<Text>().text == "")
                {
                    buttonsArray[currentCellRow, currentCellColumn].transform.GetChild(0).GetComponent<Text>().text = resultCountForCell(currentCellRow);
                    _rollDices.ResetDices();
                    countForFillFields++;
                    _counterForNoNajavaFields++;
                }
                else
                {
                    Debug.Log("Vec popunjeno polje");
                }
            }
            else if (currentCellColumn == ARROWUPcolumnIndex)
            {

                for (int i = YAMBindex; i >= 1; i--)
                {

                    if (buttonsArray[i, currentCellColumn].tag != "Touchable")
                    {
                        continue;
                    }

                    // LOGIC FOR ARROWDUP

                    if (buttonsArray[i, currentCellColumn].transform.GetChild(0).GetComponent<Text>().text == "" && i != currentCellRow)
                    {
                        Debug.Log("Prvo popuniti prethodna polja");
                        break;
                    }
                    else if (i == currentCellRow)
                    {
                        buttonsArray[i, currentCellColumn].transform.GetChild(0).GetComponent<Text>().text = resultCountForCell(currentCellRow);
                        _rollDices.ResetDices();
                        countForFillFields++;
                        _counterForNoNajavaFields++;
                        break;
                    }
                }

            }
        }
        
        if (currentCellColumn == NAJAVAcolumnIndex)
        {
            if (!_isNajava && buttonsArray[currentCellRow, currentCellColumn].transform.GetChild(0).GetComponent<Text>().text != "")
            {
                Debug.Log("vec popunjeno polje");
                return;
            }

            // mozda isNajava ovde ne treba
            if (_rollDices.currentNumberOfRoll == 1 && !_isNajava)
            {
                _isNajava = true;
                najavaButton = buttonsArray[currentCellRow, currentCellColumn];
                najavaButton.GetComponent<Image>().color = Color.gray;

                buttonsArray[0, NAJAVAcolumnIndex].enabled = true;
                buttonsArray[0, NAJAVAcolumnIndex].GetComponent<Image>().color = Color.gray;
                return;
            }

            if (!_isNajava)
            {
                return;
            }

            if (buttonsArray[currentCellRow, currentCellColumn] == najavaButton)
            {
                najavaButton.transform.GetChild(0).GetComponent<Text>().text = resultCountForCell(currentCellRow);
                _rollDices.ResetDices();
                countForFillFields++;
                _isNajava = false;

                najavaButton.GetComponent<Image>().color = Color.white;
                najavaButton = null;

                buttonsArray[0, NAJAVAcolumnIndex].GetComponent<Image>().color = Color.white;
            }
        }

        // PROMENIO SE STATUS NEKE CELIJE PROVERITI ZBIR
        if (_rollDices.currentNumberOfRoll == 0)
        {
            int currentSumIndex = -1;

            AKOJEJEDINICIAUPISANA:
            bool isAllFieldsFilled = true;
            int sum = 0;

            if (currentCellRow < SUMrowIndices[0])
            {
                currentSumIndex = SUMrowIndices[0];
                for (int i = 1; i <= 6; i++)
                {
                    if (buttonsArray[i, currentCellColumn].transform.GetChild(0).GetComponent<Text>().text == "")
                    {
                        isAllFieldsFilled = false;
                        break;
                    }
                    else
                    {
                        sum += int.Parse(buttonsArray[i, currentCellColumn].transform.GetChild(0).GetComponent<Text>().text);
                    }
                }

                if (sum >= NUMBER_GAINER_BOUNDRY)
                {
                    sum += SCORE_FOR_ABOVE_NUMBER_GAINER_BOUNDRY;
                }
            }
            else if (currentCellRow < SUMrowIndices[1])
            {
                currentSumIndex = SUMrowIndices[1];

                if (buttonsArray[MAXindex, currentCellColumn].transform.GetChild(0).GetComponent<Text>().text == string.Empty ||
                    buttonsArray[MINindex, currentCellColumn].transform.GetChild(0).GetComponent<Text>().text == string.Empty ||
                    buttonsArray[1, currentCellColumn].transform.GetChild(0).GetComponent<Text>().text == string.Empty) // provera da li je i jedinica upisana
                {
                    isAllFieldsFilled = false;
                }
                else
                {
                    sum += int.Parse(buttonsArray[MAXindex, currentCellColumn].transform.GetChild(0).GetComponent<Text>().text);
                    sum -= int.Parse(buttonsArray[MINindex, currentCellColumn].transform.GetChild(0).GetComponent<Text>().text);
                    if (sum > 0)
                    {
                        sum *= int.Parse(buttonsArray[1, currentCellColumn].transform.GetChild(0).GetComponent<Text>().text);
                    }
                    else
                    {
                        sum = 0;
                    }
                        
                }

            }
            else if (currentCellRow < SUMrowIndices[2])
            {
                currentSumIndex = SUMrowIndices[2];

                for (int i = KENTAindex; i <= YAMBindex; i++)
                {
                    if (buttonsArray[i, currentCellColumn].transform.GetChild(0).GetComponent<Text>().text == string.Empty)
                    {
                        isAllFieldsFilled = false;
                        break;
                    }
                    else
                    {
                        sum += int.Parse(buttonsArray[i, currentCellColumn].transform.GetChild(0).GetComponent<Text>().text);
                    }
                }
            }
            else
            {
                isAllFieldsFilled = false; // NIKAD NECE DA SE JAVI samo mera predostroznosti ako dodamo nesto u buducnosti
            }

            if (isAllFieldsFilled)
            {
                buttonsArray[currentSumIndex, currentCellColumn].transform.GetChild(0).GetComponent<Text>().text = sum.ToString();
                
            }

            if (currentCellRow == 1)
            {
                currentCellRow = SUMrowIndices[1] - 1;
                goto AKOJEJEDINICIAUPISANA; // proveri za max/min
            }

            if (countForFillFields == (NUMBER_OF_COLUMNS - 2) * (NUMBER_OF_ROWS - 5))
            {
                gameOver();
            }
        }

        if (_counterForNoNajavaFields == ((NUMBER_OF_COLUMNS - 3) * (NUMBER_OF_ROWS - 5)))
        {
            _onlyNajavaRemained = true;
        }

    }

    private string resultCountForCell(int index)
    {
        int sum = 0;

        List<int> temp = new List<int>();

        for (int i = 0; i < _dicesParent.transform.childCount; i++)
        {
            temp.Add(int.Parse(_dicesParent.transform.GetChild(i).transform.GetChild(0).GetComponent<Text>().text));
        }

        temp.Sort(); 

        // Ako je broj u pitanju
        if (index < 7)
        {
            int counter = 0;
            foreach (int i in temp)
            {
                if (i == index && counter < 5)
                {
                    sum += i;
                    counter++;
                }
            }
        }
        else if (index == MAXindex)
        {
            temp.Reverse();

            for (int i = 0; i < 5; i++)
            {
                sum += temp[i];
            }

        }
        else if (index == MINindex)
        {
            for (int i = 0; i < 5; i++)
            {
                sum += temp[i];
            }

        }
        else if (index == KENTAindex)
        {
            temp.Reverse();

            HashSet<int> justForDuplicateRemove = new HashSet<int>(temp.ToArray());
            
            if (justForDuplicateRemove.Count >= 5)
            {
                int isSix = justForDuplicateRemove.ElementAt(0) == 6 ? 6 : 5;

                bool isKenta = true;

                for (int i = isSix; i > isSix - 5; i--)
                {
                    if (justForDuplicateRemove.ElementAt(isSix - i) != i)
                    {
                        isKenta = false;
                        Debug.Log(justForDuplicateRemove.ElementAt(i - 1) + "\t" + i);
                    }
                }

                if (isKenta)
                {
                    sum = isSix == 6 ? KENTApointHigh : KENTApointLow;
                }  
            }
        }
        else if (index == TRILINGindex)
        {

            temp.Reverse();

            if (temp[0] == temp[1] && temp[1] == temp[2])
                sum = 3 * temp[0] + TRILINGpoint;
            else if (temp[1] == temp[2] && temp[2] == temp[3])
                sum = 3 * temp[1] + TRILINGpoint;
            else if (temp[2] == temp[3] && temp[3] == temp[4])
                sum = 3 * temp[2] + TRILINGpoint;
            else if (temp[3] == temp[4] && temp[4] == temp[5])
                sum = 3 * temp[3] + TRILINGpoint;

        }
        else if (index == FULLHOUSEindex)
        {

            temp.Reverse();

            /*  moguce kombinacije
			 * 
			 * 666551
			 * 665551
			 * 666322
			 * 663222
			 * 633322
			 * 633222
			*/

            if (temp[0] == temp[1] && temp[1] == temp[2] && temp[3] == temp[4])
                sum += (temp[0] * 3 + temp[3] * 2) + FULLHOUSEpoint;
            else if (temp[0] == temp[1] && temp[2] == temp[3] && temp[3] == temp[4])
                sum += (temp[0] * 2 + temp[2] * 3) + FULLHOUSEpoint;
            else if (temp[0] == temp[1] && temp[1] == temp[2] && temp[4] == temp[5])
                sum += (temp[0] * 3 + temp[4] * 2) + FULLHOUSEpoint;
            else if (temp[0] == temp[1] && temp[3] == temp[4] && temp[4] == temp[5])
                sum += (temp[0] * 2 + temp[3] * 3) + FULLHOUSEpoint;
            else if (temp[1] == temp[2] && temp[2] == temp[3] && temp[4] == temp[5])
                sum += (temp[1] * 3 + temp[4] * 2) + FULLHOUSEpoint;
            else if (temp[1] == temp[2] && temp[3] == temp[4] && temp[4] == temp[5])
                sum += (temp[1] * 2 + temp[3] * 3) + FULLHOUSEpoint;

        }
        else if (index == POKERindex)
        {

            temp.Reverse();

            if (temp[0] == temp[1] && temp[1] == temp[2] && temp[2] == temp[3])
                sum = 4 * temp[0] + POKERpoint;
            else if (temp[1] == temp[2] && temp[2] == temp[3] && temp[3] == temp[4])
                sum = 4 * temp[1] + POKERpoint;
            else if (temp[2] == temp[3] && temp[3] == temp[4] && temp[4] == temp[5])
                sum = 4 * temp[2] + POKERpoint;

        }
        else if (index == YAMBindex)
        {
            temp.Reverse();

            if (temp[0] == temp[1] && temp[1] == temp[2] && temp[2] == temp[3] && temp[3] == temp[4])
                sum = 5 * temp[0] + YAMBpoint;
            else if (temp[1] == temp[2] && temp[2] == temp[3] && temp[3] == temp[4] && temp[4] == temp[5])
                sum = 5 * temp[1] + YAMBpoint;
        }

        return sum.ToString();

    }

    private void gameOver()
    {
        _isGameOver = true;

        _dicesParent.GetComponent<RollDices>().enabled = false;
        
        int finalSum = 0;

        for (int i = 0; i < SUMrowIndices.Length - 1; i++)
        {
            int currentSum = 0;
            for (int j = 1; j < NUMBER_OF_COLUMNS - 1; j++)
            {
                currentSum += int.Parse(buttonsArray[SUMrowIndices[i], j].transform.GetChild(0).GetComponent<Text>().text);
            }

            finalSum += currentSum;

            buttonsArray[SUMrowIndices[i], NUMBER_OF_COLUMNS - 1].transform.GetChild(0).GetComponent<Text>().text = currentSum.ToString();
        }


        buttonsArray[SUMrowIndices[3], NUMBER_OF_COLUMNS - 1].transform.GetChild(0).GetComponent<Text>().text = finalSum.ToString();

        string endPanelText = string.Empty;

        scoreText.gameObject.SetActive(false);


        if (SaveLoad.isNew(GAMES.YAMB, null, finalSum, true))
        {
            SaveLoad.set(GAMES.YAMB, null, finalSum);
            endPanelText = "New high score\n " + finalSum + "\nBest score\n " + SaveLoad.get(GAMES.YAMB, null, true);

        }
        else
            endPanelText = "Current score\n " + finalSum + "\nBest score\n " + SaveLoad.get(GAMES.YAMB, null, true);

        scoreText.text = endPanelText;

        // scoreText.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(false);

        FindObjectOfType<EndPanel>().ShowEndPanel(finalSum, SaveLoad.get(GAMES.YAMB, null, true));
    }
    
}
