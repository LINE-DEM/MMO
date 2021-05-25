using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DelTest : MonoBehaviour {

	public delegate void PlayerDeth(HowClose a);
	public event PlayerDeth OnDath;

	public enum HowClose
	{
		fast = 1,
		slow = 2,
	}

	// Use this for initialization
	public virtual void Start () {
		OnDath += DelPlayer;
		OnDath += CloseWindow;
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void DelPlayer(HowClose a)
	{
		if (a == HowClose.fast)
			Debug.Log("角色被快速删除");
		else
			Debug.Log("角色慢速删除");
	}

	void CloseWindow(HowClose a)
	{
		if (a == HowClose.fast)
			Debug.Log("窗口被快速关闭");
		else
			Debug.Log("窗口被慢速关闭");
	}

	public void OnClose(HowClose a)
	{
		OnDath(a);
	}
}
