using Common.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : Singleton<NPCManager>{

	//这个能注册的方法需要传入npc信息
	public delegate bool NpcActionHandler(NpcDefine npc);
	//一共就 两个委托变量 
	Dictionary<NpcFunction, NpcActionHandler> eventMap = new Dictionary<NpcFunction, NpcActionHandler>();

	public void RegisterNpcEvent(NpcFunction function, NpcActionHandler action)
	{
		if (!eventMap.ContainsKey(function))
		{
			eventMap[function] = action;
		}
		else
			eventMap[function] += action;
		//每一个委托变量都能注册很多方法
	}

	public NpcDefine GetNpcDefine(int npcID)
	{
		NpcDefine npc = null;
		DataManager.Instance.Npcs.TryGetValue(npcID, out npc);
		return npc;
	}

	public bool Interactive(int npcId)
	{
		if (DataManager.Instance.Npcs.ContainsKey(npcId))
		{
			var npc = DataManager.Instance.Npcs[npcId];
			return Interactive(npc);
		}
		return false;
	}
	public bool Interactive(NpcDefine npc)
	{
		if (DoTaskInteractive(npc))
		{
			return true;
		}
		else if(npc.Type == NpcType.Functional)
		{
			return DoFunctionInteractive(npc);
		}
		
		return false;
	}

	private bool DoTaskInteractive(NpcDefine npc)
	{
		var status = QuestManager.Instance.GetQuestStatusByNpc(npc.ID);
		if (status == NpcQuestStatus.None)
			return false;


		return QuestManager.Instance.OpenNpcQuest(npc.ID);
	}
	private bool DoFunctionInteractive(NpcDefine npc)
	{
		//aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
		if (npc.Type != NpcType.Functional)
			return false;
		if (!eventMap.ContainsKey(npc.Function))
			return false;
		//根据这个npc的功能选择是哪个方法列表
		return eventMap[npc.Function](npc);
	}

}
