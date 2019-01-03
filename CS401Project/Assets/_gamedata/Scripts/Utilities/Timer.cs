using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class Timer : MonoBehaviour
{
    private Text text;
    private float timer;

    void Start()
    {
        timer = 0;
        text = gameObject.GetComponent<Text>();

    }

    void Update()
    {
        timer += Time.deltaTime;
        text.text = "Time: " + (int)timer;
    }

    public float getTime()
    {
        return timer;
    }
}
