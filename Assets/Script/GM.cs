using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.IO;
using SimpleJSON;

public class GM : MonoBehaviour {
	public Animator areaCameraAnim;
	public Animator doorAnim;
	public Transform standgroup;
	public GameObject customer;
	public int custime = 60;
	public UnityEngine.UI.Text goldt;
	public Animator sellnotice;
	public int sellectedCharacter = -1;

	public int [,] partylist = new int[5,6];

	public GameObject steampannel;
	//public List<string> party = new List<string> ();
	//public List<party> partylist = new SortedList<party> ();

	[HideInInspector]
	public List<GameObject> customerlist = new List<GameObject> ();
	public GameObject ObjCanvas;
	[Serializable]
	public class saveinfo
	{
		public List<iteminfo> inven = new List<iteminfo> ();
		public List<teaminfo> teaminven = new List<teaminfo> ();
		public List<string> recipeinven = new List<string> ();
		public List<iteminfo> putit = new List<iteminfo> ();
		public iteminfo equip;
	}

	public saveinfo savedata;
	[Serializable]
	public class iteminfo
	{
		public string code;
		public string name;
		public string type;
		public int hp;
		public int power;
		public int gold;
		public int number;
	}
	public class teaminfo
	{
		public string code;
		public string name;
		public string type;
		public int rank;
		public int hp;
		public int power;
		public int inven;
		public int party = -1;
	}

	public class exhibitinfo
	{
		public string code;
		public string spot;
	}

	public List<exhibitinfo> exhibitlist = new List<exhibitinfo> ();
	int mailcapicity = 20;
	public float atkspd = 0.5f;
	public int putnum;
	public UnityEngine.UI.Text hptxt;
	public List<iteminfo> inven = new List<iteminfo> ();
	public List<GameObject> itemslotlist = new List<GameObject> ();
	public List<GameObject> teamslotlist = new List<GameObject> ();
	public List<mailslotscript> mailslotlist = new List<mailslotscript> ();
	List<iteminfo> putit = new List<iteminfo>();
	public List<GameObject> putitemslotlist = new List<GameObject> ();
	//public List<GameObject> itemslotlist = new List<GameObject> ();
	public GameObject invenRect;
	public GameObject slot;
	public GameObject teaminvenRect;
	public GameObject teamslot;
	public GameObject recipeinvenRect;
	public GameObject recipeslot;
	public GameObject mailinvenRect;
	public GameObject mailslot;
	public JSONNode maildata;
	XmlDocument xmldoc;
	public int hp;
	public int power = 1;
	XmlNodeList Node1;
	XmlNode Node2;
	public iteminfo currentslot;
	public iteminfo equipinfo;
	public GameObject DropCountPannel;
	public Animator anim;
	public Animator tableanim;
	public ParticleSystem particleEff;
	public GameObject hammerslot;
	public GameObject infobox;
	public UnityEngine.UI.Text namet;
	public JSONNode itdb;
	public JSONNode teamdb;
	public JSONNode areadb;
	public float fevermiter = 0;
	public float feverpower = 0.01f;
	public UnityEngine.UI.Image feverguage;
	public float fevertime = 2;
	public GameObject fevertxt;
	public Animator canim;
	public UnityEngine.UI.Text getteaminfo;

	string itAdress = "http://galtragames.com/blacksmith_load.php"; // 유저 아이템 로드
	string tAdress = "http://galtragames.com/blacksmith_loadteam.php"; // 유저 용병 로드
	string rAdress = "http://galtragames.com/blacksmith_loadrecipe.php"; // 유저 레시피 로드
	string mAdress = "http://galtragames.com/blacksmith_loadmail.php"; // 유저 레시피 로드
	string itfAdress = "http://galtragames.com/blacksmith_finditem.php"; // 아이템 정보 찾기
	string putitAdress = "http://galtragames.com/blacksmith_puttoInven.php"; // 아이템 획득
	string putteamAdress = "http://galtragames.com/blacksmith_puttoteam.php"; // 팀 획득
	string putrecipeAdress = "http://galtragames.com/blacksmith_puttoRecipe.php"; // 레시피 획득
	string laAdress = "http://galtragames.com/blacksmith_loadall.php"; // 아이템 정보 로드
	string latAdress = "http://galtragames.com/blacksmith_loadallteam.php"; // 용벙 정보 로드
	string itdAdress = "http://galtragames.com/blacksmith_deleteitem.php"; // 아이템 삭제
	string gcAdress = "http://galtragames.com/blacksmith_teamgacha.php"; // 가챠
	string sendmailAdress = "http://galtragames.com/blacksmith_sendmail.php"; // 메일보내기
	string readmailAdress = "http://galtragames.com/blacksmith_readmail.php"; // 메일읽음
	string eAdress = "http://galtragames.com/blacksmith_loadexhibit.php"; // 전시품 로드
	string puteAdress = "http://galtragames.com/blacksmith_exhibit.php"; // 전시품 업데이트
	string visitAdress = "http://galtragames.com/blacksmith_customervisit.php"; // 손님 방문 시간 갱신
	string deletegoodsAdress = "http://galtragames.com/blacksmith_deletegoods.php"; // 전시품 제거
	string addgAdress = "http://galtragames.com/blacksmith_addgold.php"; // 골드 변동
	
	public void cameradown(int d){
		if(areaCameraAnim.enabled == true)
		areaCameraAnim.enabled = false;
		StartCoroutine (cameramove (d));
	}

	public void cameraup(){
		StopCoroutine("cameramove");
	}

	IEnumerator cameramove(int d){
		while(Input.GetMouseButton(0)){
			areaCameraAnim.transform.Translate (d, 0, 0);
			yield return new WaitForSeconds(0.01f);
		}
	}

	public void sellNotice(string code){
		sellnotice.gameObject.SetActive (true);
		sellnotice.transform.FindChild("Text").GetComponent<UnityEngine.UI.Text>().text = itdb[code]["name"].Value +"\n가 팔렸습니다.";
		sellnotice.transform.FindChild ("itemslot").FindChild ("Image").GetComponent<UnityEngine.UI.Image> ().sprite = Resources.Load<Sprite> ("item/" + itdb [code] ["name"].Value);
		sellnotice.Rebind ();
	}

	public IEnumerator fever(){
		canim.Play ("feverzoom");
		Time.timeScale = 0.2f;
		yield return new WaitForSeconds (0.2f);
		Time.timeScale = 1;
		canim.Play ("shake1");
		anim.speed = 3;
		fevertxt.SetActive(true);
		yield return new WaitForSeconds (fevertime);
		anim.speed = 1;
		fevermiter = 0;
		feverguage.fillAmount = 0;
		fevertxt.SetActive(false);
	}


