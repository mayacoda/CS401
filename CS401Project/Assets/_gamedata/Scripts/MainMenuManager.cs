using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{

    [SerializeField] private Button _sudokuButton;
    [SerializeField] private Button _stackButton;
    [SerializeField] private Button _flappyBirdButton;
    [SerializeField] private Button _yambButton;
    [SerializeField] private Button _zigZagButton;

    [SerializeField] private Credits _credits;
    [SerializeField] private Button _creditsButton;
    [SerializeField] private GameObject _creditPanel;


    private void OnEnable()
    {
        _creditPanel.SetActive(false);

        _sudokuButton.onClick.AddListener(() => CustomSceneManager.instance.ChangeScene((int)GlobalVariables.GAMES.SUDOKU));
        _stackButton.onClick.AddListener(() => CustomSceneManager.instance.ChangeScene((int)GlobalVariables.GAMES.STACK));
        _flappyBirdButton.onClick.AddListener(() => CustomSceneManager.instance.ChangeScene((int)GlobalVariables.GAMES.FLAPPY_BIRD));
        _yambButton.onClick.AddListener(() => CustomSceneManager.instance.ChangeScene((int)GlobalVariables.GAMES.YAMB));
        _zigZagButton.onClick.AddListener(() => CustomSceneManager.instance.ChangeScene((int)GlobalVariables.GAMES.ZIG_ZAG));
        _creditsButton.onClick.AddListener(OnCreditButtonCallback);

        _credits.endListeners += OnCreditEndCallback;

    }

    private void OnDisable()
    {
        _sudokuButton.onClick.RemoveAllListeners();
        _stackButton.onClick.RemoveAllListeners();
        _flappyBirdButton.onClick.RemoveAllListeners();
        _yambButton.onClick.RemoveAllListeners();
        _zigZagButton.onClick.RemoveAllListeners();
        _creditsButton.onClick.RemoveAllListeners();

        _credits.endListeners -= OnCreditEndCallback;
    }

    private void OnCreditEndCallback(Credits c)
    {
        _creditPanel.SetActive(false);
    }

    private void OnCreditButtonCallback()
    {
        _creditPanel.SetActive(true);
        _credits.showCredits();
    }
}
 