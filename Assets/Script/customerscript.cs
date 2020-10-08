using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;

public class customerscript : MonoBehaviour {
	public List<int> wishlist = new List<int>();
	public GM gm;
	public int retry;
	public int totalwish = UnityEngine.Random.Range (1, 4);
	public Vector3 destination;
	public float spd;
	public string wishspot;
	public string wishcode;
	// Use this for initialization
	void Start () {
		spd = 2;
		wishset ();
		retry = UnityEngine.Random.Range (0, 3);
	}

	void wishset(){
		totalwish = UnityEngine.Random.Range (1, 4);
		wishlist.Clear ();
		for (int i = 0; i < totalwish; i ++) {
			wishlist.Add(System.Convert.ToInt32(UnityEngine.Random.Range(0,gm.itdb.Count)));
		}
		findgoods ();
	}
	
	// Update is called once per frame
	void findgoods(){
		int totalgoods = gm.exhibitlist.Count;
		for (int i = 0; i < totalwish; i ++) {
			for (int f = 0; f < totalgoods; f ++){
				if(gm.exhibitlist[f].code == gm.itdb[wishlist[i]]["code"].Value){
					Debug.Log (gm.itdb[gm.exhibitlist[f].code]["name"].Value);
					wishspot = gm.exhibitlist[f].spot;
					wishcode = gm.exhibitlist[f].code;
					Transform hastarget = gm.standgroup.FindChild(gm.exhibitlist[f].spot.Substring(0,2));
					destination = hastarget.GetComponent<RectTransform>().localPosition;
					destination.z -= UnityEngine.Random.Range(50,100);
					destination.x += hastarget.GetChild(System.Convert.ToInt32(gm.exhibitlist[f].spot.Substring(2))).GetComponent<RectTransform>().anchoredPosition.x;
					destination.x += UnityEngine.Random.Range(-20,20);
					rotachk(hastarget.GetComponent<RectTransform>());
					StartCoroutine(movetodestination());
					return;
				}
			}
		}
		Debug.Log ("없다");
		Transform randomtarget = gm.standgroup.GetChild(System.Convert.ToInt32(UnityEngine.Random.Range(0,gm.standgroup.childCount)));
		destination = randomtarget.GetComponent<RectTransform>().localPosition;
		destination.z -= UnityEngine.Random.Range(50,100);
		destination.x += randomtarget.GetChild(System.Convert.ToInt32(UnityEngine.Random.Range(0, randomtarget.childCount))).GetComponent<RectTransform>().anchoredPosition.x;
		destination.x += UnityEngine.Random.Range(-20,20);
		rotachk (randomtarget.GetComponent<RectTransform>());
		StartCoroutine(movetodestination());
		return;
	}

	void rotachk(RectTransform target){
		//float distance = (float)Math.Sqrt (Math.Pow ((destination - target.localPosition).x, 2) + Math.Pow ((destination - target.localPosition).y, 2));
		float rota = (target.eulerAngles.y*Mathf.PI/180);

		destination = new Vector3 (target.localPosition.x + (float)((destination.x - target.localPosition.x)* Mathf.Cos (rota) + (destination.z -target.localPosition.z) * Mathf.Sin (rota)),
		                           destination.y,
		                           target.localPosition.z +(float)((destination.z - target.localPosition.z) * Mathf.Cos (rota) - (destination.x - target.localPosition.x)* Mathf.Sin (rota)));
	}

	IEnumerator movetodestination(){
		while(true){
			GetComponent<RectTransform>().Translate((destination - transform.localPosition).normalized * spd);
			if(Math.Sqrt(Math.Pow((destination - transform.localPosition).x,2) + Math.Pow((destination - transform.localPosition).y,2)) <= 10){
				break;
			}
			yield return new WaitForSeconds(0.01f);
		}
		yield return new WaitForSeconds(UnityEngine.Random.Range(3,10));
		purchase ();
	}

	void purchase(){
		int totalgoods = gm.exhibitlist.Count;
		for (int i = 0; i <  totalgoods; i ++) {
			if(gm.exhibitlist[i].spot == wishspot){
				//GameObject wishtarget = gm.standgroup.FindChild (gm.exhibitlist[i].spot.Substring (0, 2)).FindChild (gm.exhibitlist[i].spot.Substring (2)).gameObject;
				if (gm.exhibitlist[i].code == wishcode) {
					Debug.Log ("살게욧!");
					gm.sellNotice(wishcode);
					//wishtarget.GetComponent<UnityEngine.UI.Image>().enabled = false;
					StartCoroutine(gm.deleteExhibit(gm.exhibitlist[i].spot,i));
					StartCoroutine(gm.goldChange(gm.itdb[gm.exhibitlist[i].code]["gold"].AsInt));
					StartCoroutine (getOut ());
					return;
					break;
				}
			}
		}
		if (retry > 0) {
			Debug.Log ("다른 건 없나?");
			retry -= 1;
			wishset();
		} else {
			StartCoroutine (getOut ());
		}
	}

	IEnumerator getOut(){
		yield return new WaitForSeconds (2);
		destination = new Vector3(1100 , -149, -722);
		Debug.Log ("수고!");
		while(true){
			GetComponent<RectTransform>().Translate((destination - transform.localPosition).normalized * spd);
			if(Math.Sqrt(Math.Pow((destination - transform.localPosition).x,2) + Math.Pow((destination - transform.localPosition).y,2)) <= 10){
				break;
			}
			yield return new WaitForSeconds(0.01f);
		}
		gm.customerlist.Remove (gameObject);
		Destroy (gameObject);
		
	}
}
