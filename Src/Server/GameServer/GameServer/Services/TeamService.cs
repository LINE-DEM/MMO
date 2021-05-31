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
    class TeamService:Singleton<TeamService>
    {

        public TeamService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamInviteRequest>(this.OnTeamInviteRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamInviteResponse>(this.OnTeamInviteResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamLeaveRequest>(this.OnTeamLeaveRequest);
        }


        public void Init()
        {
            TeamManager.Instance.Init();
        }

        void OnTeamInviteRequest(NetConnection<NetSession> sender,TeamInviteRequest request)
        {
            Character character = sender.Session.Character;



            NetConnection<NetSession> target = SessionManager.Instance.GetSession(request.ToId);
            if(target == null)
            {
                sender.Session.Response.teamInviteRes = new TeamInviteResponse();
                sender.Session.Response.teamInviteRes.Result = Result.Failed;
                sender.Session.Response.teamInviteRes.Errormsg = "好友不在线";
                sender.SendResponse();
                return;
            }
            if(target.Session.Character.Team != null)
            {
                sender.Session.Response.teamInviteRes = new TeamInviteResponse();
                sender.Session.Response.teamInviteRes.Result = Result.Failed;
                sender.Session.Response.teamInviteRes.Errormsg = "对方已经有队伍了";
                sender.SendResponse();
                return;
            }

            //消息的转发
            target.Session.Response.teamInviteReq = request;
            target.SendResponse();


        }

        void OnTeamInviteResponse(NetConnection<NetSession> sender,TeamInviteResponse response)
        {
            Character characcter = sender.Session.Character;

            sender.Session.Response.teamInviteRes = response;
            if(response.Result == Result.Success)
            {
                var requester = SessionManager.Instance.GetSession(response.Request.FromId);
                if (requester == null)
                {
                    sender.Session.Response.teamInviteRes.Result = Result.Failed;
                    sender.Session.Response.teamInviteRes.Errormsg = "请求者已经下线了";
                }
                else
                {
                    TeamManager.Instance.AddTeamMember(requester.Session.Character, characcter);
                    requester.Session.Response.teamInviteRes = response;
                    requester.SendResponse();
                }
            }
            sender.SendResponse();
        }

        void OnTeamLeaveRequest(NetConnection<NetSession> sender, TeamLeaveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("队伍离开");
            sender.Session.Response.teamLeave = new TeamLeaveResponse();
            sender.Session.Response.teamLeave.characterId = character.Id;
            //sender.Session.Response.teamLeave.Restult == Result.Success;
            sender.SendResponse();
        }
    }
}
