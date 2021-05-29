using Common;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class ItemService: Singleton<ItemService>
    {
        public ItemService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ItemBuyRequest>(this.OnItemBuy);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ItemEquipRequest>(this.OnItemEquip);
        }

        public void Init()
        {

        }

        void OnItemBuy(NetConnection<NetSession> sender,ItemBuyRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("道具购买 角色{0} 商店{1} 道具{2}", character.Id, request.shopId, request.shopItemId);
            var result = ShopManager.Instance.BuyItem(sender, request.shopId, request.shopItemId);
            sender.Session.Response.itemBuy = new ItemBuyRespones();
            sender.Session.Response.itemBuy.Result = result;
            sender.SendResponse();
        }

        void OnItemEquip(NetConnection<NetSession> sender, ItemEquipRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("道具装备 角色：{0} Slot：{1} 道具：{2} 装备：{3}", character.Id, request.Slot, request.itemId,request.isEquip);
            var result = EquipManager.Instance.EquipItem(sender, request.Slot, request.itemId,request.isEquip);
            sender.Session.Response.itemEquip = new ItemEquipResponse();
            sender.Session.Response.itemEquip.Result = result;
            sender.SendResponse();
        }

    }
}
