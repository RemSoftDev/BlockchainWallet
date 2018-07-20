using System;
using System.Linq;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.WebSockets.Internal;
using Nethereum.Hex.HexConvertors;
using Nethereum.Web3;
using Wallet.BlockchainAPI.Model;
using Wallet.Models;
using Wallet.Helpers;

namespace Wallet.BlockchainAPI
{
    public static class InputDecoder
    {
        public static TransactionInput DecodeInput(string input, string contractAddress, WalletDbContext dbContext)
        {
            if (input.StartsWith(Helpers.Constants.Strings.TransactionType.Transfer))
            {
                HexBigIntegerBigEndianConvertor hexConverter = new HexBigIntegerBigEndianConvertor();
                //cut off method name (first 4 byte)
                input = input.Substring(10);
                //get value         
                var value = hexConverter.ConvertFromHex(input.Substring(input.Length / 2));
                //get address
                var address = hexConverter.ConvertToHex(hexConverter.ConvertFromHex("0x" + input.Substring(0, input.Length / 2)));

                var token = dbContext.Erc20Tokens.FirstOrDefault(t =>
                    string.Equals(t.Address, contractAddress, StringComparison.CurrentCultureIgnoreCase));
                return new Model.TransactionInput()
                {
                    To = address,
                    Value = token != null ? Web3.Convert.FromWei(value, token.DecimalPlaces) : (decimal)value,
                    What = token?.Symbol ?? "Unknown",
                    ContractAddress = token?.Address ?? "Unknown"
                };
            }
            else
            {
                return new TransactionInput()
                {
                    To = string.Empty,
                    Value = default(decimal),
                    What = string.Empty,
                    ContractAddress = string.Empty
                };
            }
        }

        public static TransactionInput GetTokenCountAndAddressFromInput(string input, int decimalPlaces)
        {
            if (input.StartsWith(Helpers.Constants.Strings.TransactionType.Transfer))
            {
                HexBigIntegerBigEndianConvertor hexConverter = new HexBigIntegerBigEndianConvertor();
                //cut off method name (first 4 byte)
                input = input.Substring(10);
                //get value         
                var value = hexConverter.ConvertFromHex(input.Substring(input.Length / 2));
                //get address
                var address = hexConverter.ConvertToHex(hexConverter.ConvertFromHex("0x" + input.Substring(0, input.Length / 2)));

                return new TransactionInput()
                {
                    To = address,
                    Value = Web3.Convert.FromWei(value, decimalPlaces),
                    What = string.Empty,
                    ContractAddress = string.Empty
                };

            }
            else
            {
                return new TransactionInput()
                {
                    To = string.Empty,
                    Value = default(decimal),
                    What = string.Empty,
                    ContractAddress = string.Empty
                };
            }
        }
    }
}