using UnityEngine;
using System.Collections;

public class infoboxscript : MonoBehaviour {
	public UnityEngine.UI.Text namet;
	public UnityEngine.UI.Text infot;
	public UnityEngine.UI.Image img;
	public GM gm;
	// Use this for initialization
	public void info(){
		namet.text = gm.currentslot.name;
		if (Resources.Load ("item/" + gm.currentslot.name) != null) {
			img.sprite = Resources.Load<Sprite> ("item/" + gm.currentslot.name);
		} else {
			img.sprite = Resources.Load<Sprite> ("item/없음");
		}
		infot.text = "타입 : " + gm.currentslot.type + "\n힘 - " + gm.currentslot.power + "\n가격 - " + gm.currentslot.gold;
	}
}
