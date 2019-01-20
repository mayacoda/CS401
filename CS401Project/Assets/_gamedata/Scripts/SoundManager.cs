using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource efxSource;                   //Drag a reference to the audio source which will play the sound effects.
    public AudioSource musicSource;                 //Drag a reference to the audio source which will play the music.

    public static SoundManager instance = null;     //Allows other scripts to call functions from SoundManager.        

    public AudioClip buttonPress;

    public enum AUDIO
    {
        BUTTON_PRESS,
    }

    void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);


    }
    
    public void PlaySingle(AudioClip clip)
    {
        efxSource.clip = clip;
        
        efxSource.Play();
    }

    public void PlaySingle(AUDIO audio)
    {
        AudioClip clip = null;

        if(audio == AUDIO.BUTTON_PRESS)
        {
            clip = buttonPress;
        }

        efxSource.PlayOneShot(clip);
        
    }


}