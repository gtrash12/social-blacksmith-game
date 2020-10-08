using UnityEngine;
using System.Collections;

public class UnitScript : MonoBehaviour {
	public FieldScript fgm;
	public int hp;
	public string type;
	public int inven;
	public int state = 0;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//상태 : 0 - 걷는중, 1 - 채광중, 2 - 공격중, 3 - 행동불가
	IEnumerator OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.CompareTag ("mineral")) {
			if (type == "3") {
				GetComponent<Animator> ().SetInteger ("state", 1);
				state = 1;
				yield return new WaitForSeconds (3);
				GetComponent<Animator> ().SetInteger ("state", 0);
				state = 0;
				StartCoroutine (Move ());
				other.gameObject.SetActive (false);
				Debug.Log ("캤다!");
				StartCoroutine (Userinfo.Instance.gm.putdb ("1001", 1));
			}
		} else if (other.gameObject.CompareTag ("enemy")) {
			GetComponent<Animator> ().SetInteger ("state", 1);
			state = 2;
		}
	}

	public void attack(GameObject other){

	}

	public IEnumerator Move(){
		while (state == 0) {
			GetComponent<RectTransform>().Translate (0.4f, 0, 0);
			if(GetComponent<RectTransform>().anchoredPosition.x > 800){
				if(fgm.Faider.color.a == 0){
					fgm.SendMessage("Arrive");
				}
			}
			yield return new WaitForSeconds (0.01f);
		}
	}
}
