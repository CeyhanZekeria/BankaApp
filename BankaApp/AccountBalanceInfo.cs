 using System;
using System.Collections.Generic;
using System.Text;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace BankaApp
{
    public class AccountBalanceInfo
    {
        public int AccountId { get; set; }
        public string AccountNo { get; set; }
        public decimal Balance { get; set; }
        public int CurrencyId { get; set; }
    }
}
