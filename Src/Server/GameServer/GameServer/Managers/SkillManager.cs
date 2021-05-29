using GameServer.Entities;
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
    class SkillManager
    {
        Character Owner;

        public SkillManager(Character owner)
        {
            Owner = owner;
            GetInfoByDB();
        }

        public List<NSkillInfo> skills = new List<NSkillInfo>();

        //*********只用来给info赋值************
        public void GetSkillInfo(List<NSkillInfo> info)
        {
            foreach (var item in skills)
            {
                info.Add(item);
            }
        }


        //*************构建info缓存（ 从DB里）************
        void GetInfoByDB()
        {
            foreach (var item in this.Owner.Data.Skills)
            {
                NSkillInfo temp = new NSkillInfo();
                temp.Id = item.Id;
                temp.Unlocked = item.Unlocked;
                skills.Add(temp);
            }

        }


        public void SkillOpen(NetConnection<NetSession> sender,int id)
        {
            List<NSkillInfo> addTemps = new List<NSkillInfo>();
            foreach (var item in this.skills)
            {
                
                if (item.Id%8 == id)
                {
                    if(item.Unlocked == 1)
                    {
                        sender.Session.Response.skillAddRes = new SkillOpenResponse();
                        sender.Session.Response.skillAddRes.Result = Result.Failed;
                        
                       
                       // break;
                    }
                    else
                    {
                        sender.Session.Response.skillAddRes = new SkillOpenResponse();
                        sender.Session.Response.skillAddRes.Result = Result.Success;
                        item.Unlocked = 1;
                        sender.Session.Character.SkillManger.skills[item.Id%8-1].Unlocked = 1;
                        //item.Unlocked = 1;
                    }
                   
                   
                }
               
                addTemps.Add(item);
                //sender.Session.Response.skillAddRes.Skillinfos = new List<NSkillInfo>();
                
            }
            sender.Session.Response.skillAddRes.Skillinfos.AddRange(addTemps);
            sender.SendResponse();



            foreach (var item in this.Owner.Data.Skills)
            {
                if (item.Id%8 == id)
                {
                    item.Unlocked = 1;
                }
            }
            
            //Owner.Data.Gold = gold - DataManager.Instance.Skills[id].Cost;
            
            sender.Session.Character.Gold -= DataManager.Instance.Skills[id].Cost;
            DBService.Instance.Save();
        }

        //public bool RemoveFriendByFriendId(int friendid)
        //{
        //    var removeItem = this.Owner.Data.Friends.FirstOrDefault(v => v.FriendID == friendid);
        //    if (removeItem != null)
        //    {
        //        DBService.Instance.Entities.TCharacterFriends.Remove(removeItem);
        //    }
        //    friendChanged = true;
        //    return true;
        //}
    }
}
