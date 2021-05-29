﻿using Models;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum NpcQuestStatus
{
	None = 0,	//无任务
	Complete,	//完成 可以提交的任务
	Available,	//可以接收 的任务
	Incomplete, //未完成 的任务
}

public class QuestManager : Singleton<QuestManager> {

	public List<NQuestInfo> questInfos;
	public Dictionary<int, Quest> allQuests = new Dictionary<int, Quest>();

	public Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>> npcQuests = new Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>>();

	public UnityAction<Quest> onQuestStatusChanged; 

	//登陆的时候就从响应里发送来的Character里获取到info列表了
	public void Init(List<NQuestInfo> quests)
	{
		this.questInfos = quests;
		allQuests.Clear();
		this.npcQuests.Clear();
		InitQuests();
	}

	void InitQuests()
	{
		//把接了的任务放入 allQuests里
		foreach (var info in this.questInfos)
		{
			Quest quest = new Quest(info);
			
			this.allQuests[quest.Info.QuestId] = quest;
		}
		//把 可以接的任务 放入 allQuests里 筛除不能接受的任务
		CheckAvailableQuests();

		foreach (var kv in this.allQuests)
		{
			this.AddNpcQuest(kv.Value. Define.AcceptNPC, kv.Value);
			this.AddNpcQuest(kv.Value.Define.SubmitNPC, kv.Value);
		}
		
	}

	//把 可以接的任务 放入 allQuests里
	void CheckAvailableQuests()
	{
		foreach (var kv in DataManager.Instance.Quests)
		{
			if (kv.Value.LimitClass != CharacterClass.None && kv.Value.LimitClass != User.Instance.CurrentCharacter.Class)
				continue;//不符合职业
			if (kv.Value.LimitLevel > User.Instance.CurrentCharacter.Level)
				continue;//不符合等级
			if (this.allQuests.ContainsKey(kv.Key))
				continue;//任务已经存在

			if (kv.Value.PreQuest > 0)
			{
				Quest preQuest;
				if (this.allQuests.TryGetValue(kv.Value.PreQuest, out preQuest))
				{
					if (preQuest.Info == null)
						continue;//前置任务未接取
					if (preQuest.Info.Status != QuestStatus.Finished)
						continue;//前置任务未完成
				}
				else
					continue;//前置任务还没接
			}

			//筛选后的Quest
			Quest quest = new Quest(kv.Value);
			this.allQuests[quest.Define.ID] = quest;
		}
	}

	//把 quest 添加给Npc
	void AddNpcQuest(int npcId, Quest quest)
	{
		if (!this.npcQuests.ContainsKey(npcId))
			this.npcQuests[npcId] = new Dictionary<NpcQuestStatus, List<Quest>>();

		List<Quest> availables;
		List<Quest> complates;
		List<Quest> incomplates;

		//给这个NPC字典初始化三个任务列表
		if(!this.npcQuests[npcId].TryGetValue(NpcQuestStatus.Available,out availables))
		{
			availables = new List<Quest>();
			this.npcQuests[npcId][NpcQuestStatus.Available] = availables;
		}
		if(!this.npcQuests[npcId].TryGetValue(NpcQuestStatus.Complete,out complates))
		{
			complates = new List<Quest>();
			this.npcQuests[npcId][NpcQuestStatus.Complete] = complates;
		}
		if(!this.npcQuests[npcId].TryGetValue(NpcQuestStatus.Incomplete,out incomplates))
		{
			incomplates = new List<Quest>();
			this.npcQuests[npcId][NpcQuestStatus.Incomplete] = incomplates;
		}

		//
		if (quest.Info == null)
		{
			if (npcId == quest.Define.AcceptNPC &&
				!this.npcQuests[npcId][NpcQuestStatus.Available].Contains(quest))
			{
				this.npcQuests[npcId][NpcQuestStatus.Available].Add(quest);
			}
		}
		else
		{
			if(quest.Define.SubmitNPC == npcId && quest.Info.Status == QuestStatus.Complated)
			{
				if (!this.npcQuests[npcId][NpcQuestStatus.Complete].Contains(quest))
				{
					this.npcQuests[npcId][NpcQuestStatus.Complete].Add(quest);
				}
			}
			if(quest.Define.SubmitNPC == npcId && quest.Info.Status == QuestStatus.InProgress)
			{
				if (!this.npcQuests[npcId][NpcQuestStatus.Incomplete].Contains(quest))
				{
					this.npcQuests[npcId][NpcQuestStatus.Incomplete].Add(quest);
				}
			}
		}
	}


