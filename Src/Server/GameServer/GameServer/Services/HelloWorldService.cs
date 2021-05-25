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
    class HelloWorldService:Singleton<HelloWorldService>
    {
        public void Init()
        {
            
        }

        public void Start()
        {
            MessageDistributer<NetConnection<NetSession>>
                .Instance.Subscribe<FirstTestRequest>(this.OnFirstTestRequset);
        }


        void OnFirstTestRequset(NetConnection<NetSession> sender, FirstTestRequest request)
        {
            Log.InfoFormat("FirstTestRequest: {0}", request.Helloword);
        }

        public void Stop()
        {

        }
    }
}
