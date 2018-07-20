
using System.Numerics;

namespace Wallet.BlockchainAPI.Model
{
    public class TransactionInput
    {
        public BigInteger Value { get; set; }
        public string To { get; set; }
    }
}
