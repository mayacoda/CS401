using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CreditLineInstanceImage : CreditLineInstance
{
	public override void setVisual(string data, string data2)
	{
		RectTransform rect = GetComponent<RectTransform>();
		Sprite s = Resources.Load<Sprite>(data);
		if(s != null)
		{
			Vector2 r = rect.sizeDelta;
			r.y = s.rect.height;
			rect.sizeDelta = r;
			GetComponent<Image>().sprite = s;
		}
	}
	
	public override void fadeIn(float time)
	{
		GetComponent<Image>().CrossFadeAlpha(1f, time, false);
	}
	
	public override void fadeOut(float time)
	{
		GetComponent<Image>().CrossFadeAlpha(0f, time, false);
	}
}
