using Common;
using GameServer.Entities;
using GameServer.Models;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// 一个角色一个ItemManger
/// </summary>
namespace GameServer.Managers
{
    class ItemManager
    {
        Character Owner;

        //存的是封装过的Item
        public Dictionary<int, Item> Items = new Dictionary<int, Item>();
        
        //创建的时候获取DB里的Item列表
        public ItemManager(Character owner)
        {
            this.Owner = owner;

            foreach(var item in Owner.Data.Items)
            {
                this.Items.Add(item.ItemID, new Item(item));
            }
        }

        public bool UseItem(int itemId, int count = 1)
        {
            Item item = null;
            if(this.Items.TryGetValue(itemId,out item))
            {
                if (item.Count < count)
                    return false;

                //TODO

                item.Remove(count);
                return true;
            }
            return false;
        }

        public bool HasItem(int itemId)
        {
            Item item = null;
            if (this.Items.TryGetValue(itemId, out item))
                return item.Count > 0;
            return false;
        }

      
        public Item GetItem(int itemId)
        {
            Item item = null;
            this.Items.TryGetValue(itemId, out item);
            Log.InfoFormat("{0} 获取道具 {1}  {2}个", this.Owner.Data.ID,item,itemId);
            return item;
        }

        public bool AddItem(int itemId , int count)
        {
            Item item = null;
            if(this.Items.TryGetValue(itemId,out item))
            {
                item.Add(count);
            }
            else
            {
                //创建一个ItemData数据 
                TCharacterItem dbItem = new TCharacterItem();
                dbItem.TCharacterID = Owner.Data.ID;
                dbItem.Owner = Owner.Data;
                dbItem.ItemID = itemId;
                dbItem.ItemCount = count;
                //给这个角色的Data添加进去
                Owner.Data.Items.Add(dbItem);
                //给这个管理类添加一份
                item = new Item(dbItem);
                this.Items.Add(itemId, item);

            }
            this.Owner.StatusManager.AddItemChange(itemId, count, StatusAction.Add);
            Log.InfoFormat("{0} 获取道具 {1}  {2}个", this.Owner.Data.ID, item, count);
            //DBService.Instance.Save();
            return true;
        }

        public bool RemoveItem(int ItemId, int count)
        {
            if (!this.Items.ContainsKey(ItemId))
            {
                return false;
            }
            Item item = this.Items[ItemId];
            if (item.Count < count)
                return false;
            item.Remove(count);
            //只需要记录到状态管理器 后面一起发送
            this.Owner.StatusManager.AddItemChange(ItemId, count, StatusAction.Delete);
            Log.InfoFormat("{0}移除道具{1}  {2}个", this.Owner.Data.ID, item, count);
            //DBService.Instance.Save();
            return true;
        }

        public void GetItemInfos(List<NItemInfo> list)
        {
            foreach(var item in this.Items)
            {
                list.Add(new NItemInfo() { Id = item.Value.ItemID, Count = item.Value.Count });
            }
        }
    }
}