	void Awake(){
		/*
		TextAsset textAsset = (TextAsset)Resources.Load ("item"); 
		xmldoc = new XmlDocument ();  // 객체선언
		xmldoc.LoadXml (textAsset.text);  // 텍스트 불러옴
*/
		Debug.Log (Userinfo.Instance.id);
		if (Userinfo.Instance.id == null) {
			Userinfo.Instance.id = "qrqrqr";
			Userinfo.Instance.name = "갈트래갓";
			Userinfo.Instance.visittime = 10;
		}

		namet.text = Userinfo.Instance.name;
	}

	void Save(){
		var binaryFomatter = new BinaryFormatter ();
		var fileStream = File.Create (Application.persistentDataPath + "/saveData.dat");
		binaryFomatter.Serialize (fileStream, savedata);
		fileStream.Close ();
		PlayerPrefs.SetInt ("hp", hp);
	}
	void Start(){
		StartCoroutine (loadall ());
		StartCoroutine(loadDB ());
		StartCoroutine(loadmail ());
		makemailslot ();
		if (Userinfo.Instance.visittime >= custime) {
			StartCoroutine (customervisit (3));
		} else {
			StartCoroutine (customervisit (custime - Userinfo.Instance.visittime));
		}
		goldt.text = Userinfo.Instance.gold.ToString ();

		//PlayerPrefs.DeleteKey ("party0");
		if(!PlayerPrefs.HasKey("party0")){
			PlayerPrefs.SetString("party0","-1-1-1-1-1-1");
			PlayerPrefs.SetString("party1","-1-1-1-1-1-1");
			PlayerPrefs.SetString("party2","-1-1-1-1-1-1");
			PlayerPrefs.SetString("party3","-1-1-1-1-1-1");
			PlayerPrefs.SetString("party4","-1-1-1-1-1-1");
			PlayerPrefs.SetInt("pnum",0);
			string st = PlayerPrefs.GetString("party1");
		}
		for(int c = 0; c < 5; c ++){
			SetPartylist(c);
		}
		Userinfo.Instance.gm = this;
	}

	public void SetPartylist(int pnum){
		if (pnum == -1)
			pnum = PlayerPrefs.GetInt ("pnum");

		string svt = PlayerPrefs.GetString("party"+pnum);
		Debug.Log (svt);
		for(int i = 0; i < 11; i += 2){
			partylist[pnum,Mathf.FloorToInt(i/2)] = System.Convert.ToInt32(svt.Substring(i,2));
		}
	}

	public void SavePartylist(int pnum){
		Transform itv = teaminvenRect.transform.parent.parent;
		if (pnum == -1)
			pnum = PlayerPrefs.GetInt ("pnum");
		string sett = "";
		for (int i = 0; i < 6; i ++) {
			if(partylist [pnum, i] != -1 && partylist [pnum, i] < 10){
				sett += "0"+partylist [pnum, i].ToString();
			}else{
				sett += partylist [pnum, i].ToString();
			}
		}
		PlayerPrefs.SetString ("party" + pnum, sett);
		//sellectedCharacter = -1;
		/*
		itv.FindChild ("itname").GetComponent<UnityEngine.UI.Text> ().text = "";
		itv.FindChild ("infot").GetComponent<UnityEngine.UI.Text> ().text = "";
		itv.FindChild ("CharacterImage").GetComponent<UnityEngine.UI.Image> ().sprite = Resources.Load<Sprite> ("team/무인");
		itv.FindChild ("CharacterImage").GetComponent<UnityEngine.UI.Image> ().SetNativeSize ();
		undopartylist.Clear ();
		StartCoroutine (Partydb (pnum, prevp));
		*/
	}

	public void SelectParty(int body){
		teaminvenRect.transform.parent.parent.FindChild ("팀" + PlayerPrefs.GetInt ("pnum")).GetComponent<Animator>().SetBool("sellected",false);
		PlayerPrefs.SetInt ("pnum",body);
		teaminvenRect.transform.parent.parent.FindChild ("팀" + PlayerPrefs.GetInt ("pnum")).GetComponent<Animator>().SetBool("sellected",true);
		//teaminvenRect.transform
		sellectedCharacter = -1;
		SetParty (body);
	}

	public void firstSetParty(){
		Transform itv = teaminvenRect.transform.parent.parent;
		itv.FindChild ("teamnum").GetComponent<UnityEngine.UI.Text> ().text = savedata.teaminven.Count + "/30"; 
		sellectedCharacter = -1;
		itv.FindChild ("itname").GetComponent<UnityEngine.UI.Text> ().text = "";
		itv.FindChild ("infot").GetComponent<UnityEngine.UI.Text> ().text = "";
		itv.FindChild("CharacterImage").GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("team/무인");
		itv.FindChild("CharacterImage").GetComponent<UnityEngine.UI.Image>().SetNativeSize();
		itv.FindChild ("팀" + PlayerPrefs.GetInt ("pnum")).GetComponent<Animator>().SetBool("sellected",true);
		SetParty(PlayerPrefs.GetInt("pnum"));
		for (int i = 0; i < 5; i++) {
			for(int c = 0; c <6; c ++){
				if(partylist[i,c] != -1){
					teamslotlist[partylist[i,c]].GetComponent<UnityEngine.UI.Image>().color = new Color(0.53f,0.53f,1);
					teamslotlist[partylist[i,c]].transform.FindChild("party").GetComponent<UnityEngine.UI.Text>().text = i+1 + "팀 참여중";
					teamslotlist[partylist[i,c]].transform.FindChild("party").gameObject.SetActive(true);
				}
			}
		}
	}

	public void SetParty(int pnum = 0){
		Transform PartyUI = teaminvenRect.transform.parent.parent;
		for (int i = 0; i < 6; i ++) {
			if(partylist[pnum,i] == -1){
				PartyUI.FindChild("Party"+i).FindChild("mask").FindChild("image").GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("team/무인");
			}else{
				PartyUI.FindChild("Party"+i).FindChild("mask").FindChild("image").GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("team/"+savedata.teaminven[partylist[pnum,i]].name);
				PartyUI.FindChild("Party"+i).FindChild("mask").FindChild("image").GetComponent<UnityEngine.UI.Image>().SetNativeSize();
			}
		}
	}

