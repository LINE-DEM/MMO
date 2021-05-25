using Common.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : Singleton<NPCManager>{

	public delegate bool NpcActionHandler(NpcDefine npc);
	Dictionary<NpcFunction, NpcActionHandler> eventMap = new Dictionary<NpcFunction, NpcActionHandler>();

	public void RegisterNpcEvent(NpcFunction function, NpcActionHandler action)
	{
		if (!eventMap.ContainsKey(function))
		{
			eventMap[function] = action;
		}
		else
			eventMap[function] += action;
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
		if (npc.Type == NpcType.Functional)
		{
			return DoFunctionInteractive(npc);
		}
		else if(npc.Type == NpcType.Task)
		{
			return DoTaskInteractive(npc);
		}
		
		return false;
	}

	private bool DoTaskInteractive(NpcDefine npc)
	{
		MessageBox.Show("点击了Npc：" + npc.Name, "是NPC对话");
		return true;
	}
	private bool DoFunctionInteractive(NpcDefine npc)
	{
		//aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
		if (npc.Type != NpcType.Functional)
			return false;
		if (!eventMap.ContainsKey(npc.Function))
			return false;
		return eventMap[npc.Function](npc);
	}

}
