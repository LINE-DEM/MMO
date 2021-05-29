using Common.Data;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 维护的是 用户的道具信息 1null    2null    3 item(3)  4 null    5 item(5)
/// 道具添加（调用背包）
/// </summary>
public class ItemManager : Singleton<ItemManager> {


	public Dictionary<int, Item> Items = new Dictionary<int, Item>();
	internal void Init(List<NItemInfo> items)
	{
		this.Items.Clear();
		foreach (var info in items)
		{
			Item item = new Item(info);
			this.Items.Add(item.Id, item);

			Debug.LogFormat("物品管理器初始化 {0}", item);
		}
		
		StatusService.Instance.RegisterStatusNofity(StatusType.Item, OnItemNotify);
	}

	public ItemDefine GetItem(int itemId)
	{


		return null;
	}

	bool OnItemNotify(NStatus status)
	{
		if(status.Action == StatusAction.Add)
		{
			this.AddItem(status.Id, status.Value);
		}
		if (status.Action == StatusAction.Delete)
		{
			this.RemoveItem(status.Id, status.Value);
		}
		return true;
	}

	void AddItem(int itemId,int count)
	{
		Item item = null;
		if(this.Items.TryGetValue(itemId,out item))
		{
			item.Count += count;
		}
		else
		{
			item = new Item(itemId, count);
			this.Items.Add(itemId, item);
		}
		BagManager.Instance.AddItem(itemId, count);
	}

	void RemoveItem(int itemId,int count)
	{
		if (!this.Items.ContainsKey(itemId))
			return;
		Item item = this.Items[itemId];
		if (item.Count < count)
			return;
		item.Count -= count;

		BagManager.Instance.RemoveItem(itemId, count);
	}
	public bool UseItem(int itemId)
	{
		return false;
	}

	public bool UseItem(ItemDefine item)
	{
		return false;
	}

	//没写完！！！！！！！
}
