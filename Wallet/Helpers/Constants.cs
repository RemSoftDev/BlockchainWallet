namespace Wallet.Helpers
{
    public static class Constants
    {
        public static class Strings
        {
            public static class JwtClaimIdentifiers
            {
                public const string Rol = "rol", Id = "id";
                public const string AdminRol = "adminrol", AdminId = "adminid";
            }

            public static class JwtClaims
            {
                public const string ApiAccess = "api_access";
            }

            public static class WalletCode
            {
                public const string AccountCode = "0x";
            }

            public static class TransactionType
            {
                public const string Transfer = "0xa9059cbb";
            }
        }
    }
}