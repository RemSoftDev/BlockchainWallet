using System;
using System.Collections.Generic;
using Wallet.Models;
using Wallet.ViewModels;

namespace Wallet.Helpers
{
    public static class WatchlistHelper
    {
        public static List<WatchlistByAccounts> OrganizeData(List<UserWatchlist> data)
        {
            var result = new List<WatchlistByAccounts>();
            var accounts = new List<UserWatchListViewModel>();
            var contracts = new List<UserWatchListViewModel>();

            foreach (var entry in data)
            {
                if (entry.IsContract)
                {
                    contracts.Add(new UserWatchListViewModel() { Address = entry.Address, UserEmail = entry.UserEmail });
                }
                else
                {
                    accounts.Add(new UserWatchListViewModel() { Address = entry.Address, UserEmail = entry.UserEmail });
                }
            }

            int length = contracts.Count >= accounts.Count ? contracts.Count : accounts.Count;
            if (contracts.Count > accounts.Count)
            {
                int j = length - accounts.Count;
                for (int i = 0; i < j; i++)
                {
                    accounts.Add(new UserWatchListViewModel() { Address = String.Empty, UserEmail = String.Empty });
                }
            }
            else
            {
                int j = length - contracts.Count;
                for (int i = 0; i < j; i++)
                {
                    contracts.Add(new UserWatchListViewModel() { Address = String.Empty, UserEmail = String.Empty });
                }
            }

            for (int i = 0; i < length; i++)
            {
                result.Add(new WatchlistByAccounts()
                {
                    Contract = contracts[i],
                    Account = accounts[i]
                });
            }

            return result;
        }
    }
}