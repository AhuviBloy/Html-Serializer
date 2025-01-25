using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;


namespace Html_Serializer
{
    internal class HtmlHelper
    {
        private readonly static HtmlHelper _instance = new HtmlHelper();
        public static HtmlHelper Instance => _instance;
        public string[] HtmlTags { get; set; } 
        public string[] HtmlVoidTags { get; set; }
        private HtmlHelper() 
        {
            try
            {
                
                HtmlTags = JsonSerializer.Deserialize<string[]>(File.ReadAllText("HtmlTags.json"));
                HtmlVoidTags = JsonSerializer.Deserialize<string[]>(File.ReadAllText("HtmlVoidTags.json"));

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
