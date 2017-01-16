using HtmlAgilityPack;
using Newtonsoft.Json;
using ScrapySharp.Network;
using ScrapySharp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScraping.Entitidades;

namespace WebScraping.ScrapySharp
{
    class Program
    {
        static void Main(string[] args)
        {
            var uri = "https://www.jucerja.rj.gov.br/Servicos/TbPrecos/index.asp";
            HtmlNode htmlContent = GetHtmlContent(uri);
            
            IList<RowTableContent> table = GetTableContentFromHtml(htmlContent);

            string json = JsonConvert.SerializeObject(table, Formatting.Indented);

            Console.WriteLine(json);

            Console.ReadKey();
        }

        private static bool FilterRows(HtmlNode row) 
        {
            var cells = row.CssSelect("td");
            if (cells.Count() > 0) {
                var firstcell = cells.FirstOrDefault();
                if (firstcell != null)
                {
                    if (firstcell.HasAttributes)
                    {
                        if (firstcell.GetAttributeValue("valign").Equals("middle") || firstcell.GetAttributeValue("colspan").Equals("6"))                        
                            return false;                        
                    }
                }
            }            
            return true;
        }

        private static IList<RowTableContent> GetTableContentFromHtml(HtmlNode htmlnode)
        {
            IList<RowTableContent> result = new List<RowTableContent>();
            
            var table = htmlnode.CssSelect("table").First();
            var rows = table.CssSelect("tr").Where(x => FilterRows(x)).Take(10);
            foreach (var row in rows)
            {
                var rowTableContent = new RowTableContent();
                var cells = row.CssSelect("td");
                rowTableContent.Ato = (cells.ElementAt(0)).InnerText;
                rowTableContent.Descricao = (cells.ElementAt(1)).InnerText;
                result.Add(rowTableContent);
            }

            return result;
        }

        private static HtmlNode GetHtmlContent(string uri)
        {
            ScrapingBrowser browser = new ScrapingBrowser();

            var pageResult = browser.NavigateToPage(new Uri(uri));

            return pageResult.Html;
        }
    }
}
