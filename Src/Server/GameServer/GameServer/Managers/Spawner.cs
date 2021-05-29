using Common;
using Common.Data;
using GameServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{/// <summary>
/// 刷怪器   一个刷怪规则对应一个刷怪点
/// </summary>
    class Spawner
    {
        public SpawnRuleDefine Define { get; set; }

        private Map Map;

        private float spawnTime = 0;

        private float unspawnTime = 0;

        private bool spawned = false;

        private SpawnPointDefine spawnPoint = null;

        public Spawner(SpawnRuleDefine define, Map map)
        {
            this.Define = define;
            this.Map = map;

            if (DataManager.Instance.SpawnPoints.ContainsKey(this.Map.ID))
            {
                if (DataManager.Instance.SpawnPoints[this.Map.ID].ContainsKey(this.Define.SpawnPoint))
                {
                    spawnPoint = DataManager.Instance.SpawnPoints[this.Map.ID][this.Define.SpawnPoint];
                }
                else
                    Log.ErrorFormat("刷怪规则【{0}】  刷怪点【{1}】不存在！！！", this.Define.ID, this.Define.SpawnPoints);
            }
        }

        public void Update()
        {
            if (this.CanSpawn())
            {
                this.Spawn();
            }
        }

        bool CanSpawn()
        {
            if (this.spawned)
                return false;
            //怪物被杀死的时间
            if (this.unspawnTime + this.Define.SpawnPeriod > Time.time)
                return false;

            return true;
        }

        public void Spawn()
        {
            this.spawned = true;
            Log.InfoFormat("地图【{0}】 刷怪【{1}】 怪物【{2}】 等级【{3}】 在的坐标【{4}】", this.Define.MapID, this.Define.ID, this.Define.SpawnMonID, this.Define.SpawnLevel, this.Define.SpawnPoint);
            if (this.Define.MapID == 3)
                return;
            this.Map.MonsterManager.Create(this.Define.SpawnMonID, this.Define.SpawnLevel, this.spawnPoint.Position, this.spawnPoint.Direction);
        }
    }
}
