using Common;
using Common.Data;
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
    class QuestManager 
    {
        Character Owner;

        public QuestManager(Character owner)
        {
            this.Owner = owner;
        }

        //从数据库到 网络Quest列表
        public void GetQuestInfos(List<NQuestInfo> list)
        {
            foreach (var quest in this.Owner.Data.Quests)
            {
                list.Add(GetQuestInfo(quest));
            }
        }

        //用数据库 构造网络信息
        public NQuestInfo GetQuestInfo(TCharacterQuest quest)
        {
            return new NQuestInfo()
            {
                QuestId = quest.QuestID,
                QuestGuid = quest.Id,
                Status = (QuestStatus)quest.Status,
                Targets = new int[3]
                {
                    quest.Target1,
                    quest.Target2,
                    quest.Target3,
                }

            };
        }

        public Result AcceptQuest(NetConnection<NetSession> sender , int questId)
        {
            Character character = sender.Session.Character;

            QuestDefine quest;
            if(DataManager.Instance.Quests.TryGetValue(questId , out quest))
            {
                //和new是一样的
                var dbquest = DBService.Instance.Entities.CharacterQuests.Create();
                dbquest.QuestID = quest.ID;
                if(quest.Target1 == QuestTarget.None)
                {
                    //没有目标 直接完成
                    dbquest.Status = (int)QuestStatus.Complated;
                }
                else
                {
                    //有目标的
                    dbquest.Status = (int)QuestStatus.InProgress;
                }
                sender.Session.Response.questAccept.Quest = this.GetQuestInfo(dbquest);
                character.Data.Quests.Add(dbquest);
                DBService.Instance.Save();
                return Result.Success;
            }
            else
            {
                sender.Session.Response.questAccept.Errormsg = "任务不存在";
                return Result.Failed;
            }
        }

        public Result SubmitQuest(NetConnection<NetSession> sender , int questId)
        {
            Character character = sender.Session.Character;

            QuestDefine quest;
            if(DataManager.Instance.Quests.TryGetValue(questId,out quest))
            {
                var dbquest = character.Data.Quests.Where(q => q.QuestID == questId).FirstOrDefault();
                if(dbquest != null)
                {
                    if(dbquest.Status != (int)QuestStatus.Complated)
                    {
                        sender.Session.Response.questSubmit.Errormsg = "任务未完成";
                        return Result.Failed;
                    }
                    dbquest.Status = (int)QuestStatus.Finished;
                    sender.Session.Response.questSubmit.Quest = this.GetQuestInfo(dbquest);
                    //DBService.Instance.Save();

                    //处理奖励
                    if(quest.RewardGold > 0)
                    {
                        character.Gold += quest.RewardGold;
                    }
                    if(quest.RewardExp > 0)
                    {
                        //character.Exp += quest.RewardExp;
                    }
                    if(quest.RewardItem1 > 0)
                    {
                        character.ItemManager.AddItem(quest.RewardItem1, quest.RewardItem1Count);
                    }
                    if(quest.RewardItem2 > 0)
                    {
                        character.ItemManager.AddItem(quest.RewardItem2, quest.RewardItem2Count);
                    }
                    if(quest.RewardItem3 > 0)
                    {
                        character.ItemManager.AddItem(quest.RewardItem3, quest.RewardItem3Count);
                    }

                    DBService.Instance.Save();
                    return Result.Success;
                }
                sender.Session.Response.questSubmit.Errormsg = "任务不存在[2]";
                return Result.Failed;
            }
            else
            {
                sender.Session.Response.questSubmit.Errormsg = "任务不存在[1]";
                return Result.Failed;
            }
        }
    }
}
