using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Utf8Json;

namespace LivrariaRomana.API.Test
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

        public static StringContent GenerateStringContent(object book)
        {
            var jsonSerialized = JsonSerialize.Serialize(book);
            var contentString = new StringContent(jsonSerialized, Encoding.UTF8, "application/json");
            contentString.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return contentString;
        }
    }
}
