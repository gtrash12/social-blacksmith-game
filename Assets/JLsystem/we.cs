using UnityEngine;
using System.Collections;
using SimpleJSON;

public class we : MonoBehaviour {
	public UnityEngine.UI.InputField fuserid;
	public UnityEngine.UI.InputField fname;
	public UnityEngine.UI.InputField fpasswd;
	public UnityEngine.UI.InputField fpasswdr;
	public UnityEngine.UI.InputField fage;
	public string fsex = "man";
	string cAdress = "http://galtragames.com/joinchk_unity.php";
	string iAdress = "http://galtragames.com/id_chk_unity.php";
	string pAdress = "http://galtragames.com/puttoDB.php";
	string lAdress = "http://galtragames.com/login_unity.php";
	string nAdress = "http://galtragames.com/blacksmith_join_chk.php";
	string cnAdress = "http://galtragames.com/namechk_unity.php";
	string pbAdress = "http://galtragames.com/putto_blacksmith.php";
	bool idchkresult = false;
	public UnityEngine.UI.Text chkmessage;

	public UnityEngine.UI.InputField logid;
	public UnityEngine.UI.InputField logpw;

	public UnityEngine.UI.InputField nameset;
	void Start () {
		//StartCoroutine (this.Call (cAdress));
		fsex = "man";
	}

	public void setman(){
		fsex = "man";
	}
	public void setwoman(){
		fsex = "wom";
	}

	public void resetidchk(){
		if (idchkresult == true) {
			chkmessage.text = "";
			idchkresult = false;
		}
	}
	public void go(){
		if (idchkresult == true) {
			StartCoroutine (this.Call (cAdress));
		} else {
			Debug.Log("ID중복확인을 수행해주세요.");
		}
	}
	public void chkbtt(){
		StartCoroutine (this.idChk (iAdress));
	}
	public void loginbtt(){
		StartCoroutine (loginchk(lAdress));
	}
	public void namebtt(){
		StartCoroutine (gameinputDB (cnAdress));
	}

	public IEnumerator loginchk(string _adress){
		WWWForm cForm = new WWWForm ();
		cForm.AddField ("fuserid", logid.text,System.Text.Encoding.UTF8);
		cForm.AddField ("fpasswd", logpw.text);
		WWW wwwUrl = new WWW(_adress, cForm);
		yield return wwwUrl;
		Debug.Log (wwwUrl.text);
		if (wwwUrl.text.Contains ("ok")) {
			wwwUrl = new WWW (nAdress, cForm);
			yield return wwwUrl;
			if (wwwUrl.text.Contains ("no")) {
				nameset.transform.parent.gameObject.SetActive (true);
			} else {
				JSONNode json_data = JSONNode.Parse(wwwUrl.text);
				Userinfo.Instance.id = json_data["id"].Value;
				Userinfo.Instance.name = json_data["name"].Value;
				Userinfo.Instance.gold = json_data["gold"].AsInt;
				Userinfo.Instance.visittime = json_data["visittime"].AsInt;
				Application.LoadLevel ("Main");
			}
		}
	}

	public IEnumerator gameinputDB(string _adress){
		WWWForm cForm = new WWWForm ();
		cForm.AddField ("fname", nameset.text, System.Text.Encoding.UTF8);
		WWW wwwUrl = new WWW(_adress, cForm);
		yield return wwwUrl;
		if (wwwUrl.text.Contains ("ok")) {
			Userinfo.Instance.id = logid.text;
			Userinfo.Instance.name = nameset.text;
			cForm.AddField ("fuserid", logid.text, System.Text.Encoding.UTF8);
			wwwUrl = new WWW(pbAdress, cForm);
			yield return wwwUrl;
			Application.LoadLevel ("Main");
			PlayerPrefs.SetString("ID",logid.text);
		} else {
			Debug.Log(wwwUrl.text);
		}
	}

	public IEnumerator idChk(string _adress){
		WWWForm cForm = new WWWForm ();
		cForm.AddField ("fuserid", fuserid.text, System.Text.Encoding.UTF8);
		WWW wwwUrl = new WWW(_adress, cForm);
		yield return wwwUrl;
		if (wwwUrl.text.Contains ("ok")) {
			chkmessage.text = "사용 가능한 ID입니다.";
			chkmessage.color = new Color (0, 1, 0);
			idchkresult = true;
		} else {
			chkmessage.text = wwwUrl.text;
			chkmessage.color = new Color (1, 0, 0);
			idchkresult = true;
		}
	}
	public IEnumerator Call(string _adress){
		WWWForm cForm = new WWWForm ();
		cForm.AddField ("fuserid", fuserid.text, System.Text.Encoding.UTF8);
		cForm.AddField ("fname", fname.text, System.Text.Encoding.UTF8);
		cForm.AddField ("fpasswd", fpasswd.text);
		cForm.AddField ("fpasswdr", fpasswdr.text);
		cForm.AddField ("fage", fage.text);
		WWW wwwUrl = new WWW(_adress, cForm);
		yield return wwwUrl;
		Debug.Log (wwwUrl.text);
		if (wwwUrl.text.Contains("성공")) {
			cForm.AddField ("fsex", fsex);
			wwwUrl = new WWW(pAdress, cForm);
			yield return wwwUrl;
			fuserid.transform.parent.gameObject.SetActive(false);
		}
	}
}
