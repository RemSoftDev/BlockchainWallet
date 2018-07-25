using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Extensions;
using AngleSharp.Parser.Html;
using Wallet.ViewModels;

namespace Wallet.Helpers
{
    public static class Parser
    {
        public static async Task<ContractHoldersAndTransactionsModel> GetContractHoldersAndTransactions(string contractAddress)
        {
            HtmlParser parser = new HtmlParser();

            Task<int> getTransactionsCount = Task.Run(() =>
            {
                using (WebClient client = new WebClient())
                {
                    var htmlCode = client.DownloadString(
                        $"https://etherscan.io/token/generic-tokentxns2?contractAddress={contractAddress}&a=");
                    IHtmlDocument document = parser.Parse(htmlCode);
                    var tableElement = document.QuerySelector("span.hidden-xs");
                    var text = tableElement.Text();
                    return Int32.Parse(Regex.Match(text, @"\d+").Value);
                }
            });

            Task<int> getHoldersCount = Task.Run(() =>
            {
                using (WebClient client = new WebClient())
                {
                    string htmlCode1 =
                        client.DownloadString(
                            $"https://etherscan.io/token/{contractAddress}");
                    IHtmlDocument document1 = parser.Parse(htmlCode1);
                    IElement tableElement1 = document1.GetElementById("ContentPlaceHolder1_tr_tokenHolders");
                    var text1 = tableElement1.Text();
                    return Int32.Parse(Regex.Match(text1, @"\d+").Value);
                }
            });

            try
            {
                await Task.WhenAll(getHoldersCount, getTransactionsCount);
                return new ContractHoldersAndTransactionsModel()
                {
                    HoldersCount = getHoldersCount.Result,
                    TransactionsCount = getTransactionsCount.Result
                    
                };
            }
            catch (Exception e)
            {
                return new ContractHoldersAndTransactionsModel();
            } 
        }
    }
}