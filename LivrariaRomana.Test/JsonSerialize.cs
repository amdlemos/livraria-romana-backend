using System;
using System.Collections.Generic;
using System.Text;
using Utf8Json;

namespace LivrariaRomana.Test
{
    public class JsonSerialize
    {
        public static string Serialize(object obj)
        {
            byte[] result = JsonSerializer.Serialize(obj);
            var p2 = JsonSerializer.Deserialize<object>(result);
            var json = JsonSerializer.ToJsonString(p2);
            return json;
        }
    }
}
