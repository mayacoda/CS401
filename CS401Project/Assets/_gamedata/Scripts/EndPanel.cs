using UnityEngine;
using UnityEngine.UI;

public class EndPanel : MonoBehaviour
{
    public CustomSceneManager sceneManager;

    public GameObject newHighScorePanel;
    public GameObject normalScorePanel;
    public Text normalScoreText;
    public Text normalHighScoreText;
    public Text newHighScoreText;
    public Button backButton;
    public Button restartButton;
    
    private void Awake()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void ShowEndPanel(int score, int highScore)
    {
        if (highScore > score)
        {
            normalScorePanel.SetActive(true);
            normalScoreText.text = score.ToString();
            normalHighScoreText.text = highScore.ToString();
        }
        else
        {
            newHighScorePanel.SetActive(true);
            newHighScoreText.text = highScore.ToString();
        }
        
        backButton.onClick.AddListener(() => sceneManager.ChangeScene(0));

        restartButton.onClick.AddListener(() => sceneManager.ChangeScene(sceneManager.GetCurrentScene()));

        gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }
}