	public void AddParty(){
		int pnum = PlayerPrefs.GetInt ("pnum");
		Debug.Log (sellectedCharacter);
		for (int i = 0; i < 6; i ++) {
			if(partylist[pnum,i] != -1){
				if(savedata.teaminven[partylist[pnum,i]].name == savedata.teaminven[sellectedCharacter].name){
					return;
				}
			}
		}
		Debug.Log ("dfdf");
		for (int i = 0; i < 6; i ++) {
			if(partylist[pnum,i] == -1){
				partylist[pnum,i] = sellectedCharacter;
				teaminvenRect.transform.parent.parent.FindChild("Party"+i).FindChild("mask").FindChild("image").GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("team/"+savedata.teaminven[partylist[pnum,i]].name);
				teaminvenRect.transform.parent.parent.FindChild("Party"+i).FindChild("mask").FindChild("image").GetComponent<UnityEngine.UI.Image>().SetNativeSize();
				savedata.teaminven[sellectedCharacter].party = pnum;
				teamslotlist[sellectedCharacter].GetComponent<UnityEngine.UI.Image>().color = new Color(0.53f,0.53f,1);
				teamslotlist[partylist[pnum,i]].transform.FindChild("party").GetComponent<UnityEngine.UI.Text>().text = pnum +  1+ "팀 참여중";
				teamslotlist[partylist[pnum,i]].transform.FindChild("party").gameObject.SetActive(true);
				SavePartylist(pnum);
				return;
				break;
			}
		}
	}

	public void RemoveParty(){
		int pnum = PlayerPrefs.GetInt ("pnum");
		int total = savedata.teaminven.Count;
		teamslotlist[partylist[pnum,sellectedCharacter]].GetComponent<UnityEngine.UI.Image>().color = new Color(0.735f,0.735f,0.735f);
		teamslotlist[partylist[pnum,sellectedCharacter]].transform.FindChild("party").gameObject.SetActive(false);
		partylist [pnum, sellectedCharacter] = -1;
		teaminvenRect.transform.parent.parent.FindChild("Party"+sellectedCharacter).FindChild("mask").FindChild("image").GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("team/무인");
		SavePartylist (pnum);
		/*
		savedata.teaminven[sellectedCharacter].party = pnum;
		for (int i = 0; i < total; i++) {
			if(savedata.teaminven[i].party == pnum && savedata.teaminven[i].code == savedata.teaminven[partylist[pnum,sellectedCharacter]].code){
				savedata.teaminven[i].party = -1;
				teamslotlist[i].GetComponent<UnityEngine.UI.Image>().color = new Color(0.735f,0.735f,0.735f);
				break;
			}
		}
		undopartylist.Add(sellectedCharacter);
		*/
	}

	public void GoParty(int c){
		steampannel.transform.FindChild ("팀" + PlayerPrefs.GetInt ("pnum")).GetComponent<Animator>().SetBool("sellected",false);
		PlayerPrefs.SetInt ("pnum", c);
		for (int i = 0; i < 6; i ++) {
			if(partylist[c,i] == -1){
				steampannel.transform.FindChild("CharacterImage"+i).GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("team/무인");
			}else{
				steampannel.transform.FindChild("CharacterImage"+i).GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("team/"+savedata.teaminven[partylist[c,i]].name);
				steampannel.transform.FindChild("CharacterImage"+i).GetComponent<UnityEngine.UI.Image>().SetNativeSize();
			}
		}
		steampannel.transform.FindChild ("팀" + PlayerPrefs.GetInt ("pnum")).GetComponent<Animator>().SetBool("sellected",true);
		steampannel.transform.FindChild ("출발").gameObject.SetActive (true);
	}

	IEnumerator customervisit(float sec){
		yield return new WaitForSeconds (sec);
		if (exhibitlist.Count > 0) {
			var it = Instantiate (customer, new Vector3 (0, 0, 0), Quaternion.identity) as GameObject;
			it.SetActive (true);
			it.transform.SetParent (ObjCanvas.transform);
			it.GetComponent<RectTransform> ().localScale = new Vector2 (1, 1);
			it.GetComponent<RectTransform> ().anchoredPosition3D = new Vector3 (1100, -149, -722);
			customerlist.Add (it);
			/*
		WWWForm cForm = new WWWForm ();
		cForm.AddField ("fuserid", Userinfo.Instance.id);
		WWW wwwUrl = new WWW(visitAdress, cForm);
		yield return wwwUrl;
		*/
			StartCoroutine (customervisit (custime));
		} else {

			StartCoroutine (customervisit (300));
		}
	}

	public IEnumerator loadall(){
		WWW wwwUrl = new WWW(laAdress);
		yield return wwwUrl;
		itdb = JSONNode.Parse (wwwUrl.text);
		wwwUrl = new WWW(latAdress);
		yield return wwwUrl;
		teamdb = JSONNode.Parse (wwwUrl.text);
	}
	//------------------  네트워크 획득
	public void putdbbtt(string c){
		StartCoroutine (putdb (c));
	}
	public IEnumerator putdb(string c, int n = 1){
		WWWForm cForm = new WWWForm ();
		cForm.AddField ("fuserid", Userinfo.Instance.id);
		cForm.AddField ("code", c);
		cForm.AddField ("number", n);
		WWW wwwUrl = new WWW(putitAdress, cForm);
		yield return wwwUrl;
		putfromdb (c, n);
	}

	public void gachabtt(){
		StartCoroutine (gacha ());
	}

	public IEnumerator gacha(){
		WWWForm cForm = new WWWForm();
		WWW wwwUrl = new WWW (gcAdress);
		yield return wwwUrl;
		StartCoroutine (getteam(wwwUrl.text.Trim ()));
	}

	public IEnumerator getteam(string code){
		WWWForm cForm = new WWWForm ();
		cForm.AddField ("fuserid", Userinfo.Instance.id);
		cForm.AddField ("code", code);
		WWW wwwUrl = new WWW(putteamAdress, cForm);
		yield return wwwUrl;
		getteaminfo.text = teamdb [code] ["name"].Value;
		getteaminfo.transform.parent.gameObject.SetActive (true);
		getteaminfo.transform.parent.GetComponent<Animator> ().Rebind ();
		getteaminfo.transform.parent.FindChild ("Character").GetComponent<UnityEngine.UI.Image> ().sprite = Resources.Load<Sprite> ("team/" + teamdb [code] ["name"].Value);
		int rank = teamdb [code] ["rank"].AsInt;
		for (int i = 1; i <= 6; i ++) {
			getteaminfo.transform.parent.FindChild ("star" + (i).ToString()).gameObject.SetActive (false);
		} 
		for (int i = 1; i <= rank; i ++) {
			getteaminfo.transform.parent.FindChild ("star" + (i).ToString()).gameObject.SetActive (true);
		}
		getteaminfo.transform.parent.FindChild ("빛").GetComponent<Animator> ().SetInteger("rank",rank);
		puttoteam (code);
	}
	
