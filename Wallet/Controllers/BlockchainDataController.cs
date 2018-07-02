using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Wallet.BlockchainAPI;
using Wallet.ViewModels;

namespace Wallet.Controllers
{
    [Route("api/[controller]")]
    public class BlockchainDataController : Controller
    {
        private IBlockchainExplorer _explorer;

        public BlockchainDataController(IBlockchainExplorer explorer)
        {
            this._explorer = explorer;
        }

        [HttpGet("[action]")]
        public async Task<WalletInfoViewModel> GetWalletInfo(string accountAddress)
        {
            WalletInfoViewModel model = new WalletInfoViewModel();
            Task<HexBigInteger> getBalance = _explorer.BalanceETH(accountAddress);

            model.Balance = Web3.Convert.FromWei(await getBalance, 18);
            return model;
        }
    }
}