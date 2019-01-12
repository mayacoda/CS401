using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{

    [SerializeField] private Button _sudokuButton;
    [SerializeField] private Button _stackButton;
    [SerializeField] private Button _flappyBirdButton;
    [SerializeField] private Button _yambButton;
    [SerializeField] private Button _zigZagButton;


    private void OnEnable()
    {
        _sudokuButton.onClick.AddListener(() => CustomSceneManager.instance.ChangeScene((int)GlobalVariables.GAMES.SUDOKU));
        _stackButton.onClick.AddListener(() => CustomSceneManager.instance.ChangeScene((int)GlobalVariables.GAMES.STACK));
        _flappyBirdButton.onClick.AddListener(() => CustomSceneManager.instance.ChangeScene((int)GlobalVariables.GAMES.FLAPPY_BIRD));
        _yambButton.onClick.AddListener(() => CustomSceneManager.instance.ChangeScene((int)GlobalVariables.GAMES.YAMB));
        _zigZagButton.onClick.AddListener(() => CustomSceneManager.instance.ChangeScene((int)GlobalVariables.GAMES.ZIG_ZAG));
    }

    private void OnDisable()
    {
        _sudokuButton.onClick.RemoveAllListeners();
        _stackButton.onClick.RemoveAllListeners();
        _flappyBirdButton.onClick.RemoveAllListeners();
        _yambButton.onClick.RemoveAllListeners();
        _zigZagButton.onClick.RemoveAllListeners();
    }
}
 