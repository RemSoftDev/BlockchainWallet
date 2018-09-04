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
            var addresses = new List<string>();

            logs.ForEach(l =>
            {
                addresses.Add(l.From);
                addresses.Add(l.To);
            });

            addresses = addresses.Distinct().ToList();

            var tasks = new List<Task<List<TokenHolder>>>();

            for (int i = 0; i < addresses.Count; i += 1000)
            {
                List<string> temp = addresses.Skip(i).Take(1000).ToList();
                var task = Task.Run(() => SortHolders(temp, logs));
                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());
            var result = new List<TokenHolder>();
            tasks.ForEach(r => result.AddRange(r.Result));

            return result;
        }

        private static async Task<List<TokenHolder>> SortHolders(List<string> addresses, List<CustomEventLog> logs)
        {
            var holders = new List<TokenHolder>();

            foreach (var address in addresses.AsQueryable())
            {
                var sentTransactionsNumber =
                    logs.Count(e => e.From.Equals(address, StringComparison.CurrentCultureIgnoreCase));

                var recievedTransactionsNumber =
                    logs.Count(e => e.To.Equals(address, StringComparison.CurrentCultureIgnoreCase));

                var generalTransNumber = sentTransactionsNumber + recievedTransactionsNumber;

                var tokenSentNumber = logs.Where(e => e.From.Equals(address, StringComparison.CurrentCultureIgnoreCase))
                    .Select(t => t.AmountOfToken).Sum();

                var tokenRecievedNumber = logs
                    .Where(e => e.To.Equals(address, StringComparison.CurrentCultureIgnoreCase))
                    .Select(t => t.AmountOfToken).Sum();


                holders.Add(new TokenHolder()
                {
                    Address = address,
                    GeneralTransactionsNumber = generalTransNumber,
                    SentTransactionsNumber = sentTransactionsNumber,
                    ReceivedTransactionsNumber = recievedTransactionsNumber,
                    TokensSent = tokenSentNumber,
                    TokensReceived = tokenRecievedNumber
                });
            }

            return holders;
        }
    }
}