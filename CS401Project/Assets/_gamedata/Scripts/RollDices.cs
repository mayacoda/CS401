using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class RollDices : MonoBehaviour
{
    private const int MAX_NUMBER_OF_ROLLS = 3;

    [SerializeField] private GameObject _dicesParent;

    [SerializeField] private Text rollText;

    public int currentNumberOfRoll
    {
        get;
        private set;
    }

    private YambController _yambController;
    
    private Dictionary<Button, bool> buttons;

    void Start()
    {
        currentNumberOfRoll = 0;

        buttons = new Dictionary<Button, bool>();

        for (int i = 0; i < 6; i++)
        {
            _dicesParent.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(OnDiceClickedCallback);
            buttons.Add(transform.GetChild(i).GetComponent<Button>(), false);
        }

        _yambController = FindObjectOfType<YambController>();

        ResetDices();
    }

    public void ResetDices()
    {
        currentNumberOfRoll = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            buttons[buttons.Keys.ElementAt(i)] = false;
            transform.GetChild(i).GetComponent<Image>().color = Color.white;
            transform.GetChild(i).transform.GetChild(0).GetComponent<Text>().text = "";
        }
    }

    private void OnDiceClickedCallback()
    {
        if (currentNumberOfRoll == 0)
        {
            return;
        }

        int index = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);
        Button currentButton = buttons.Keys.ElementAt(index);

        buttons[currentButton] = !buttons[currentButton];

        currentButton.GetComponent<Image>().color = buttons[currentButton] ? Color.gray : Color.white;

    }

    public void onRoll()
    {
        // if only najava left, and we want to roll without selection
        if (currentNumberOfRoll == 1 && _yambController._onlyNajavaRemained == true && _yambController._isNajava == false)
        {
            rollText.text = "Because N only left, you need to select N field first than process with roll!";
            return;
        }
        
        if (currentNumberOfRoll == MAX_NUMBER_OF_ROLLS)
        {
            return;
        }

        // foreach button that is not locked, get new random number
        for (int i = 0; i < 6; i++)
        {
            if (buttons.Values.ElementAt(i) == false)
            {
                transform.GetChild(i).GetChild(0).GetComponent<Text>().text = Random.Range(1, 7).ToString();
            }
        }

        currentNumberOfRoll++;
        
        if (currentNumberOfRoll == 3)
        {
            rollText.text = "Select field";
        }
        else
        {
            rollText.text = "Roll number: " + currentNumberOfRoll;
        }
    }
}