	/// <summary>
	/// 获取 Npc 任务状态 (为了给头顶小图标用的)
	/// </summary>
	/// <param name="npcId"></param>
	/// <returns></returns>
	public NpcQuestStatus GetQuestStatusByNpc(int npcId)
	{
		Dictionary<NpcQuestStatus, List<Quest>> status = new Dictionary<NpcQuestStatus, List<Quest>>();
		if(this.npcQuests.TryGetValue(npcId , out status))
		{
			if (status[NpcQuestStatus.Complete].Count > 0)
				return NpcQuestStatus.Complete;
			if (status[NpcQuestStatus.Available].Count > 0)
				return NpcQuestStatus.Available;
			if (status[NpcQuestStatus.Incomplete].Count > 0)
				return NpcQuestStatus.Incomplete;
		}
		return NpcQuestStatus.None;
	}

	/// <summary>
	///		打开任务对话框
	/// </summary>
	/// <param name="npcid"></param>
	/// <returns></returns>
	public bool OpenNpcQuest(int npcid)
	{
		Dictionary<NpcQuestStatus, List<Quest>> status = new Dictionary<NpcQuestStatus, List<Quest>>();
		if(this.npcQuests.TryGetValue(npcid , out status))
		{
			if (status[NpcQuestStatus.Complete].Count > 0)
				return ShowQuestDialog(status[NpcQuestStatus.Complete].First());
			if (status[NpcQuestStatus.Available].Count > 0)
				return ShowQuestDialog(status[NpcQuestStatus.Available].First());
			if (status[NpcQuestStatus.Incomplete].Count > 0)
				return ShowQuestDialog(status[NpcQuestStatus.Incomplete].First());

		}
		return false;
	}


	bool ShowQuestDialog(Quest quest)
	{
		if(quest.Info == null || quest.Info.Status == QuestStatus.Complated)
		{
			UIQuestDialog dlg = UIManager.Instance.Show<UIQuestDialog>();
			dlg.SetQuest(quest);
			dlg.OnClose += OnQuestDialogClose;
		}
		if (quest.Info != null)
		{
			if (!string.IsNullOrEmpty(quest.Define.DialogIncomplete))
				MessageBox.Show(quest.Define.DialogIncomplete);
		}
		return true;
	}

	void OnQuestDialogClose(UIWindow sender,UIWindow.WindowResult result)
	{
		UIQuestDialog dlg = (UIQuestDialog)sender;
		if(result == UIWindow.WindowResult.Yes)
		{
			if (dlg.quest.Info == null)
				QuestService.Instance.SendQuestAccept(dlg.quest);
			else if (dlg.quest.Info.Status == QuestStatus.Complated)
				QuestService.Instance.SendQuestSubmit(dlg.quest);
		}
		else if(result == UIWindow.WindowResult.No)
		{
			MessageBox.Show(dlg.quest.Define.DialogDeny);
		}
	}

	Quest RefreshQuestStatus(NQuestInfo quest)
	{
		this.npcQuests.Clear();
		Quest result;
		if (this.allQuests.ContainsKey(quest.QuestId))
		{
			//更新任务的状态
			this.allQuests[quest.QuestId].Info = quest;
			result = this.allQuests[quest.QuestId];
		}
		else
		{
			result = new Quest(quest);
			this.allQuests[quest.QuestId] = result;
		}

		CheckAvailableQuests();

		foreach (var kv in this.allQuests)
		{
			this.AddNpcQuest(kv.Value.Define.AcceptNPC, kv.Value);
			this.AddNpcQuest(kv.Value.Define.SubmitNPC, kv.Value);
		}

		if (onQuestStatusChanged != null)
			onQuestStatusChanged(result);
		return result;
	}


	/// <summary>
	/// 两个方法给Service响应
	/// </summary>
	/// <param name="info"></param>
	public void OnQuestAccepted(NQuestInfo info)
	{
		var quest = this.RefreshQuestStatus(info);
		MessageBox.Show(quest.Define.DialogAccept);
	}

	public void OnQuestSubmited(NQuestInfo info)
	{
		var quest = this.RefreshQuestStatus(info);
		MessageBox.Show(quest.Define.DialogFinish);
	}
} 
