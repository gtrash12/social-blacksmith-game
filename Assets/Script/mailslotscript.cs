using UnityEngine;
using System.Collections;

public class mailslotscript : MonoBehaviour {
	public UnityEngine.UI.Text title;
	public UnityEngine.UI.Text sender;
	public UnityEngine.UI.Text senddate;

	public void set(string titlet, string sendert, string senddatet){
		title.text = titlet;
		sender.text = sendert;
		senddate.text = senddatet;
	}
}
