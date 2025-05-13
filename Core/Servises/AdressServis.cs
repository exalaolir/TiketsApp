using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TiketsApp.Core.Servises
{
    class AdressServis
    {
        public static async Task<string> GetCoordsByQuery ( string query )
        {
            if (!await CheckInternetConnectionAsync())
            {
                throw new HttpRequestException("Нет подключения к интернету");
            }

            string appName = "ticketsApp_0.1";
            var requestUrl = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(query)}&format=json";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", appName);
                client.DefaultRequestHeaders.Add("Accept-Language", "ru-RU");

                var response = await client.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    using var jsonResults = JsonDocument.Parse(json);

                    foreach (var root in jsonResults.RootElement.EnumerateArray())
                    {
                        if (root.TryGetProperty("display_name", out var adress))
                        {
                            var adressStr = adress.GetString();

                            if(adressStr == null)
                                throw new Exception("Адрес не найден");

                            return adressStr;
                        }
                        else throw new Exception("Адрес не найден");
                    }
                    throw new Exception("Адрес не найден");
                }
                else throw new Exception("Неверные данные");
            }
        }

        private static async Task<bool> CheckInternetConnectionAsync ()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(5);
                    var response = await client.GetAsync("http://www.google.com");
                    return response.IsSuccessStatusCode;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
