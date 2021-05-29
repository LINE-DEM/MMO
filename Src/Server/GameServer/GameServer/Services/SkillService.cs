using Common;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class SkillService : Singleton<SkillService>
    {
        public SkillService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<SkillOpenRequest>(OnSkillOpen);
        }

        public void Init()
        {

        }

        private void OnSkillOpen(NetConnection<NetSession> sender, SkillOpenRequest message)
        {
            var character = sender.Session.Character;
           
            character.SkillManger.SkillOpen(  sender,message.Id);

            //sender.Session.Response.skillAddRes = new SkillOpenResponse();
            //sender.Session.Response.skillAddRes.Result = Result.Success;
            //sender.SendResponse();
        }
    }
}