	public IEnumerator deletedb(string code, int num){
		WWWForm cForm = new WWWForm ();
		cForm.AddField ("fuserid", Userinfo.Instance.id);
		cForm.AddField ("code", code);
		cForm.AddField ("number", num);
		WWW wwwUrl = new WWW(itdAdress, cForm);
		yield return wwwUrl;
	}

	IEnumerator loadDB(){
		WWWForm cForm = new WWWForm ();
		cForm.AddField ("fuserid", Userinfo.Instance.id, System.Text.Encoding.UTF8);
		WWW wwwUrl = new WWW(itAdress, cForm);
		yield return wwwUrl;
		if (!wwwUrl.text.Contains ("no")) {
			JSONNode json_data = JSONNode.Parse(wwwUrl.text);
			int total = json_data.Count;
			for (int i = 0; i < total; i++){
				putfromdb(json_data[i]["code"].Value , json_data[i]["number"].AsInt);
			}
		}
		wwwUrl = new WWW(tAdress, cForm);
		yield return wwwUrl;
		if (!wwwUrl.text.Contains ("no")) {
			JSONNode json_data = JSONNode.Parse(wwwUrl.text);
			int total = json_data.Count;
			for (int i = 0; i < total; i++){
				puttoteam(json_data[i]["code"].Value, json_data[i]["party"].AsInt);
			}
		}
		wwwUrl = new WWW(rAdress, cForm);
		yield return wwwUrl;
		if (!wwwUrl.text.Contains ("no")) {
			JSONNode json_data = JSONNode.Parse(wwwUrl.text);
			int total = json_data.Count;
			for (int i = 0; i < total; i++){
				puttorecipe(json_data[i]["code"].Value);
			}
		}
		wwwUrl = new WWW(eAdress, cForm);
		yield return wwwUrl;
		if (!wwwUrl.text.Contains ("no")) {
			JSONNode json_data = JSONNode.Parse(wwwUrl.text);
			int total = json_data.Count;
			for (int i = 0; i < total; i++){
				addexhibit(json_data[i]["code"].Value, json_data[i]["spot"].Value);
				//exhibitlist.Add(json_data[i].Value);
			}
			yield return new WaitForChangedResult();
			setexhibit();
		}
	}

	public void addexhibit(string code, string spot){
		exhibitinfo info = new exhibitinfo();
		info.code = code;
		info.spot = spot;
		exhibitlist.Add (info);
	}

	public IEnumerator deleteExhibit(string spot, int i){
		WWWForm cForm = new WWWForm ();
		cForm.AddField ("fuserid", Userinfo.Instance.id, System.Text.Encoding.UTF8);
		cForm.AddField ("fspot", spot);
		WWW wwwUrl = new WWW(deletegoodsAdress, cForm);
		yield return wwwUrl;
		standgroup.FindChild (exhibitlist [i].spot.Substring (0, 2)).FindChild (exhibitlist [i].spot.Substring (2)).GetComponent<UnityEngine.UI.Image>().enabled = false;
		exhibitlist.RemoveAt (i);
	}

	public IEnumerator goldChange(int g){
		WWWForm cForm = new WWWForm ();
		cForm.AddField ("fuserid", Userinfo.Instance.id, System.Text.Encoding.UTF8);
		cForm.AddField ("fgold", g);
		WWW wwwUrl = new WWW(addgAdress, cForm);
		yield return wwwUrl;
		Userinfo.Instance.gold += g;
		goldt.text = Userinfo.Instance.gold.ToString();
	}


	public void setexhibit(){
		int total = exhibitlist.Count;
		for(int i= 0; i < total; i++ ) {
			GameObject obj = standgroup.FindChild(exhibitlist[i].spot.Substring(0,2)).GetChild(System.Convert.ToInt32(exhibitlist[i].spot.Substring(2))).gameObject;
			if (Resources.Load ("item/" + itdb[exhibitlist[i].code]["name"].Value) != null) {
				obj.GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite> ("item/" + itdb[exhibitlist[i].code]["name"].Value);
			} else {
				obj.GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite> ("item/없음");
			}
			//obj.GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("item/"+itdb[exhibitlist[i].code]["name"].Value);
			obj.GetComponent<UnityEngine.UI.Image>().enabled = true;
		}
	}

	public IEnumerator puttoexhibit(string code, string spot, bool c){
		if (c == false) {
			addexhibit (code, spot);
		} else {
			int total = exhibitlist.Count;
			for ( int i = 0; i < total; i++){
				if(exhibitlist[i].spot == spot){
					StartCoroutine(putdb (exhibitlist[i].code, 1));
					exhibitlist[i].code = code;

					break;
				}
			}
		}
		WWWForm cForm = new WWWForm ();
		cForm.AddField ("fuserid", Userinfo.Instance.id, System.Text.Encoding.UTF8);
		cForm.AddField ("fcode", code);
		cForm.AddField ("fspot", spot);
		WWW wwwUrl = new WWW(puteAdress, cForm);
		yield return wwwUrl;
	}

	public void makemailslot(){
		for(int i = 0; i < mailcapicity; i ++){
			var it = Instantiate (mailslot, new Vector3 ( 0, 0, 0), Quaternion.identity) as GameObject;
			it.SetActive (true);
			it.transform.SetParent (mailinvenRect.transform);
			it.GetComponent<RectTransform> ().localScale = new Vector3 (1, 1, 1);
			it.GetComponent<RectTransform> ().anchoredPosition = new Vector3 ( 0, - 70 * i, 0);
			it.SetActive(false);
			mailslotlist.Add(it.GetComponent<mailslotscript>());
		}
	}

