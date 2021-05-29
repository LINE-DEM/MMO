﻿using Common;
using Common.Data;
using GameServer.Services;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
     class ShopManager : Singleton<ShopManager>
    {
        public Result BuyItem(NetConnection<NetSession> sender,int shopId ,int shopItemId)
        {
            if (!DataManager.Instance.Shops.ContainsKey(shopId))
                return Result.Failed;

            ShopItemDefine shopItem;
            if(DataManager.Instance.ShopItems[shopId].TryGetValue(shopItemId ,out shopItem))
            {
                Log.InfoFormat("买道具了 角色：{0} 道具{1} 数量{2} 价格{3}",
                    sender.Session.Character.Id ,shopId,shopItem.Count ,shopItem.Price);
                if(sender.Session.Character.Gold > shopItem.Price)
                {
                    sender.Session.Character.ItemManager.AddItem(shopItem.ItemID, shopItem.Count);
                    sender.Session.Character.Gold -= shopItem.Price;
                    DBService.Instance.Save();
                    return Result.Success;
                }
                
            }
            return Result.Failed;
        }
    }
}
