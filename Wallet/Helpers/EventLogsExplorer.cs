using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wallet.Models;

namespace Wallet.Helpers
{
    public static class EventLogsExplorer
    {
        public static List<TokenHolder> GetInfoFromLogs(List<CustomEventLog> logs)
        {
            var groups = (logs.GroupBy(l => l.From).ToList()).Concat(logs.GroupBy(l => l.To).ToList());

            var sortedGroups = groups.GroupBy(i => i.Key)
                .Select(group => new
                {
                    Key = group.Key,
                    Values = group.SelectMany(item => item).ToList()
                }).ToList();

            var holders = new List<TokenHolder>();

            foreach (var g in sortedGroups)
            {
                var sentTransactionsNumber = g.Values.Count(e => e.From.Equals(g.Key, StringComparison.CurrentCultureIgnoreCase));
                holders.Add(new TokenHolder()
                {
                    Address = g.Key,
                    GeneralTransactionsNumber = g.Values.Count,
                    SentTransactionsNumber = sentTransactionsNumber,
                    ReceivedTransactionsNumber = g.Values.Count - sentTransactionsNumber,
                    TokensSent = g.Values.Where(e => e.From.Equals(g.Key, StringComparison.CurrentCultureIgnoreCase))
                        .Select(t => t.AmountOfToken).Sum(),
                    TokensReceived = g.Values.Where(e => e.To.Equals(g.Key, StringComparison.CurrentCultureIgnoreCase))
                        .Select(t => t.AmountOfToken).Sum()
                });
            }

            return holders;
        }
    }
}