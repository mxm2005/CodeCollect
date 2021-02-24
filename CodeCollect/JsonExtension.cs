using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;
using System.IO;

namespace CodeCollect
{
    /// <summary> Object to json 的扩展类</summary>
    public static class JsonExtension
    {
        /// <summary>
        /// 把对象转换成Json字符串
        /// </summary>
        /// <param name="obj">数据对象</param>
        /// <param name="isCamel">数据对象属性的Json首字母是否小写</param>
        /// <param name="isIsoDate">是否使用iso日期格式</param>
        /// <returns>Json字符串</returns>
        public static string ToJson(this object obj, bool isCamel, bool isIsoDate)
        {
            var converters = isIsoDate ? new List<JsonConverter> { new IsoDateTimeConverter() } : null;

            JsonSerializerSettings serializerSettings = null;
            if (isCamel)
            {
                serializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    // 循环引用
                    //ReferenceLoopHandling = ReferenceLoopHandling.Ignore 
                };

                if (converters != null)
                {
                    serializerSettings.Converters = converters;
                }
            }
            //JsonConvert.DeserializeObject(
            return JsonConvert.SerializeObject(obj, Formatting.Indented, serializerSettings);         
        }

        /// <summary>
        /// 把对象转换成Json字符串
        /// </summary>
        /// <param name="obj">数据对象</param>
        /// <param name="isCamel">数据对象属性的Json首字母是否小写</param>
        /// <returns>Json字符串</returns>
        public static string ToJson(this object obj, bool isCamel = true)
        {
            return obj.ToJson(isCamel, true);
        }

        public static T DeserializeObject<T>(string jsonString, bool isCamel = true, bool isIsoDate = true)
        {
            var converters = isIsoDate ? new List<JsonConverter> { new IsoDateTimeConverter() } : null;

            JsonSerializerSettings serializerSettings = null;
            if (isCamel)
            {
                serializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    // 循环引用
                    //ReferenceLoopHandling = ReferenceLoopHandling.Ignore 
                };

                if (converters != null)
                {
                    serializerSettings.Converters = converters;
                }
            }
            //JsonConvert.DeserializeObject(
            return JsonConvert.DeserializeObject<T>(obj, serializerSettings);
        }
    }
}
