using MFDictionary.MVVM.Model;
using MFDictionary.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MFDictionary.Core
{
    public class YandexService
    {
        private const string key = "dict.1.1.20221106T112155Z.dbaab137810c5440.98aa89155ad9b46e203069e3dccf6834074af6e8";
        private const string endpoint = "https://dictionary.yandex.net/api/v1/dicservice.json";

        public YandexService()
        {
        }

        public async Task<List<string>> GetLangsAsync()
        {
            string route = String.Format("/getLangs?key={0}", key);
            string result = null;

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

        public async Task<YandexAnswer> LookupAsync(string word, string langFrom, string langTo)
        {
            YandexAnswer answer = new YandexAnswer();
            string route = String.Format("/lookup?key={0}&lang={1}-{2}&text={3}", key, langFrom, langTo, word);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Get;
                request.RequestUri = new Uri(endpoint + route);

                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    answer.DictionaryAnswer = JsonConvert.DeserializeObject<YandexDictionary>(result);
                } 
                else
                    answer.Text = response.ReasonPhrase;

                answer.Code = response.StatusCode.ToString();
            }

            return answer;
        }
    }
}
