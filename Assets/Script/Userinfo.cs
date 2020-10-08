using UnityEngine;
using System.Collections;

public class Userinfo {
	private static Userinfo instance = null;
	public static Userinfo Instance
	{
		get
		{
			if(instance==null)
			{
				instance = new Userinfo();
			}
			return instance;
		}
	}

	private Userinfo(){
	}

	public string id;
	public string name;
	public int area;
	public int gold;
	public int visittime;
	public GM gm;
}