	public IEnumerator loadmail(){
		WWWForm cForm = new WWWForm ();
		cForm.AddField ("fuserid", Userinfo.Instance.id);
		WWW wwwUrl = new WWW (mAdress, cForm);
		yield return wwwUrl;
		Debug.Log (wwwUrl.text);
		if (!wwwUrl.text.Contains ("no")) {
			maildata = JSONNode.Parse(wwwUrl.text);
			Debug.Log(maildata.Count);
		}
	}
	public void sendmailbtt(){
		StartCoroutine (sendmail ());
	}
	public IEnumerator sendmail(){
		WWWForm cForm = new WWWForm ();
		cForm.AddField ("fname", Userinfo.Instance.name);
		cForm.AddField ("freceivername", mailinvenRect.transform.parent.parent.FindChild ("sendform").FindChild ("receiver").GetComponent<UnityEngine.UI.InputField> ().text);
		cForm.AddField ("ftitle", mailinvenRect.transform.parent.parent.FindChild ("sendform").FindChild ("title").GetComponent<UnityEngine.UI.InputField> ().text);
		cForm.AddField ("fbody", mailinvenRect.transform.parent.parent.FindChild ("sendform").FindChild ("body").GetComponent<UnityEngine.UI.InputField> ().text);
		WWW wwwUrl = new WWW (sendmailAdress, cForm);
		yield return wwwUrl;
		Debug.Log (wwwUrl.text);
		mailinvenRect.transform.parent.parent.FindChild ("sendform").FindChild ("receiver").GetComponent<UnityEngine.UI.InputField> ().text = "";
		if (!wwwUrl.text.Contains ("no")) {
			mailinvenRect.transform.parent.parent.FindChild ("sendform").FindChild ("title").GetComponent<UnityEngine.UI.InputField> ().text = "";
			mailinvenRect.transform.parent.parent.FindChild ("sendform").FindChild ("body").GetComponent<UnityEngine.UI.InputField> ().text = "";
		} else {
			mailinvenRect.transform.parent.parent.FindChild ("sendform").FindChild ("receiver").GetComponent<UnityEngine.UI.InputField> ().placeholder.GetComponent<UnityEngine.UI.Text>().text = "존재하지 않는 유저입니다.";
		}
	}

	public void setmail(){
		if (maildata != null) {
		int total = maildata.Count;
			for (int i = 0; i < total; i++) {
				mailslotlist [i].set (maildata [i] ["title"].Value, maildata [i] ["sender"].Value, maildata [i] ["senddate"].Value);
				mailslotlist [i].gameObject.SetActive (true);
			}
		}
	}

	public IEnumerator getrecipe (string code){
		WWWForm cForm = new WWWForm ();
		cForm.AddField ("fuserid", Userinfo.Instance.id);
		cForm.AddField ("code", code);
		WWW wwwUrl = new WWW(putrecipeAdress, cForm);
		yield return wwwUrl;
		if (wwwUrl.text.Trim () != "no") {
			puttorecipe (code);
		}
	}

	public void puttorecipe(string code){
		savedata.recipeinven.Add (code);
		recipeslotmake (code, savedata.recipeinven.Count -1);
		recipeinvenRect.GetComponent<RectTransform> ().sizeDelta = new Vector3 (445, 135 + 135 * Mathf.Floor((savedata.recipeinven.Count-1)/4), 0);
	}

	void recipeslotmake (string code, int i){
		var it = Instantiate (recipeslot, new Vector3 ( 0, 0, 0), Quaternion.identity) as GameObject;
		it.SetActive (true);
		it.transform.SetParent (recipeinvenRect.transform);
		it.GetComponent<RectTransform> ().localScale = new Vector3 (1, 1, 1);
		it.GetComponent<RectTransform> ().anchoredPosition = new Vector3 (5+ 110 * (i - 3 * Mathf.Floor(i/3)), -5- 135 * Mathf.Floor(i/3), 0);
		if (Resources.Load ("item/" + itdb[code]["name"].Value) != null) {
			it.transform.FindChild ("Image").GetComponent<UnityEngine.UI.Image> ().sprite = Resources.Load<Sprite> ("item/" + itdb[code]["name"].Value);
		} else {
			it.transform.FindChild ("Image").GetComponent<UnityEngine.UI.Image> ().sprite = Resources.Load<Sprite> ("item/없음");
		}
		it.transform.FindChild ("name").GetComponent<UnityEngine.UI.Text> ().text = itdb [code] ["name"].Value;
	}

	public void puttoteam(string code, int party=-1){
		teaminfo data = new teaminfo ();
		data.code = teamdb[code]["code"].Value;
		data.name = teamdb[code]["name"].Value;
		data.type = teamdb[code]["type"].Value;
		data.rank = teamdb[code]["rank"].AsInt;
		data.hp = teamdb[code]["hp"].AsInt;
		data.party = party;
		if (teamdb[code]["power"] != null)
			data.power = teamdb[code]["power"].AsInt;
		if (teamdb[code]["inven"] != null)
			data.power = teamdb[code]["inven"].AsInt;
		savedata.teaminven.Add (data);
		teamslotmake (data, savedata.teaminven.Count -1);
		teaminvenRect.GetComponent<RectTransform> ().sizeDelta = new Vector3 (335, 160 + 155 * Mathf.FloorToInt((savedata.teaminven.Count-1)/3), 0);
	}
	void teamslotmake(teaminfo data, int i){
		var it = Instantiate (teamslot, new Vector3 ( 0, 0, 0), Quaternion.identity) as GameObject;
		it.transform.SetParent (teaminvenRect.transform);
		it.SetActive (true);
		it.GetComponent<RectTransform> ().localScale = new Vector3 (1, 1, 1);
		it.GetComponent<RectTransform> ().anchoredPosition = new Vector3 ( 5 + 110 * (i - 3 * Mathf.Floor(i/3)),-5 - 155 * Mathf.Floor(i/3), 0);
		if (Resources.Load ("team/" + data.name) != null) {
			it.transform.FindChild ("Image").GetComponent<UnityEngine.UI.Image> ().sprite = Resources.Load<Sprite> ("team/" + data.name);
		} else {
			it.transform.FindChild ("Image").GetComponent<UnityEngine.UI.Image> ().sprite = Resources.Load<Sprite> ("item/없음");
		}
		it.transform.FindChild ("name").GetComponent<UnityEngine.UI.Text>().text = data.name;
		int total = data.rank;
		for(int s = 1; s <= total; s ++){
			it.transform.FindChild ("star"+s.ToString()).gameObject.SetActive(true);
		}
		/*
		if (data.party >= 0) {
			it.GetComponent<UnityEngine.UI.Image>().color = new Color(0.53f,0.53f,1);
			it.transform.FindChild("party").GetComponent<UnityEngine.UI.Text>().text = PlayerPrefs.GetInt("pnum")+1 + "팀 참여중";
			it.transform.FindChild("party").gameObject.SetActive(true);

		}
		*/
		teamslotlist.Insert (i, it);
		//it.GetComponent<SlotBounce> ().startBounce ();
	}
	public void putfromdb(string code, int num = 1){
		int haveit;
		if (itdb[code]["type"].Value == "재료") {
			haveit = haveChk (itdb[code]["name"].Value);
		} else {
			haveit = -1;
		}
		
		if (haveit >= 0) {
			savedata.inven [haveit].number += num;
			itemslotlist[haveit].transform.FindChild ("Number").GetComponent<UnityEngine.UI.Text> ().text = savedata.inven [haveit].number.ToString();
			itemslotlist[haveit].GetComponent<SlotBounce> ().startBounce ();
		} else {
			iteminfo data = new iteminfo ();
			data.code = itdb[code]["code"].Value;
			data.name = itdb[code]["name"].Value;
			data.number = num;
			data.type = itdb[code]["type"].Value;
			data.hp = itdb[code]["hp"].AsInt;
			if (itdb[code]["gold"] != null)
				data.gold = itdb[code]["gold"].AsInt;
			if (itdb[code]["power"] != null)
				data.power = itdb[code]["power"].AsInt;
			savedata.inven.Add (data);
			slotmake (data, savedata.inven.Count - 1);
			invenRect.GetComponent<RectTransform> ().sizeDelta = new Vector3 (savedata.inven.Count * 70.0f+2.5f, 70, 0);
		}
	}
	///////////////

