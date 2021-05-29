using Network;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Events;

public class SkillService :Singleton<SkillService> {

	public UnityAction OnSkillRefrash;


	public void Init()
	{

	}

	public SkillService()
	{
		MessageDistributer.Instance.Subscribe<SkillOpenResponse>(OnSkillOpen);
	}


	public int Id;
	public void SendSkillOpenRequest(int id)
	{
		NetMessage message = new NetMessage();
		message.Request = new NetMessageRequest();
		message.Request.skillAddReq = new SkillOpenRequest();
		message.Request.skillAddReq.Id = id;
		Id = id;
		NetClient.Instance.SendMessage(message);
	}

	private void OnSkillOpen(object sender, SkillOpenResponse message)
	{
		if (message.Result == Result.Success)
		{
			MessageBox.Show("学习成功");
			SkillManager.Instance.Init(message.Skillinfos);
			OnSkillRefrash();
		}
			
		else
			MessageBox.Show("学习失败 请先学习基础技能或者已经学习过了");
		
	}
}
