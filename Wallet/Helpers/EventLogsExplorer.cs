using System;
using System.Collections.Generic;
using System.Linq;
using Wallet.Models;

namespace Wallet.Helpers
{
    public static class EventLogsExplorer
    {
        public static List<TokenHolder> GetInfoFromLogs(List<CustomEventLog> logs)
        {
            List<TokenHolder> holders = new List<TokenHolder>();

            var addresses = new HashSet<string>();

            logs.ForEach(l =>
            {
                addresses.Add(l.From);
                addresses.Add(l.To);
            });

            foreach (var address in addresses)
            {
                var generalTransNumber = logs.Count(e => e.To.Equals(address,
                                                             StringComparison.CurrentCultureIgnoreCase) ||
                                                         e.From.Equals(address,
                                                             StringComparison.CurrentCultureIgnoreCase));

                var sentTransactionsNumber =
                    logs.Count(e => e.From.Equals(address, StringComparison.CurrentCultureIgnoreCase));

                var recievedTransactionsNumber =
                    logs.Count(e => e.To.Equals(address, StringComparison.CurrentCultureIgnoreCase));

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