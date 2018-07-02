using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.BlockchainAPI.Model
{
    public class Token
    {
        public string Name;
        public decimal Balance;
        public int DecimalPlaces;
        public string Address;
      
        public string Symbol;
        public string Type;

        public Token(string name, decimal balance)
        {
            Name = name;
            Balance = balance;
        }

        public Token(string name, string address, int decimalPlaces)
        {
            Name = name;
            Address = address;
            DecimalPlaces = decimalPlaces;
        }

        public Token(string name, string address, int decimalPlaces, string types)
        {
            Name = name;
            Address = address;
            DecimalPlaces = decimalPlaces;
            Type = types;
        }
        public Token()
        {
            Name = string.Empty;
            Address = string.Empty;
            DecimalPlaces = 0;
            Type = string.Empty;
        }

    }
}
