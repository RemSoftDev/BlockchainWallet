using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.BlockchainAPI.Model
{
    class Token
    {
        public string Name;
        public decimal Balance;
        public int DecimalPlaces;
        public string Address;
        
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
    }
}
