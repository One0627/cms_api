using System;
using System.Collections.Generic;
using StackExchange.Redis;

namespace CMS_Infrastructure.Redis
{
    public interface IRedisCommon
    {
        RedisValue[] GetStringKey(List<RedisKey> listKey);
        RedisValue GetStringKey(string key);
        T GetStringKey<T>(string key);
        bool HashDelete(RedisKey key, RedisValue hashField);
        bool HashExists(RedisKey key, RedisValue hashField);
        string HashGet(RedisKey key, RedisValue hashField);
        bool HashRemove(string key, string dataKey);
        bool HashSet(RedisKey key, RedisValue hashField, RedisValue value);
        long KeyDelete(RedisKey[] keys);
        bool KeyDelete(string key);
        bool KeyExists(string key);
        bool KeyRename(string key, string newKey);
        void SetExpire(string key, TimeSpan time);
        bool SetStringKey<T>(string key, T obj, TimeSpan? expiry = null);
        void StringAppend(string key, string value);
        string StringGet(string key);
        string[] StringGetMany(string[] keyStrs);
        bool StringSet(KeyValuePair<RedisKey, RedisValue>[] arr);
        bool StringSet(string key, string value);
        bool StringSet(string key, string value, TimeSpan? expiry = null);
        bool StringSetMany(string[] keysStr, string[] valuesStr);
    }
}