using Common;
using GameServer.Entities;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class BagService:Singleton<BagService>
    {
        public BagService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<BagSaveRequset>(this.OnBagSave);
        }

        public void Init()
        {

        }

        void OnBagSave(NetConnection<NetSession> sender,BagSaveRequset requset)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("保存背包请求：角色：{0} Unlocked{1}", character.Id, requset.BagInfo.Unlocked);

            if(requset.BagInfo != null)
            {
                character.Data.Bag.Items = requset.BagInfo.Items;
                DBService.Instance.Save();
            }

        }

    }
}
