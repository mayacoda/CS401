using UnityEngine;
using UnityEngine.UI;

public class EndPanel : MonoBehaviour
{
    public CustomSceneManager sceneManager;

    public Text endText;
    public Button backButton;
    public Button restartButton;
    
    private void Awake()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void ShowEndPanel(string text)
    {
        endText.text = text;
        backButton.onClick.AddListener(() => sceneManager.ChangeScene(0));

        restartButton.onClick.AddListener(() => sceneManager.ChangeScene(sceneManager.GetCurrentScene()));

        gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }
}
