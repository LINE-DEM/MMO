using Common.Data;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : Singleton<SkillManager> {

	public List<NSkillInfo> skills = new List<NSkillInfo>();

	//public Dictionary<int, NSkillInfo> skills = null;

	public void Init(List<NSkillInfo> info)
	{
		skills.Clear();
		foreach (var item in info)
		{
			item.Id = item.Id % 8;
			skills.Add(item);
		}

	}

	


	public void OpenSkill(int id)
	{
		SkillService.Instance.SendSkillOpenRequest(id);
	}
	
}
