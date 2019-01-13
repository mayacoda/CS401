using UnityEngine;

public class FlappyBirdBackgroundLooper : MonoBehaviour
{
    private const int numBGPanels = 6;

    [SerializeField] private float pipeMax = 1f;
    [SerializeField] private float pipeMin = 0f;
    
    void Start()
    {
        GameObject[] pipes = GameObject.FindGameObjectsWithTag("Pipe");

        foreach (GameObject pipe in pipes)
        {
            Vector3 pos = pipe.transform.position;
            pos.y = Random.Range(pipeMin, pipeMax);
            pipe.transform.position = pos;
        }
    }

    // TRIGER za objekte sa background tagom i pipe-om
    void OnTriggerEnter2D(Collider2D collider)
    {

        float widthOfBGObject = ((BoxCollider2D)collider).size.x;

        Vector3 pos = collider.transform.position;

        pos.x += widthOfBGObject * numBGPanels * collider.gameObject.transform.lossyScale.x;

        if (collider.tag == "Pipe")
        {
            pos.y = Random.Range(pipeMin, pipeMax);
        }

        collider.transform.position = pos;

    }
}
