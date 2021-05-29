﻿using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    
    class MinimapManager : Singleton<MinimapManager>
    {
        public UIMinimap minimap;

        private Collider minimapBoundingBox;
        public Collider MinimapBoundingBox
        {
            get { return minimapBoundingBox; }
        }
        public Transform PlayerTransform
        {
            get
            {
                if (User.Instance.CurrentCharacterObject == null)
                    return null;
                return User.Instance.CurrentCharacterObject.transform;
            }
        }

        public Sprite LoadCurrentMinimap()
        {    
            return Resloader.Load<Sprite>("UI/Minimap/" + User.Instance.CurrentMapData.MiniMap);
        }
        //别人告诉管理器要更新小地图了 然后管理器才告诉小地图要更新
        public void UpdateMinimap(Collider minimapBoundingBox)
        {
            this.minimapBoundingBox = minimapBoundingBox;
            if (this.minimap != null)
            {
                
                this.minimap.UpdateMap();
            }
                
        }
    }
}
