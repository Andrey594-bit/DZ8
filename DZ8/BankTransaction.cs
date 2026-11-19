using System;
namespace DZ8
{
    public class BankTransaction
    {
        private readonly DateTime transaction;
        private readonly decimal Amount;
        public BankTransaction(decimal Amount)
        {
            transaction = DateTime.Now;
            this.Amount = Amount;
        }
        public override string ToString()
        {
            return $"{transaction} | Сумма: {Amount}";
        }
    }
}