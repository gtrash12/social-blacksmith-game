using UnityEngine;
using System.Collections;

public class Playerscript : MonoBehaviour {
	public GM gm;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void atk(){
		gm.canim.Play ("shake1");
		gm.canim.Rebind ();
		gm.particleEff.Play();
		gm.hp -= gm.power + gm.savedata.equip.power;
		gm.fevermiter += gm.feverpower;
		gm.feverguage.fillAmount = gm.fevermiter;
		gm.hptxt.text = gm.hp.ToString();
		gm.tableanim.Rebind();
		if (gm.hp <= 0) {
			gm.hp = 0;
			gm.SendMessage ("make");
		}
		if(gm.anim.speed == 1 && gm.fevermiter >= 1){
			StartCoroutine(gm.fever());
		}
	}
}
