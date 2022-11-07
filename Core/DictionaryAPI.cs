using MFDictionary.MVVM.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MFDictionary.Core
{
    internal class DictionaryAPI
    {
        private static readonly string key = "dict.1.1.20221106T112155Z.dbaab137810c5440.98aa89155ad9b46e203069e3dccf6834074af6e8";
        private static readonly string endpoint = "https://dictionary.yandex.net/api/v1/dicservice.json";

        public DictionaryAPI()
        {

        }

        public async Task<List<string>> GetLangs()
        {
            string route = String.Format("/getLangs?key={0}", key);
            string result;

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Get;
                request.RequestUri = new Uri(endpoint + route);

                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                result = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<List<string>>(result);
        }
        public async Task<Object> Request(string word)
        { 
            string route = String.Format("/lookup?key={0}&lang=ru-en&text={1}", key, word);
            string result;

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Get;
                request.RequestUri = new Uri(endpoint + route);

                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                result = await response.Content.ReadAsStringAsync();   
            }

            return JsonConvert.DeserializeObject(result);
        }
    }
}
