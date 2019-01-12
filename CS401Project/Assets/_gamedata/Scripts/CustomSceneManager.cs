using UnityEngine;
using UnityEngine.SceneManagement;


public class CustomSceneManager : MonoBehaviour
{
    public static CustomSceneManager instance;

    private void Awake()
    {
        instance = this;
    }
    
    public void ChangeScene(int index)
    {
        SceneManager.LoadScene(index);
    }

}
