using UnityEngine;
using System.Collections;

public class SlotBounce : MonoBehaviour {
	float ax = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void startBounce(){
		ax = 10;
		StartCoroutine (Bounce ());
	}
	IEnumerator Bounce (){
		do {
			yield return new WaitForSeconds (0.01f);
			GetComponent<RectTransform> ().Translate (0, ax, 0);
			ax -= 2;
		} while( GetComponent<RectTransform> ().anchoredPosition.y > 0);
		GetComponent<RectTransform> ().anchoredPosition = new Vector2 (GetComponent<RectTransform> ().anchoredPosition.x, 0);
	}
}
