using UnityEngine;
using UnityEngine.SceneManagement;
using static SoundManager;

public class CustomSceneManager : MonoBehaviour
{
    public static CustomSceneManager instance;
    
    private void Awake()
    {
        instance = this;
    }
    
    public void ChangeScene(int index)
    {
        if(SoundManager.instance != null)
            SoundManager.instance.PlaySingle(AUDIO.BUTTON_PRESS);

        SceneManager.LoadScene(index);
    }

    public int GetCurrentScene()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }
}
