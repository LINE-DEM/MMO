using Common.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class TestManager :Singleton<TestManager> {

	// Use this for initialization
	void Start () {
		
	}

	public  void init()
	{
		NPCManager.Instance.RegisterNpcEvent(Common.Data.NpcFunction.InvokeInsrance, OpenInvokeInsrance);
		NPCManager.Instance.RegisterNpcEvent(Common.Data.NpcFunction.InvokeShop, OpenInvokeShop);
	}
	private bool OpenInvokeShop(NpcDefine npc)
	{
		UITest temp = UIManager.Instance.Show<UITest>();
		temp.SetTitle(npc.Name);
		return true;
	}

	private bool OpenInvokeInsrance(NpcDefine npc)
	{
		MessageBox.Show("点击了" + npc.Name + "Npc的对话");
		return true;
	}

	
	void Update () {
		
	}

}
