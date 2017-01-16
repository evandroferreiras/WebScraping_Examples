using AngleSharp.Dom;
using AngleSharp.Parser.Html;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebScraping.Entitidades;

namespace WebScraping.ExampleAngleSharp
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

            var parser = new HtmlParser();
            var doc = parser.Parse(htmlContent);
            var table = doc.All.First(m => m.LocalName.Equals("table"));
            var rows = table.QuerySelectorAll("*").Where(m => m.LocalName.Equals("tr")).ToList();

            for (int i = 2; i < 12; i++)
            {
                IElement row = rows[i];

                var rowTableContent = new RowTableContent();
                var cells = row.QuerySelectorAll("*");
                rowTableContent.Ato = (cells[0]).TextContent;
                rowTableContent.Descricao = (cells[1]).TextContent;
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
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
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
