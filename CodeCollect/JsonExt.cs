using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace CodeCollect
{
    public static class JsonExt
    {
        public static string ToJson<T>(this object obj)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            return ser.Serialize(obj);
        }

        public static T ToObj<T>(this string jsonStr)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            return ser.Deserialize<T>(jsonStr);
        }
    }
}
