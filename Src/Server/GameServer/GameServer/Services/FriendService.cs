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
    class FriendService:Singleton<FriendService>
    {
        //List<FriendAddRequest> friendRequests = new List<FriendAddRequest>();

        public FriendService()
        {
            //当消息来的时候 如果发送者的Session.Request里面有这个请求就调用
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendAddRequest>(this.OnFriendAddRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendAddResponse>(this.OnFriendAddRespone);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendRemoveRequest>(this.OnFriendRemove);
        }

        public void Init()
        {

        }

        //A的添加请求 --> 发到服务端 --> 根据客户端构建的request --> 用Mananger处理下数据 --> 构建出Respone --> Send给客户端 
        void OnFriendAddRequest(NetConnection<NetSession> sender , FriendAddRequest request)
        {
            //取到发送者A
            Character character = sender.Session.Character;

            //客户端构建消息的时候如果是输入的名字 就 根据名字取到Toid
            if(request.ToId == 0)
            {
                //在线玩家
                foreach (var cha in CharacterManager.Instance.Characters)
                {
                    if(cha.Value.Data.Name == request.ToName)
                    {
                        request.ToId = cha.Key;
                        break;
                    }
                }
            }

            //被添加者B的session
            NetConnection<NetSession> friend = null;

            // 给B的Session赋值！！！(根据 TOID找到被添加者B)
            if(request.ToId > 0)
            {
                if(character.FriendManager.CheekFriendByID( request.ToId))
                  //  (friend.Session.Character.Data.Friends.FirstOrDefault(v => v.FriendID == request.ToId)) != null)
                {
                    sender.Session.Response.friendAddRes = new FriendAddResponse();
                    sender.Session.Response.friendAddRes.Result = Result.Failed;
                    sender.Session.Response.friendAddRes.Errormsg = "已经是好友了";
                    sender.SendResponse();
                    return;
                }
                //看看有没有这个friend  一定是要DBid 在数据库中都是唯一的
                friend = SessionManager.Instance.GetSession(request.ToId);
            }
            //如果在上面赋值的时候刚好掉线 没找到这个 被添加者B 就返回失败
            if(friend == null)
            {
                sender.Session.Response.friendAddRes = new FriendAddResponse();
                sender.Session.Response.friendAddRes.Result = Result.Failed;
                sender.Session.Response.friendAddRes.Errormsg = "好友不存在或者不在线";
                sender.SendResponse();
                return;
            }

           //给想加的好友 发送响应  客户端接收响应后 构建一个请求 里面只有是否同意
            friend.Session.Response.friendAddReq = request;
            friend.SendResponse();
        }

        void OnFriendAddRespone(NetConnection<NetSession> sender,FriendAddResponse response)
        {
            Character character = sender.Session.Character;

            sender.Session.Response.friendAddRes = response;
            //客户端接收响应后 构建一个请求 里面只有是否同意
            if (response.Result == Result.Success)
            {
                var requester = SessionManager.Instance.GetSession(response.Request.FromId);
                //如果发送者A已经下线了
                if(requester == null)
                {
                    sender.Session.Response.friendAddRes.Result = Result.Failed;
                    sender.Session.Response.friendAddRes.Errormsg = "请求者已下线";
                }
                else
                {
                    //A B 互加好友
                    character.FriendManager.AddFriend(requester.Session.Character);
                    requester.Session.Character.FriendManager.AddFriend(character);

                    DBService.Instance.Save();
                    requester.Session.Response.friendAddRes = response;
                    requester.Session.Response.friendAddRes.Result = Result.Success;
                    requester.Session.Response.friendAddRes.Errormsg = "添加好友成功";
                    requester.SendResponse();
                }
            }


            //sender.Session.Response.friendAddRes.Result = Result.Success;
            //sender.Session.Response.friendAddRes.Errormsg = response.Request.FromName + "成为您的好友";
            sender.SendResponse();
        }


        void OnFriendRemove (NetConnection<NetSession> sender,FriendRemoveRequest request)
        {
            Character character = sender.Session.Character;
            sender.Session.Response.friendRemove = new FriendRemoveResponse();
            sender.Session.Response.friendRemove.Id = request.Id;

            if (character.FriendManager.RemoveFriendByID(request.Id))
            {
                sender.Session.Response.friendRemove.Result = Result.Success;

                var friend = SessionManager.Instance.GetSession(request.friendId);
                if(friend != null)
                {
                    friend.Session.Character.FriendManager.RemoveFriendByID(character.Id);
                }
                else
                {
                    this.RemoveFriend(request.friendId, character.Id);
                }
            }
            else
            {
                sender.Session.Response.friendRemove.Result = Result.Failed;
            }

            DBService.Instance.Save();
            sender.SendResponse();
        }
        void RemoveFriend(int charId,int friendId)
        {
            var removeItem = DBService.Instance.Entities.TCharacterFriends.FirstOrDefault(v => v.CharacterID == charId && v.FriendID == friendId);
            if(removeItem != null)
            {
                DBService.Instance.Entities.TCharacterFriends.Remove(removeItem);
            }
        }
    }
}
