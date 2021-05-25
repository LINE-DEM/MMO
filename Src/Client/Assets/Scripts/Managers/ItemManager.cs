using Common.Data;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	}

	public ItemDefine GetItem(int itemId)
	{


		return null;
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
