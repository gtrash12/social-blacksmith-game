using UnityEngine;
using System.Collections;

public class slotmovescript : MonoBehaviour {
	public float spot;
	public float a = 0;
	public int direction = -1;
	public float distancex = 0;
	public float distancey = 0;

	void Start () {
	
	}

	void OnEnable(){
		//GetComponent<itemslotscript> ().enabled = false;
		if(GetComponent<RectTransform> ().anchoredPosition.x - spot != 0){
			direction = -System.Convert.ToInt32((GetComponent<RectTransform> ().anchoredPosition.x - spot)/Mathf.Abs(GetComponent<RectTransform> ().anchoredPosition.x - spot));
			distancex = spot - GetComponent<RectTransform>().anchoredPosition.x;
			distancey = GetComponent<RectTransform>().anchoredPosition.y;
		}
	}

	// Update is called once per frame
	void Update () {
			//GetComponent<RectTransform> ().Translate((spot - GetComponent<RectTransform> ().anchoredPosition.x ) * Time.deltaTime,0,0);

		GetComponent<RectTransform> ().Translate(direction*a * Time.deltaTime,0,0);
		GetComponent<RectTransform> ().anchoredPosition = new Vector2(GetComponent<RectTransform>().anchoredPosition.x,(spot-GetComponent<RectTransform>().anchoredPosition.x)/distancex*distancey);
			a += 5000*Time.deltaTime;
		if ((GetComponent<RectTransform> ().anchoredPosition.x - spot < 1 && direction == -1) || (spot - GetComponent<RectTransform> ().anchoredPosition.x < 1 && direction == 1)) {
			GetComponent<RectTransform> ().anchoredPosition = new Vector2(spot,0);
			a = 0;
			GetComponent<itemslotscript> ().enabled = true;
			this.enabled = false;
		}
	}
}
