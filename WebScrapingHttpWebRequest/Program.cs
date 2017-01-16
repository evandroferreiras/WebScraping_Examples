using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using WebScraping.Entitidades;

namespace WebScraping.ExampleHttpWebRequest
{
    class Program
    {
        static void Main(string[] args)
        {
            var uri = "https://www.jucerja.rj.gov.br/Servicos/TbPrecos/index.asp";
            var htmlContent = GetHtmlContent(uri);

            IList<RowTableContent> table = GetTableContentFromHtml(htmlContent);
            
            string json = JsonConvert.SerializeObject(table, Formatting.Indented);

            Console.WriteLine(json);

            Console.ReadKey();
        }

        private static IList<RowTableContent> GetTableContentFromHtml(string htmlContent)
        {
            IList<RowTableContent> result = new List<RowTableContent>();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            var table = doc.DocumentNode.SelectNodes("//table")[0];
            for (int i = 2; i < 12; i++)
            {               
                HtmlNode row = table.SelectNodes("tr")[i];

                var rowTableContent = new RowTableContent();
                var cells = row.SelectNodes("td");
                rowTableContent.Ato = (cells[0]).InnerText;
                rowTableContent.Descricao = (cells[1]).InnerText;

                result.Add(rowTableContent);
            }
            return result;
        }

        private static string GetHtmlContent(string uri)
        {
            var result = String.Empty;
                                    
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream receiveStream = response.GetResponseStream())
                {
                    using(StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8)) {
                        result = readStream.ReadToEnd();
                        readStream.Close();
                    }
                    receiveStream.Close();
                }
                response.Close();
            }
            return result;
        }
    }
}
