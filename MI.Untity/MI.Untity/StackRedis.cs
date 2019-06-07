using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MI.Untity
{
    public class StackRedis : IDisposable
    {
        #region 配置属性   基于 StackExchange.Redis 封装
        //连接串 （注：IP:端口,属性=,属性=)
        //public string _ConnectionString = "47.99.92.76:6379,password=shenniubuxing3";
        public string _ConnectionString = "47.99.92.76:6379";
        //操作的库（注：默认0库）
        public int _Db = 0;
        #endregion

        #region 管理器对象

        /// <summary>
        /// 获取redis操作类对象
        /// </summary>
        private static StackRedis _StackRedis;
        private static object _locker_StackRedis = new object();
        public static StackRedis Current
        {
            get
            {
                if (_StackRedis == null)
                {
                    lock (_locker_StackRedis)
                    {
                        _StackRedis = _StackRedis ?? new StackRedis();
                        return _StackRedis;
                    }
                }

                return _StackRedis;
            }
        }

        /// <summary>
        /// 获取并发链接管理器对象
        /// </summary>
        private static ConnectionMultiplexer _redis;
        private static object _locker = new object();
        public ConnectionMultiplexer Manager
        {
            get
            {
                if (_redis == null)
                {
                    lock (_locker)
                    {

                        _redis = _redis ?? GetManager(_ConnectionString);
                        return _redis;
                    }
                }

                return _redis;
            }
        }

        /// <summary>
        /// 获取链接管理器
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public ConnectionMultiplexer GetManager(string connectionString)
        {
            return ConnectionMultiplexer.Connect(connectionString);
        }

        /// <summary>
        /// 获取操作数据库对象
        /// </summary>
        /// <returns></returns>
        public IDatabase GetDb()
        {
            return Manager.GetDatabase(_Db);
        }
        #endregion

        #region 操作方法

        #region string 操作

        /// <summary>
        /// 根据Key移除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> Remove(string key)
        {
            var db = this.GetDb();

            return await db.KeyDeleteAsync(key);
        }

        /// <summary>
        /// 根据key获取string结果
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<string> Get(string key)
        {
            var db = this.GetDb();
            return await db.StringGetAsync(key);
        }

        /// <summary>
        /// 根据key获取string中的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> Get<T>(string key)
        {
            var t = default(T);
            try
            {
                var _str = await this.Get(key);
                if (string.IsNullOrWhiteSpace(_str)) { return t; }

                t = JsonConvert.DeserializeObject<T>(_str);
            }
            catch (Exception ex) { }
            return t;
        }

        /// <summary>
        /// 存储string数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireMinutes"></param>
        /// <returns></returns>
        public async Task<bool> Set(string key, string value, int expireMinutes = 0)
        {
            var db = this.GetDb();
            if (expireMinutes > 0)
            {
                return db.StringSet(key, value, TimeSpan.FromMinutes(expireMinutes));
            }
            return await db.StringSetAsync(key, value);
        }

        /// <summary>
        /// 存储对象数据到string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireMinutes"></param>
        /// <returns></returns>
        public async Task<bool> Set<T>(string key, T value, int expireMinutes = 0)
        {
            try
            {
                var jsonOption = new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                var _str = JsonConvert.SerializeObject(value, jsonOption);
                if (string.IsNullOrWhiteSpace(_str)) { return false; }

                return await this.Set(key, _str, expireMinutes);
            }
            catch (Exception ex) { }
            return false;
        }
        #endregion

        #region List操作（注：可以当做队列使用）

        /// <summary>
        /// list长度
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<long> GetListLen<T>(string key)
        {
            try
            {
                var db = this.GetDb();
                return await db.ListLengthAsync(key);
            }
            catch (Exception ex) { }
            return 0;
        }

        /// <summary>
        /// 获取队列出口数据并移除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> GetListAndPop<T>(string key)
        {
            var t = default(T);
            try
            {
                var db = this.GetDb();
                var _str = await db.ListRightPopAsync(key);
                if (string.IsNullOrWhiteSpace(_str)) { return t; }
                t = JsonConvert.DeserializeObject<T>(_str);
            }
            catch (Exception ex) { }
            return t;
        }

        /// <summary>
        /// 集合对象添加到list左边
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public async Task<long> SetLists<T>(string key, List<T> values)
        {
            var result = 0L;
            try
            {
                var jsonOption = new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                var db = this.GetDb();
                foreach (var item in values)
                {
                    var _str = JsonConvert.SerializeObject(item, jsonOption);
                    result += await db.ListLeftPushAsync(key, _str);
                }
                return result;
            }
            catch (Exception ex) { }
            return result;
        }

        /// <summary>
        /// 单个对象添加到list左边
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<long> SetList<T>(string key, T value)
        {
            var result = 0L;
            try
            {
                result = await this.SetLists(key, new List<T> { value });
            }
            catch (Exception ex) { }
            return result;
        }

        /// <summary>
        /// 获取List所有数据
        /// </summary>
        public async Task<List<string>> GetAllList(string list)
        {
            var db = this.GetDb();
            var redisList = await db.ListRangeAsync(list);
            List<string> listMembers = new List<string>();
            foreach (var item in redisList)
            {
                listMembers.Add(JsonConvert.DeserializeObject<string>(item));
            }
            return listMembers;
        }


        #endregion

        #region 额外扩展

        /// <summary>
        /// 手动回收管理器对象
        /// </summary>
        public void Dispose()
        {
            this.Dispose(_redis);
        }

        public void Dispose(ConnectionMultiplexer con)
        {
            if (con != null)
            {
                con.Close();
                con.Dispose();
            }
        }

        #endregion

        #endregion
    }
}
