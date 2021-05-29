using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIQuestDialog : UIWindow {

	public UIQuestInfo questInfo;

	public Quest quest;

	public GameObject openButons;
	public GameObject submitButtons;


	/// <summary>
	///				三种情况
	/// </summary>
	/// <param name="quest"></param>
	public void SetQuest(Quest quest)
	{
		this.quest = quest;
		this.UpdateQuest();
		//如果 任务信息为空 说明是可接的任务 
		
		if(this.quest.Info == null)
		{
			openButons.SetActive(true);
			submitButtons.SetActive(false);
		}
		else
		{
			if(this.quest.Info.Status == SkillBridge.Message.QuestStatus.Complated)
			{
				openButons.SetActive(false);
				submitButtons.SetActive(true);
			}
			else
			{
				openButons.SetActive(false);
				submitButtons.SetActive(false);
			}
			
		}
	}

	void UpdateQuest()
	{
		if(this.quest != null)
		{
			if(this.questInfo != null)
			{
				this.questInfo.SetQuestInfo(quest);
			}
		}
	}
	
}
