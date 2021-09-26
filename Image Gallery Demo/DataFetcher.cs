using Newtonsoft.Json;
using System.Net.Http;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Gallery_Demo
{
    class DataFetcher
    {
        async Task<string> GetDatafromService(string searchstring)
        {
            string readText = null;
            try
            {
                String url = @"https://imagefetcherapi.azurewebsites.net/api/fetch_images?query=" +
               searchstring + "&max_count=5";
                using (HttpClient c = new HttpClient())
                {
                    readText = await c.GetStringAsync(url);
                }
            }
            catch
            {
                readText =
               File.ReadAllText(@"Data/sampleData.json");
            }
            return readText;
        }
        public async Task<List<ImageItem>> GetImageData(string search)
        {
            string data = await GetDatafromService(search);

            // deserializing the json file and typecasting to ImageItem class and returning the result
            return JsonConvert.DeserializeObject<List<ImageItem>>(data);
        }
    }
}
