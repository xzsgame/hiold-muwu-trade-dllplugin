﻿using HioldMod.HttpServer;
using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.database;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.service
{
    class UserStorageService
    {
        /// <summary>
        /// 向数据库中插入玩家库存数据
        /// </summary>
        /// <param name="item">物品</param>
        public static void addUserStorage(UserStorage storage)
        {
            DataBase.db.Insertable<UserStorage>(storage).ExecuteCommand();
        }


        /// <summary>
        /// 根据id获取库存数据
        /// </summary>
        /// <param name="playerid">用户id</param>
        /// <returns></returns>
        public static UserStorage selectUserStorageByid(string id)
        {
            return DataBase.db.Queryable<UserStorage>().Where(string.Format("id = '{0}' ", id)).First();
        }


        /// <summary>
        /// 根据用户id获取优惠券
        /// </summary>
        /// <param name="playerid">用户id</param>
        /// <returns></returns>
        public static List<UserStorage> selectPlayersCou(string playerid)
        {
            return DataBase.db.Queryable<UserStorage>().Where(string.Format("gameentityid = '{0}' and storageCount > 0 and itemStatus='1' and itemtype='2' ", playerid)).ToList();
        }


        /// <summary>
        /// 查询可供货物品
        /// </summary>
        /// <param name="playerid">用户id</param>
        /// <returns></returns>
        public static UserStorage selectSupplyableItem(string playerid, string itemname, string quality, string count)
        {
            return DataBase.db.Queryable<UserStorage>().Where(string.Format("gameentityid = '{0}' and storageCount >= {1} and itemStatus='1' and itemtype='1' and name='{2}' and cast(quality as int)>={3}", playerid, count, itemname, quality)).First();
        }

        /// <summary>
        /// 根据用户id获取优惠券
        /// </summary>
        /// <param name="playerid">用户id</param>
        /// <returns></returns>
        public static UserStorage selectAvaliableItem(string playerid, string itemname, string quality, string itemtype, string count)
        {
            return DataBase.db.Queryable<UserStorage>().Where(string.Format("gameentityid = '{0}' and storageCount >= {1} and itemStatus='1' and itemtype='{2}' and name='{3}' and cast(quality as int)>={4}", playerid, count, itemtype, itemname, quality)).First();
        }

        /// <summary>
        /// 根据用户id获取玩家库存
        /// </summary>
        /// <param name="playerid">用户id</param>
        /// <param name="itemtype">物品类型</param>
        /// <returns></returns>
        public static Dictionary<string, object> selectPlayersStorage(string playerid, string itemname, int pageIndex, int pageSize, string mainType, string gruopsStrs)
        {
            int totalCount = 0;
            string groupStr = "";
            string mainTypeStr = "";
            string sortStr = "";
            //包含group信息
            if (gruopsStrs != null && gruopsStrs.Length > 0)
            {
                groupStr += " and (";
                string[] groups = gruopsStrs.Split('/');
                for (int i = 0; i < groups.Length; i++)
                {
                    if (i == 0)
                    {
                        groupStr += string.Format(" class1 like '%{0}%' or class2 like '%{0}%'", groups[i]);
                    }
                    else
                    {
                        groupStr += string.Format(" or class1 like '%{0}%' or class2 like '%{0}%'", groups[i]);
                    }
                }
                groupStr += ") ";
            }
            //包含大分类信息
            if (mainType != null && mainType.Length > 0)
            {
                if (mainType.Equals("普通物品"))
                {
                    mainTypeStr += " and itemtype='1' ";
                }
                if (mainType.Equals("特殊物品"))
                {
                    mainTypeStr += " and itemtype='2' ";
                }
                if (mainType.Equals("积分商城"))
                {
                    mainTypeStr += " and currency='1' ";
                }
                if (mainType.Equals("钻石商城"))
                {
                    mainTypeStr += " and currency='2' ";
                }
            }

            List<UserStorage> ls = DataBase.db.Queryable<UserStorage>().Where(string.Format("gameentityid = '{0}' and storageCount > 0 and (name like '%{1}%' or translate like '%{1}%') and deleteTime ='0001-01-01 00:00:00' and itemStatus='1' " + groupStr + mainTypeStr, playerid, itemname)).ToPageList(pageIndex, pageSize, ref totalCount);
            Dictionary<string, object> result = new Dictionary<string, object>();
            result.Add("data", ls);
            result.Add("count", totalCount);
            return result;
        }




        /// <summary>
        /// 根据用户id获取物品库存
        /// </summary>
        /// <param name="id">物品id</param>
        /// <returns></returns>
        public static List<UserStorage> selectPlayerStorage(string id)
        {
            return DataBase.db.Queryable<UserStorage>().Where(string.Format("id = '{0}' ", id)).ToList();
        }

        /// <summary>
        /// 更新物品信息
        /// </summary>
        /// <param name="item">物品</param>
        public static void UpdateUserStorage(UserStorage storage)
        {
            DataBase.db.Updateable<UserStorage>(storage).ExecuteCommand();
        }
    }
}
