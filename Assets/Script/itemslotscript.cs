using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class itemslotscript : MonoBehaviour,IPointerDownHandler,IPointerClickHandler//,IPointerUpHandler 
{
	public int number;
	bool Chk;
	public string name;
	public GM gm;
	public UnityEngine.UI.ScrollRect dragarea;
	public GameObject invenarea;
	Vector2 prevmouseposition;
	public RectTransform pin;
	public RaycastHit hit;

	IEnumerator MoveChk(){
		while (true) {
			if (Input.mousePosition.x > prevmouseposition.x + 20 || Input.mousePosition.x < prevmouseposition.x - 20) {
				dragarea.enabled = true;
				break;
			}else if (Input.mousePosition.y < prevmouseposition.y - 5){
				dragarea.enabled = false;
				//dragarea.content = GetComponent<RectTransform>();
				//dragarea.gameObject.SetActive (true);
				Chk = true;
				StartCoroutine (Move ());
				break;
			}
			yield return new WaitForSeconds(0.01f);
		}
	}

	int positionChk()
	{
		int nm = System.Convert.ToInt32 ((GetComponent<RectTransform> ().anchoredPosition.x - 32.5) / 70);
		if (nm < 0) {
			nm = 0;
		} else if (nm >= gm.savedata.inven.Count - 1) {
			nm = gm.savedata.inven.Count - 1;
		}
		return nm;
	}

	IEnumerator Move(){
		while(true){
			GetComponent<RectTransform>().position = new Vector2 (GetComponent<RectTransform>().position.x ,Input.mousePosition.y - 20);
			if(!Input.GetMouseButton(0)){
				dragarea.enabled = true;
				//number = gm.haveChk(name);
				number = positionChk();
				gm.itemslotlist[number].GetComponent<slotmovescript>().spot = 37.5f + number * 70.0f;
				gm.itemslotlist[number].GetComponent<slotmovescript>().enabled = true;
				gm.itemslotlist[number].GetComponent<slotmovescript>().a = 0;
				/* 
				for(int i = number + 1; i < gm.inven.Count; i ++){
					gm.itemslotlist[i].GetComponent<slotmovescript>().enabled = true;
					gm.itemslotlist[i].GetComponent<slotmovescript>().a = 0;
				}
				*/
				yield break;
			}else{
				if(GetComponent<RectTransform>().anchoredPosition.y < -60){
					number = positionChk();
					GetComponent<slotmovescript>().enabled = false;
					gm.currentslot = gm.savedata.inven[number];
					gm.savedata.inven.RemoveAt(number);
					gm.itemslotlist.RemoveAt(number);
					gm.putnum = number;
					GetComponent<Rigidbody2D>().isKinematic = false;
					//GetComponent<DistanceJoint2D>().enabled = true;
					pin.GetComponent<HingeJoint2D>().connectedBody = GetComponent<Rigidbody2D>();
					//transform.SetParent (transform.parent.parent.parent);
					for(int i = number; i < gm.savedata.inven.Count; i ++){
						gm.itemslotlist[i].GetComponent<slotmovescript>().spot = 37.5f +i * 70.0f;
						gm.itemslotlist[i].GetComponent<slotmovescript>().a = 0;
						gm.itemslotlist[i].GetComponent<slotmovescript>().enabled = true;
					}
					StartCoroutine (MoveFree ());
					break;
				}
			yield return new WaitForSeconds(0.01f);
			}
		}
	}

	IEnumerator Enterslot(){
		int enternum = System.Convert.ToInt32((GetComponent<RectTransform>().anchoredPosition.x-32.5)/70);
		//float slotleft = GetComponent<RectTransform> ().position.x - (enternum * 35 + 20);
		//transform.parent.GetComponent<RectTransform> ().Translate (-slotleft, 0, 0);
		if (enternum < 0)
			enternum = 0;
		if (enternum >= gm.savedata.inven.Count) {
			gm.savedata.inven.Add (gm.currentslot);
			gm.itemslotlist.Add (gameObject);
		} else {
			gm.savedata.inven.Insert (enternum, gm.currentslot);
			gm.itemslotlist.Insert (enternum, gameObject);
			for (int i = enternum + 1; i < gm.savedata.inven.Count; i ++) {
				gm.itemslotlist[i].GetComponent<slotmovescript> ().spot = 37.5f + i * 70.0f;
				gm.itemslotlist[i].GetComponent<slotmovescript> ().a = 0;
				gm.itemslotlist[i].GetComponent<slotmovescript> ().enabled = true;
			}
		}
		StartCoroutine (Move ());
		yield break;
	}

	IEnumerator MoveFree(){
		while (true) {
			//GetComponent<RectTransform>().position = new Vector2 (Input.mousePosition.x ,Input.mousePosition.y);
			pin.position = new Vector2 (Input.mousePosition.x ,Input.mousePosition.y);
			if(!Input.GetMouseButton(0)){
				Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, Mathf.Infinity, 1 << 8);
				if(Userinfo.Instance.area == 1){
					if(hit.collider != null){
						int totalstand = hit.collider.transform.childCount;
						if (Resources.Load ("item/" + gm.itdb[gm.currentslot.code]["name"].Value) != null) {
							hit.collider.GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite> ("item/" + gm.itdb[gm.currentslot.code]["name"].Value);
						} else {
							hit.collider.GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite> ("item/없음");
						}
						if(hit.collider.GetComponent<UnityEngine.UI.Image>().enabled == false){
						hit.collider.GetComponent<UnityEngine.UI.Image>().enabled = true;
						StartCoroutine(gm.puttoexhibit(gm.currentslot.code, hit.collider.transform.parent.name +hit.collider.name, false));
						}else{
							StartCoroutine(gm.puttoexhibit(gm.currentslot.code, hit.collider.transform.parent.name +hit.collider.name, true));
						}
						StartCoroutine(gm.deletedb(gm.currentslot.code,1));
						Destroy(gameObject);
						gm.invenRect.GetComponent<RectTransform> ().sizeDelta = new Vector3 (gm.savedata.inven.Count * 70.0f+2.5f, 70, 0);
						/*
						for (int i = 0; i < totalstand; i ++){
							if(hit.collider.transform.GetChild(i).gameObject.activeSelf == false){
								hit.collider.transform.GetChild(i).GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite> ("item/" + gm.itdb[gm.currentslot.code]["name"].Value);
								hit.collider.transform.GetChild(i).gameObject.SetActive(true);
								StartCoroutine(gm.puttoexhibit(gm.currentslot.code, hit.collider.name +i.ToString()));
								StartCoroutine(gm.deletedb(gm.currentslot.code,1));
								Destroy(gameObject);
								gm.invenRect.GetComponent<RectTransform> ().sizeDelta = new Vector3 (gm.savedata.inven.Count * 70.0f+2.5f, 70, 0);
								break;
							}
						}
						*/
					}else{
						GetComponent<Rigidbody2D>().isKinematic = true;
						transform.rotation = new Quaternion ();
						pin.GetComponent<HingeJoint2D>().connectedBody = null;
						gm.savedata.inven.Add (gm.currentslot);
						gm.itemslotlist.Add (gameObject);
						GetComponent<slotmovescript> ().spot = - 32.5f + gm.savedata.inven.Count * 70.0f;
						GetComponent<slotmovescript> ().a = 0;
						GetComponent<slotmovescript> ().enabled = true;
					}
				}else if(Userinfo.Instance.area == 0){
					if(gm.currentslot.type == "망치")
					{
						if(gm.savedata.equip.name != ""){
							gm.putiteminfo(gm.savedata.equip);
						}
						gm.hammerslot.GetComponent<UnityEngine.UI.Image>().enabled = true;
						gm.hammerslot.GetComponent<UnityEngine.UI.Image>().sprite = transform.GetComponent<UnityEngine.UI.Image>().sprite;
						gm.savedata.equip = gm.currentslot;
						//gm.SendMessage("Save");
						
					}else{
						Chk = false;
						dragarea.enabled = true;
						if(gm.currentslot.number > 1){
							gm.DropCountPannel.transform.parent.gameObject.SetActive(true);
							gm.DropCountPannel.GetComponent<UnityEngine.UI.InputField>().text = "";
						}else{
							gm.drop(1);
						}
					}
					Destroy(gameObject);
					//transform.SetParent (invenarea.transform);
					//dragarea.content = transform.parent.GetComponent<RectTransform>();
					//dragarea.gameObject.SetActive (false);
				}
				yield break;
			}else{
				if(GetComponent<RectTransform>().anchoredPosition.y >= -60){
					//transform.SetParent (invenarea.transform);
					GetComponent<Rigidbody2D>().isKinematic = true;
					transform.rotation = new Quaternion ();
					pin.GetComponent<HingeJoint2D>().connectedBody = null;
					//GetComponent<DistanceJoint2D>().enabled = false;
					StartCoroutine(Enterslot());
					yield break;
				}
			}
			yield return new WaitForSeconds(0.01f);
		}
	}

	public void OnPointerDown(PointerEventData data){
		prevmouseposition = data.position;
		number = positionChk();
		for(int i = number; i < gm.savedata.inven.Count; i ++){
			gm.itemslotlist[i].GetComponent<slotmovescript>().enabled = false;
		}
		transform.SetAsLastSibling ();
		StartCoroutine (MoveChk ());
		//StartCoroutine (Drag ());
	}
	public void OnPointerClick(PointerEventData data){
		number = positionChk();
		gm.currentslot = gm.savedata.inven[number];
		gm.infobox.GetComponent<infoboxscript>().info ();
		gm.infobox.SetActive (true);
	}
	public void Drag(Vector2 data){
		dragarea.content = GetComponent<RectTransform> ();
		if (data.x > 1.0f) {
			//transform.SetParent(transform.parent.parent.parent);
			GetComponent<RectTransform> ().position = new Vector3 (data.x, data.y, 1);
		}
	}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               
}
