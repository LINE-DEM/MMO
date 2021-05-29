using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GameServer;
using GameServer.Entities;
using GameServer.Network;
using GameServer.Services;
using SkillBridge.Message;

namespace Network
{
    class NetSession : INetSession
    {
        public TUser User { get; set; }
        public Character Character { get; set; }
        public NEntity Entity { get; set; }
        public IPostResponser PostResponser { get; set; }

        public void Disconnected()
        {
            this.PostResponser = null;
            if (this.Character != null)
                UserService.Instance.CharacterLeave(this.Character);
        }


        NetMessage response;

        public NetMessageResponse Response
        {
            get
            {
                if (response == null)
                {
                    response = new NetMessage();
                }
                if (response.Response == null)
                    response.Response = new NetMessageResponse();
                return response.Response;
            }
        }

        //每次Send的时候都会调用 所有后处理的东西都不需要多发送一次
        public byte[] GetResponse()
        {
            if (response != null)
            {
                if(response != null)
                {
                    //新的后处理器机制 唯一的调用地方！！！
                    if (PostResponser != null)
                        this.PostResponser.PostProcess(Response);
                }

                //以前的版本 用状态管理

                //if (this.Character != null && this.Character.StatusManager.HasStatus)
                //{
                //    this.Character.StatusManager.ApplyResponse(Response);
                //}
                byte[] data = PackageHandler.PackMessage(response);
                response = null;
                return data;
            }
            return null;
        }
    }
}