	/*
	void Start(){
		//Application.targetFrameRate = 2000;
		//Debug.Log(Application.targetFrameRate);
		if (File.Exists (Application.persistentDataPath + "/saveData.dat")) {
			var binaryFomatter = new BinaryFormatter ();
			var fileStream = File.Open (Application.persistentDataPath + "/saveData.dat",FileMode.Open);
			savedata = (saveinfo)binaryFomatter.Deserialize(fileStream);

			fileStream.Close();

			if(savedata.equip.name != null){
				hammerslotimg.sprite = Resources.Load<Sprite> ("item/" + savedata.equip.name);
			}
			invenRect.GetComponent<RectTransform> ().sizeDelta = new Vector3 (savedata.inven.Count * 70.0f+2.5f, 70, 0);
		}
		if (PlayerPrefs.GetInt ("hp") >= 0) {
			hp = PlayerPrefs.GetInt ("hp");
			hptxt.text = hp.ToString();
		}
		for (int i = 0; i < savedata.inven.Count; i ++) {
			slotmake(savedata.inven[i],i);
		}
	}
	*/



	public IEnumerator Call(){

		string cAdress = "";
		yield break;
	}

	public void invenClear (){
		foreach (var i in itemslotlist) {
			GameObject.Destroy(i);
		}
		savedata.inven.Clear ();
		itemslotlist.Clear ();
		//Save();
	}

	//--------------체크 함수
	public int haveChk(string name){
		if (savedata.inven.Count == 0) {
			return -1;
		}
		for (int i = 0; i < savedata.inven.Count; i ++) {
			if(savedata.inven[i].name == name){
				return i;
				break;
			}
		}
		return -1;
	}
	public int prevputChk(string name){
		if (savedata.putit.Count == 0) {
			return -1;
		}
		for (int i = 0; i < savedata.putit.Count; i ++) {
			if (savedata.putit [i].name == name) {
				return i;
				break;
			}
		}
		return -1;
	}

	// 로컬획득
	public void putt(string it){
		put (it, 1);
		//Save();
	}
	public void put(string code, int num = 1){
		//putit.Add (it);
		int haveit;
		if (itdb[code]["type"].Value == "재료") {
			haveit = haveChk (itdb[code]["name"].Value);
		} else {
			haveit = -1;
		}
		if (haveit >= 0) {
			savedata.inven [haveit].number += num;
			itemslotlist[haveit].transform.FindChild ("Number").GetComponent<UnityEngine.UI.Text> ().text = savedata.inven [haveit].number.ToString();
			itemslotlist[haveit].GetComponent<SlotBounce> ().startBounce ();
		} else {
			iteminfo data = new iteminfo ();
			data.code = itdb[code]["code"].Value;
			data.name = itdb[code]["name"].Value;
			data.number = num;
			data.type = itdb[code]["type"].Value;
			data.hp = itdb[code]["hp"].AsInt;
			if (itdb[code]["gold"].Value != null)
				data.gold = itdb[code]["gold"].AsInt;
			if (itdb[code]["power"].Value != null)
				data.power = itdb[code]["power"].AsInt;
			savedata.inven.Add (data);
			slotmake (data, savedata.inven.Count - 1);
			invenRect.GetComponent<RectTransform> ().sizeDelta = new Vector3 (savedata.inven.Count * 70.0f+2.5f, 70, 0);
		}
	}
	public void putiteminfo(iteminfo data){
		savedata.inven.Add (data);
		slotmake (data, savedata.inven.Count - 1);
		invenRect.GetComponent<RectTransform> ().sizeDelta = new Vector3 (savedata.inven.Count * 70.0f+2.5f, 70, 0);
		//Save();
	}
	void slotmake(iteminfo data, int i){
		var it = Instantiate (slot, new Vector3 ( 0, 0, 0), Quaternion.identity) as GameObject;
		it.SetActive (true);
		it.transform.SetParent (invenRect.transform);
		it.GetComponent<RectTransform> ().localScale = new Vector3 (1, 1, 1);
		it.GetComponent<RectTransform> ().localPosition = new Vector3 (37.5f + 70.0f * (i), 0, 0);
		if (Resources.Load ("item/" + data.name) != null) {
			it.transform.GetComponent<UnityEngine.UI.Image> ().sprite = Resources.Load<Sprite> ("item/" + data.name);
		} else {
			it.transform.GetComponent<UnityEngine.UI.Image> ().sprite = Resources.Load<Sprite> ("item/없음");
		}
		it.transform.FindChild ("Number").GetComponent<UnityEngine.UI.Text> ().text = data.number.ToString();
		it.GetComponent<itemslotscript> ().name = data.name;
		itemslotlist.Insert (i, it);
		if (data.type != "재료") {
			it.transform.FindChild("Number").gameObject.SetActive(false);
			it.transform.FindChild("Dmg").gameObject.SetActive(true);
		}
		it.GetComponent<SlotBounce> ().startBounce ();
	}
	/*
	void slotmake(iteminfo data, int i){
		var it = Instantiate (slot, new Vector3 ( 0, 0, 0), Quaternion.identity) as GameObject;
		it.SetActive (true);
		it.transform.SetParent (invenRect.transform);
		it.GetComponent<RectTransform> ().localScale = new Vector3 (1, 1, 1);
		it.GetComponent<RectTransform> ().localPosition = new Vector3 (37.5f + 70.0f * (i), 0, 0);
		if (Resources.Load ("item/" + data.name) != null) {
			it.transform.FindChild ("Image").GetComponent<UnityEngine.UI.Image> ().sprite = Resources.Load<Sprite> ("item/" + data.name);
		} else {
			it.transform.FindChild ("Image").GetComponent<UnityEngine.UI.Image> ().sprite = Resources.Load<Sprite> ("item/없음");
		}
		it.transform.FindChild ("Number").GetComponent<UnityEngine.UI.Text> ().text = data.number.ToString();
		it.GetComponent<itemslotscript> ().name = data.name;
		itemslotlist.Insert (i, it);
		if (data.type != "재료") {
				it.transform.FindChild("Number").gameObject.SetActive(false);
			it.transform.FindChild("Dmg").gameObject.SetActive(true);
		}
		it.GetComponent<SlotBounce> ().startBounce ();
	}
	*/






