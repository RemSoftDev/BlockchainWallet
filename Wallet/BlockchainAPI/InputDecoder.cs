using System;
using System.Linq;
using System.Numerics;
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
        public static TransactionInput DecodeTransferInput(string input)
        {

            HexBigIntegerBigEndianConvertor hexConverter = new HexBigIntegerBigEndianConvertor();
            //cut off method name (first 4 byte)
            input = input.Substring(10);
            //get value      
            if (input.Length > 133)
            {
                 return new TransactionInput()
                {
                    To = string.Empty,
                    Value = new BigInteger()
                };
            }
            var value = hexConverter.ConvertFromHex(input.Substring(input.Length / 2));
            //get address
            var address = hexConverter.ConvertToHex(hexConverter.ConvertFromHex("0x" + input.Substring(0, input.Length / 2)));

            return new TransactionInput()
            {
                To = address,
                Value = value
            };
        }

        public static TransactionInput GetTokenCountAndAddressFromInput(string input)
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
                    Value = value
                };

            }
            else
            {
                return new TransactionInput()
                {
                    To = string.Empty,
                    Value = default(BigInteger),
                };
            }
        }
    }
}