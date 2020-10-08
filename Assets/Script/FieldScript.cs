using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldScript : MonoBehaviour {
	public GM gm;
	public List<GameObject> Units = new List<GameObject>();
	public List<GameObject> bgImage = new List<GameObject>();
	public List<GameObject> Obj = new List<GameObject>();
	public List<string> Enemylist = new List<string>();
	public GameObject EnemyObj;
	public UnityEngine.UI.Image Faider;
	public int pnum;
	float screenend = 800;
	// Use this for initialization
	void Start () {
		teamChange(PlayerPrefs.GetInt("pnum"));
		setTeam ();
		ClearLevel ();
		//StartCoroutine (runtime ());
		screenend = 800;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void teamChange(int tnum){
		pnum = tnum;
		Debug.Log (pnum);
	}

	void setTeam(){
		for (int i = 0; i < 6; i ++) {
			if(gm.partylist[pnum,i] == -1){
				Units[i].GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("team/무인");
				Units[i].SetActive(false);
				Units[i].tag = "Untagged";
			}else{
				Units[i].GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("team/"+gm.savedata.teaminven[gm.partylist[pnum,i]].name);
				Units[i].GetComponent<UnityEngine.UI.Image>().SetNativeSize();
				Units[i].GetComponent<BoxCollider2D>().size = Units[i].GetComponent<RectTransform>().sizeDelta;
				Units[i].GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("anim/"+gm.savedata.teaminven[gm.partylist[pnum,i]].name+"애니");
				Units[i].SetActive(true);
				Units[i].tag = gm.chktype(System.Convert.ToInt32(gm.savedata.teaminven[gm.partylist[pnum,i]].type));
				Units[i].GetComponent<UnitScript>().type = gm.savedata.teaminven[gm.partylist[pnum,i]].type;
				Units[i].GetComponent<UnitScript>().hp = gm.savedata.teaminven[gm.partylist[pnum,i]].hp;
				Units[i].GetComponent<UnitScript>().inven = gm.savedata.teaminven[gm.partylist[pnum,i]].inven;
				StartCoroutine(Units[i].GetComponent<UnitScript>().Move());
			}
		}
	}
	/*
	IEnumerator runtime(){
		while (true) {
			for (int i = 0; i < 6; i ++) {
				Units [i].GetComponent<RectTransform>().Translate (0.2f, 0, 0);
				if(Units[i].GetComponent<RectTransform>().anchoredPosition.x > screenend){
					StartCoroutine(FaidOut());
					yield return new WaitForSeconds(2);
				}
			}
			yield return new WaitForSeconds (0.01f);
		}
	}
	*/
	void ClearLevel(){
		//팀
		for (int i = 0; i < 6; i ++) {
			Units[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(-150 + i*40,Units[i].GetComponent<RectTransform>().anchoredPosition.y);
		}
		//배경
		int total = bgImage.Count;
		for (int i = 0; i < total; i ++) {
			bgImage[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(i*screenend/total,i*screenend/total + 100), 110);
		}
		//광석
		int ro = Random.Range (0, 10);
		total = Obj.Count;
		int objnum;
		if (ro < 6) {
			objnum = 0;
		}else if(ro < 9){
			objnum = 1;
		}else{
			objnum = 2;
		}
		for(int i = 0; i < total; i++){
			if(i >= objnum){
				Obj[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0,-500);
				Obj[i].SetActive(false);
			}else{
				Obj[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(100,screenend-50),110);
				int r = Random.Range(-1,2);
				if(r == 0)
					r= 1;
				
				Obj[i].GetComponent<RectTransform>().localScale = new Vector3(r,1,1);
				Obj[i].GetComponent<BoxCollider2D>().offset = new Vector2(0,Obj[i].GetComponent<RectTransform>().sizeDelta.y/2);
				Obj[i].GetComponent<BoxCollider2D>().size = Obj[i].GetComponent<RectTransform>().sizeDelta;
				Obj[i].SetActive(true);
			}
		}
		ro = Random.Range (0, 10);
		total = Obj.Count;
		if (ro < 6) {
			objnum = 0;
		}else if(ro <= 9){
			objnum = 1;
		}else{
			objnum = 1;
		}
		total = Enemylist.Count;
		ro = Random.Range (0, total);
		if (objnum > 0) {
			EnemyObj.SetActive (true);
			EnemyObj.GetComponent<BoxCollider2D>().offset = new Vector2(0,EnemyObj.GetComponent<RectTransform>().sizeDelta.y/2);
			EnemyObj.GetComponent<BoxCollider2D>().size = EnemyObj.GetComponent<RectTransform>().sizeDelta;
		} else {
			EnemyObj.SetActive(false);
		}
	}

	void Arrive(){
		StartCoroutine (FaidOut ());
	}

	IEnumerator FaidOut(){
		for(int i = 0; i <= 50; i ++){
			Faider.color = new Color(0,0,0,i*0.02f);
			yield return new WaitForSeconds(0.02f);
		}
		ClearLevel();
		yield return new WaitForSeconds (1);
		for(int i = 50; i >= 0; i --){
			Faider.color = new Color(0,0,0,i*0.02f);
			yield return new WaitForSeconds(0.02f);
		}
	}
}