	string findmaterial(string mat){
		int total = itdb.Count;
		for (int i = 0; i < total; i ++) {
			if(itdb[i]["material"].Value == mat){
				return itdb[i]["code"].Value;
				break;
			}
		}
		return "no";
	}


	//--------------------- 로컬 생성
	void make(){
		if (savedata.putit.Count > 0) {
			List<string> putnamelist = new List<string>();
			for(int i = 0; i < savedata.putit.Count; i ++){
				putnamelist.Add(savedata.putit[i].name + ":" + savedata.putit[i].number+",");
			}
			putnamelist.Sort ();
			string putitnamea = "";
			for (int i = 0; i < savedata.putit.Count; i ++) {
				putitnamea = putitnamea.Insert(putitnamea.Length, putnamelist[i]);
			}
			putitnamea = putitnamea.Remove(putitnamea.Length-1);
			//Debug.Log(!putitnamea);
			string chkres = findmaterial(putitnamea);
			if (chkres != "no") {
				StartCoroutine(getrecipe(chkres));
				hptxt.text = itdb[chkres]["name"].Value + "을 만들었다!|";
				int total = savedata.putit.Count;
				for (int i = 0; i < total; i ++){
					StartCoroutine(deletedb(savedata.putit[i].code,savedata.putit[i].number));
				}
				savedata.putit.Clear();
				StartCoroutine(putdb (chkres,1));
			} else {
				hptxt.text = "실패다!";
				int total = savedata.putit.Count;
				for (int i = 0; i < total; i ++){
						put (savedata.putit[i].code,savedata.putit[i].number);
					//Destroy(putitemslotlist[i]);
				}
				savedata.putit.Clear();
				//Save();
				//Debug.Log ("실패다!");
			}
		}
	}



	//--------------- 드롭 함수

	public void dropInputtext(UnityEngine.UI.Text data){
		if (data.text == "") {
			putcancle();
			return;
		}
		int n = System.Convert.ToInt32 (data.text);
		if (n == 0) {
			putcancle();
			return;
		}
		if(currentslot.number < n){
		//	DropCountPannel.GetComponent<UnityEngine.UI.InputField>().text = currentslot.number.ToString();
			n = currentslot.number;
		}
		drop (n);
	}
	public void Inputbackspace(){
		if (DropCountPannel.GetComponent<UnityEngine.UI.InputField> ().text.Length > 0) {
			DropCountPannel.GetComponent<UnityEngine.UI.InputField> ().text = DropCountPannel.GetComponent<UnityEngine.UI.InputField> ().text.Remove (DropCountPannel.GetComponent<UnityEngine.UI.InputField> ().text.Length - 1);
		}
	}
	public void drop(int num){
		int chki = -1;
		if (currentslot.type == "재료") {
			chki = prevputChk (currentslot.name);
		}
		if (chki >= 0) {
			savedata.putit[chki].number += num;
		} else {
			iteminfo cl = new iteminfo();
			cl.code = currentslot.code;
			cl.name = currentslot.name;
			cl.hp = currentslot.hp;
			cl.gold = currentslot.gold;
			cl.power = currentslot.power;
			cl.type = currentslot.type;
			cl.number = num;
			savedata.putit.Add (cl);
		}
		currentslot.number = currentslot.number-num;
		hp += currentslot.hp*num;
		hptxt.text = hp.ToString ();
		DropCountPannel.transform.parent.gameObject.SetActive (false);
		if (currentslot.number > 0) {
			savedata.inven.Insert(putnum, currentslot);
			slotmake(currentslot, putnum);
			//putitemslotlist[putitemslotlist.Count-1].transform.FindChild ("Number").GetComponent<UnityEngine.UI.Text> ().text = num.ToString();
			for(int i = putnum; i < savedata.inven.Count; i ++){
				itemslotlist[i].GetComponent<slotmovescript>().spot = 37.5f +i * 70.0f;
				itemslotlist[i].GetComponent<slotmovescript>().a = 0;
				itemslotlist[i].GetComponent<slotmovescript>().enabled = true;
			}
		} else {

		}
		//Save();
	}
	public void putcancle(){
		if (currentslot.number > 0) {
			savedata.inven.Insert(putnum, currentslot);
			slotmake(currentslot, putnum);
			//putitemslotlist[putitemslotlist.Count-1].transform.FindChild ("Number").GetComponent<UnityEngine.UI.Text> ().text = num.ToString();
			for(int i = putnum; i < savedata.inven.Count; i ++){
				itemslotlist[i].GetComponent<slotmovescript>().spot = 37.5f +i * 70.0f;
				itemslotlist[i].GetComponent<slotmovescript>().a = 0;
				itemslotlist[i].GetComponent<slotmovescript>().enabled = true;
			}
		} 
	}
	public void numbutton(string num){
		DropCountPannel.GetComponent<UnityEngine.UI.InputField>().text += num;
		if (System.Convert.ToInt32 (DropCountPannel.GetComponent<UnityEngine.UI.InputField>().text) > currentslot.number) {
			DropCountPannel.GetComponent<UnityEngine.UI.InputField>().text = currentslot.number.ToString();
		}

	}
	public void inputChk(string num){
		int n = System.Convert.ToInt32(num);
		if(currentslot.number < n){
			DropCountPannel.GetComponent<UnityEngine.UI.InputField>().text = currentslot.number.ToString();
		}
	}

