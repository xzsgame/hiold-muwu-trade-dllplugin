﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hide_and_seek.common
{
    public class Injections
    {
        /// <summary>
        /// Block类下OnBlockDamaged方法
        /// </summary>
        /// <param name="_world"></param>
        /// <param name="_clrIdx"></param>
        /// <param name="_blockPos"></param>
        /// <param name="_blockValue"></param>
        /// <param name="_damagePoints"></param>
        /// <param name="_entityIdThatDamaged"></param>
        /// <param name="_bUseHarvestTool"></param>
        /// <param name="_bBypassMaxDamage"></param>
        /// <param name="_recDepth"></param>
        /// <returns></returns>
        public static bool ChangeBlocks(GameManager __instance, PlatformUserIdentifierAbs persistentPlayerId, List<BlockChangeInfo> _blocksToChange)
        {
            //判断打击的是否为玩家伪装的方块
            int i = 0;
            while (i < _blocksToChange.Count)
            {
                BlockChangeInfo blockChangeInfo = _blocksToChange[i];

                foreach (KeyValuePair<int, Vector3i> tempdt in MainController.HidersPos)
                {
                    if (MainController.HidersPos.Values.Contains(blockChangeInfo.pos))
                    {
                        //找到了躲藏者
                        //移除躲藏者数据
                        MainController.Hiders.Remove(tempdt.Key);
                        MainController.HidersPos.Remove(tempdt.Key);
                        //给躲藏者发送消息
                        ClientInfo _cinfoHider = UserTools.GetClientInfoFromEntityId(tempdt.Key);
                        _cinfoHider.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(EChatType.Whisper, -1, "你被找到了!", "[87CEFA]躲猫猫", false, null));

                        //获取寻找者ID
                        int pid = UserTools.GetEntityPlatformUserIdentifierAbs(persistentPlayerId);
                        ClientInfo _cinfoSeeker = UserTools.GetClientInfoFromEntityId(pid);
                        _cinfoSeeker.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(EChatType.Whisper, -1, "你找到一名躲藏者!", "[87CEFA]躲猫猫", false, null));
                        return false;
                    }
                }

            }
            //放行
            return true;
        }



        public static bool ProcessPackage_fix(NetPackageEntityRelPosAndRot __instance, World _world, GameManager _callbacks)
        {
            int pid = Traverse.Create(__instance).Field("entityId").GetValue<int>();
            if (MainController.Hiders.Contains(pid))
            {
                Entity entity = _world.GetEntity(pid);
                if (entity == null)
                {
                    return true;
                }
                Entity attachedMainEntity = entity.AttachedMainEntity;
                if (attachedMainEntity != null && _world.GetPrimaryPlayerId() == attachedMainEntity.entityId)
                {
                    return true;
                }
                Vector3i dPos = Traverse.Create(__instance).Field("dPos").GetValue<Vector3i>();
                Vector3i newPos = entity.serverPos + dPos;
                Vector3i oldPos = MainController.HidersPos[pid];
                //如果位置发生变化，更新目标位置 burntWoodRoof
                if (newPos != oldPos)
                {
                    BlockTools.RemoveBlock(oldPos);
                    BlockTools.SetBlock(newPos, "burntWoodRoof");
                }
                //阻止玩家移动
                return false;
            }
            //放行
            return true;
        }



    }
}
