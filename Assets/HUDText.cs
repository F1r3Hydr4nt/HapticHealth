using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDText : MonoBehaviour {
	Text text;

	void Awake(){
		text = GetComponent<Text> ();
	}

	public void Spawn(Vector3 v){
		transform.position = Camera.main.WorldToScreenPoint(v);
	}

	public void Display(Canvas canvas){
		
		text.enabled = true;
		transform.SetParent(canvas.transform,true);
		LeanTween.value (gameObject, UpdateAlpha, text.color.a,0f ,1f).setDelay(0.5f).setEase(LeanTweenType.easeInCirc);
		LeanTween.moveLocalY (gameObject, Mathf.Abs(transform.localPosition.y) * 20f, 1.5f).setEase(LeanTweenType.easeInSine);
		LeanTween.scale (gameObject, transform.localScale * 2f, 1.5f).setEase(LeanTweenType.easeInSine);
	}

	void UpdateAlpha(float value){
		text.color = new Color (text.color.r, text.color.g, text.color.b,value);
	}
}
