using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Core
{
    public static class JsonExtensions
    {
        //SAMPLES
        //----------
        //var o = o.SelectToken("template")?.ToString(); //path
        //var o = o.SelectToken("emp.name")?.ToString(); //nested path
        //var o = o.SelectToken("emp").ToObject<dynamic>().name; //object access
        //var o = o.SelectToken("dept").ToObject<string[]>(); //array of strings
        //var o = o.SelectToken("employees[1].name")?.ToString(); //nested path using array syntax
        //var oCollection = o.SelectToken("employees")?.ToObject<List<dynamic>>(); //array of objects
        //foreach (var o in oCollection)
        //{
        //    var n = o.name;
        //}

        public static bool HasPath(dynamic o, string path)
        {
            var v = GetObjectFromPath<dynamic>(o, path);
            return v != null;
        }

        public static bool PathExists(this JObject obj, string path)
        {
            var tokens = obj.SelectTokens(path);
            return tokens.Any();
        }

        public static bool IsArray(dynamic o)
        {
            return o.Type == JTokenType.Array;
        }

        public static T GetValueFromPath<T>(dynamic o, string path)
        {
            T result = default(T);
            if (o != null && o is JObject)
            {
                var v = o.SelectToken(path)?.ToString();
                if (v != null) result = (T)Convert.ChangeType(v, typeof(T));
            }
            return result;
        }

        public static T GetValueFromPath<T>(dynamic o, string path, T defaultValue)
        {
            T result = GetValueFromPath<T>(o, path);
            if (typeof(T) == typeof(string) &&
                string.IsNullOrEmpty(result as string))
            {
                result = defaultValue;
            }
            else if (EqualityComparer<T>.Default.Equals(result, default(T)) &&
                     !EqualityComparer<T>.Default.Equals(result, defaultValue))
            {
                result = defaultValue;
            }

            return result;
        }

        public static T GetObjectFromPath<T>(dynamic o, string path)
        {
            T result = default(T);
            if (o != null && o is JObject)
            {
                var v = o.SelectToken(path)?.ToObject(typeof(T));
                result = v;
            }
            return result;
        }

        public static dynamic GetDynObjFromPath(dynamic o, string path)
        {
            return GetObjectFromPath<dynamic>(o, path);
        }

        public static JToken ReplacePath<T>(this JToken root, string path, T newValue)
        {
            if (root == null || path == null)
                throw new ArgumentNullException();

            foreach (var value in root.SelectTokens(path).ToList())
            {
                if (value == root)
                    root = JToken.FromObject(newValue);
                else
                    value.Replace(JToken.FromObject(newValue));
            }

            return root;
        }

        public static string ReplacePath<T>(string jsonString, string path, T newValue)
        {
            return JToken.Parse(jsonString).ReplacePath(path, newValue).ToString();
        }

        public static JObject ToJObject(this String str)
        {
            return (JObject)JToken.FromObject(str.ToDynamicFromJsonString()); //TODO: better way?
        }

        public static JObject ToJObject(dynamic o)
        {
            return (JObject)JToken.FromObject(o); //TODO: better way?
        }

        public static JArray ToJArray(this String str)
        {
            return (JArray)JToken.FromObject(str.ToDynamicFromJsonString()); //TODO: better way?
        }

        public static dynamic ToDynamicFromJsonString(this String str)
        {
            return JsonConvert.DeserializeObject(str);
        }
        public static T ToTypeFromJsonString<T>(this String str)
        {
            return JsonConvert.DeserializeObject<T>(str);
        }

    }
}
