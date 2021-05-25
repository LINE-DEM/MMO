using Common.Data;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

	public int Id;
	public int Count;
	public ItemDefine Define;
	public Item(NItemInfo item)
	{
		this.Id = item.Id;
		this.Count = item.Count;
		this.Define = DataManager.Instance.Items[item.Id];
	}

	public override string ToString()
	{
		return string.Format("物品id {0} 数量：{1}", this.Id, this.Count);
	}
}
