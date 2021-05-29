using Common.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : Singleton<ShopManager> {

	public void Init()
	{
		NPCManager.Instance.RegisterNpcEvent(NpcFunction.InvokeShop, OnOpenShop);
	}

	private bool OnOpenShop(NpcDefine npc)
	{
		this.ShowShop(npc.Param);
		return true; 
	}

	public void ShowShop(int shopId)
	{
		ShopDefine shop;
		if(DataManager.Instance.Shops.TryGetValue(shopId ,out shop))
		{
			UIShop uiShop = UIManager.Instance.Show<UIShop>();
			if(uiShop != null)
			{
				uiShop.SetShop(shop);
			}
		}
	}
	public bool BuyItem(int shopId , int shopItemid)
	{
		ItemService.Instance.SendBuyItem(shopId, shopItemid);
		return true;
	}
}
