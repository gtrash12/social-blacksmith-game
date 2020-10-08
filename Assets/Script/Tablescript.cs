using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Tablescript : MonoBehaviour, IPointerClickHandler {
	GM gm;
	void Awake(){
		gm = GameObject.Find ("GM").GetComponent<GM> ();
	}
	public void OnPointerClick(PointerEventData data){
		if (gm.hp > 0 && gm.anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "idle") {
			gm.anim.Play("attack");
			/*
			gm.particleEff.Play();
			gm.hp -= gm.power;
			gm.hptxt.text = gm.hp.ToString();
			if (gm.hp <= 0) {
				gm.hp = 0;
				gm.SendMessage ("make");
			}
			*/
		}
	}
}