	public void recipechk(RectTransform slot){
		int n = Mathf.FloorToInt((slot.anchoredPosition.x+5) / 110 - 4 * (slot.anchoredPosition.y +5)/ 135);
		Debug.Log (n);
		string code = savedata.recipeinven [n];
		recipeinvenRect.transform.parent.parent.FindChild ("itname").GetComponent<UnityEngine.UI.Text> ().text = itdb [code] ["name"].Value;
		//slot.parent.FindChild ("material").GetComponent<UnityEngine.UI.Text> ().text = itdb [code] ["material"].Value;

		char[] pat = new char[2];
		pat[0] = ':'; pat [1] = ',';
		string[] matar = itdb [code] ["material"].Value.Split(pat);
		int total = matar.Length;
		//List<string> matname = new List<string> ();
		//List<string> matnum = new List<string> ();
		string res = "- 필요재료 -\n";
		for (int i = 0; i < total; i += 2) {
			res = res + "\n" + matar[i] + " × " + matar[i+1];
			//matname.Add(mtr[0]);
			//matnum.Add(mtr[1]);
		}
		recipeinvenRect.transform.parent.parent.FindChild ("material").GetComponent<UnityEngine.UI.Text> ().text = res;
		recipeinvenRect.transform.parent.parent.FindChild ("itemslot").FindChild ("Image").GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite> ("item/" + itdb[code]["name"].Value);
		recipeinvenRect.transform.parent.parent.FindChild ("itemslot").gameObject.SetActive (true);
	}

	public string chktype(int type){
		switch (type) {
		case 1:
			return "전사";
			break;
		case 2:
			return "성직자";
			break;
		case 3:
			return "광부";
			break;
		case 4:
			return "짐꾼";
			break;
		default: return "평민";break;
		}
	}

	public void partyChk(int n){
		Transform teaminven = teaminvenRect.transform.parent.parent;
		string code = savedata.teaminven[partylist[PlayerPrefs.GetInt("pnum"),n]].code;
		sellectedCharacter = n;
		teaminven.FindChild ("itname").GetComponent<UnityEngine.UI.Text> ().text = teamdb [code] ["name"].Value;
		//slot.parent.FindChild ("material").GetComponent<UnityEngine.UI.Text> ().text = itdb [code] ["material"].Value;
		string type = chktype(teamdb[code]["type"].AsInt);
		
		teaminven.FindChild ("infot").GetComponent<UnityEngine.UI.Text> ().text = "[ " + type + " ]\n 체력 : " + teamdb [code] ["hp"].Value + "\n공격력 : "  + teamdb [code] ["power"].Value + "\n소지가능갯수 : " + teamdb [code] ["inven"].Value;
		teaminven.FindChild("CharacterImage").GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("team/"+teamdb[code]["name"].Value);
		teaminven.FindChild ("CharacterImage").GetComponent<UnityEngine.UI.Image> ().SetNativeSize ();
		//teaminvenRect.transform.parent.parent.FindChild ("itemslot").FindChild ("Image").GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite> ("item/" + teamdb[code]["name"].Value);
		//teaminvenRect.transform.parent.parent.FindChild ("itemslot").gameObject.SetActive (true);
		teaminven.FindChild ("방출").gameObject.SetActive (true);
		teaminven.FindChild ("합류").gameObject.SetActive (false);
	}

	public void teamchk(RectTransform slot){
		//( 5 + 110 * (i - 3 * Mathf.Floor(i/3)),-5 - 155 * Mathf.Floor(i/3), 0);
		Transform teaminven = teaminvenRect.transform.parent.parent;
		int n = Mathf.FloorToInt(slot.anchoredPosition.x / 110 - 3 * slot.anchoredPosition.y / 155);
		string code = savedata.teaminven [n].code;
		sellectedCharacter = n;
		teaminven.FindChild ("itname").GetComponent<UnityEngine.UI.Text> ().text = teamdb [code] ["name"].Value;
		//slot.parent.FindChild ("material").GetComponent<UnityEngine.UI.Text> ().text = itdb [code] ["material"].Value;
			string type = chktype(teamdb[code]["type"].AsInt);

		teaminven.FindChild ("infot").GetComponent<UnityEngine.UI.Text> ().text = "[ " + type + " ]\n 체력 : " + teamdb [code] ["hp"].Value + "\n공격력 : "  + teamdb [code] ["power"].Value + "\n소지가능갯수 : " + teamdb [code] ["inven"].Value;
		teaminven.FindChild("CharacterImage").GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("team/"+teamdb[code]["name"].Value);
		teaminven.FindChild ("CharacterImage").GetComponent<UnityEngine.UI.Image> ().SetNativeSize ();
		//teaminvenRect.transform.parent.parent.FindChild ("itemslot").FindChild ("Image").GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite> ("item/" + teamdb[code]["name"].Value);
		//teaminvenRect.transform.parent.parent.FindChild ("itemslot").gameObject.SetActive (true);
		teaminven.FindChild ("방출").gameObject.SetActive (false);
		if (savedata.teaminven [n].party == -1) {
			teaminven.FindChild ("합류").gameObject.SetActive (true);
		} else {
			teaminven.FindChild ("합류").gameObject.SetActive (false);
		}
	}
	public void mailchk(RectTransform slot){
		int n = Mathf.FloorToInt(-slot.anchoredPosition.y / 70);
		mailinvenRect.transform.parent.parent.FindChild ("mailtitle").GetComponent<UnityEngine.UI.Text> ().text = maildata [n] ["title"].Value;
		mailinvenRect.transform.parent.parent.FindChild ("body").GetComponent<UnityEngine.UI.Text> ().text = maildata [n] ["body"].Value;
		if (maildata [n] ["item"] != null) {

		}
		//StartCoroutine (maildata [n] ["mailid"].Value);
	}
	public IEnumerator maildelete(string n){
		WWWForm cForm = new WWWForm ();
		cForm.AddField ("mailid", n);
		WWW wwwUrl = new WWW (readmailAdress, cForm);
		yield return wwwUrl;
		Debug.Log (wwwUrl.text);
	}
	public void setstate(int w){
		Userinfo.Instance.area = w;
		if(areaCameraAnim.enabled == false)
		areaCameraAnim.enabled = true;

		areaCameraAnim.SetInteger ("area", w);
		doorAnim.SetInteger ("area", w);
	}
}
