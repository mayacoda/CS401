using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Numpad : MonoBehaviour
{
    // private const int BACKindex = 9;
    // private const int EMPTYindex = 10;
    private const int CLEARindex = 9;
    
    private SudokuController _sudokuController;
    
    void Start()
    {
        _sudokuController = FindObjectOfType<SudokuController>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Button btn = transform.GetChild(i).GetComponent<Button>();
            btn.onClick.AddListener(() => ButtonPressed());
        }

        // gameObject.SetActive(false);

    }

    private void ButtonPressed()
    {
        GameObject go = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

        /*if (go.transform.parent.transform.GetChild(BACKindex).name == go.name)
        {

        }
        else if (go.transform.parent.transform.GetChild(EMPTYindex).name == go.name)
        {
            return;
        }
        else */

        if(_sudokuController.CurrentButtonPressedText == null)
        {
            return;
        }

        if (go.transform.parent.transform.GetChild(CLEARindex).name == go.name)
        {
            _sudokuController.CurrentButtonPressedText.text = string.Empty;
        }
        else
        {
            _sudokuController.CurrentButtonPressedText.text = go.transform.GetChild(0).GetComponent<Text>().text;
        }
        
        // gameObject.SetActive(false);

        // _sudokuController.enabled = true;

        _sudokuController.CheckForEnd();
    }


}
