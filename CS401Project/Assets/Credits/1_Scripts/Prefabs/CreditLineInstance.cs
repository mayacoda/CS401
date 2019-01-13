using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class CreditLineInstance : MonoBehaviour
{
	// Parameters
	private float speed;
	private float fadeTime;

	// Position & size
	private Vector2 pos;
	private RectTransform rect = null;

	// Callbacks
	private bool nextLine = false;

	/// <summary>
	/// Sets the texts or the image of the credits line.
	/// </summary>
	/// <param name="data">Data.</param>
	/// <param name="data2">Data2.</param>
	public abstract void setVisual(string data, string data2);

	/// <summary>
	/// Fades in the graphic.
	/// </summary>
	/// <param name="time">Duration of the fade.</param>
	public abstract void fadeIn(float time);

	/// <summary>
	/// Fades out the graphic.
	/// </summary>
	/// <param name="time">Duration of the fade.</param>
	public abstract void fadeOut(float time);

	/// <summary>
	/// Entry point after creating the prefab. Set parameters and launches the line.
	/// </summary>
	/// <param name="data">Data.</param>
	/// <param name="margin">The margin in pixels.</param>
	/// <param name="speed">The speed of the text/sprite.</param>
	/// <param name="fadeTime">Fade time.</param>
	public void go(string data, string data2, int height, float speed, float fadeTime)
	{
		// Set parameters
		this.speed = speed;
		this.fadeTime = Mathf.Clamp(fadeTime, 0f, 10f);

		// Set visual & hide
		setVisual(data, data2);
		fadeOut(0f);

		// Get position
		rect = GetComponent<RectTransform>();
		pos = rect.anchoredPosition;

		// When do we fade in?
		float whenIn = rect.rect.height/speed;
		Invoke("fadeIn", whenIn);

		// Calculate when we fade out
		// time = distance / speed.
		// distance is screen height - distance(=speed*time) of fade
		float whenOut = (height-(this.fadeTime*speed))/speed;
		Invoke("fadeOut", whenOut);
		Invoke("destroy", whenOut+this.fadeTime+.1f);
	}

	private void Update()
	{
		if(rect != null)
		{
			// Move upwards!
			pos.y += Time.deltaTime*speed;
			rect.anchoredPosition = pos;

			// Call the next line if the object has moved its heigth
			if(!nextLine && pos.y > rect.rect.height/**.5f*/){
				nextLine = true;
				Credits.getInstance().spawnNext();
			}
		}
	}

	private void fadeIn(){
		fadeIn(fadeTime);
	}

	private void fadeOut(){
		fadeOut(fadeTime);
	}

	private void destroy(){
		Credits.getInstance().callbackDeleted();
		DestroyImmediate(gameObject);
	}
}
