using DZ8;
using System;
using System.Collections;
using System.IO;

namespace TumakovLab
{
    public class BankAccount : IDisposable
    {
        public static int LastAccountNumber = 0;
        private decimal FinalBalance;
        private decimal NewBalance;

        public int UniqueAccountNumber { get; private set; }
        public decimal Balance { get; private set; }
        public AccountType AccountType { get; private set; }

        private Queue transactions;

        static int GenerationNumber()
        {
            LastAccountNumber++;
            return LastAccountNumber;
        }

        public BankAccount()
        {
            UniqueAccountNumber = GenerationNumber();
            Balance = 0;
            AccountType = AccountType.Current;
            transactions = new Queue();
        }

        public BankAccount(decimal balance)
        {
            UniqueAccountNumber = GenerationNumber();
            Balance = balance;
            AccountType = AccountType.Current;
            transactions = new Queue();
        }

        public BankAccount(AccountType accountType)
        {
            UniqueAccountNumber = GenerationNumber();
            Balance = 0;
            AccountType = accountType;
            transactions = new Queue();
        }

        public BankAccount(decimal balance, AccountType accountType)
        {
            UniqueAccountNumber = GenerationNumber();
            Balance = balance;
            AccountType = accountType;
            transactions = new Queue();
        }

        public void PrintHistory()
        {
            Console.WriteLine($"История операций для счета {UniqueAccountNumber}:");
            if (transactions.Count == 0) Console.WriteLine("Пусто.");

            foreach (BankTransaction check in transactions)
            {
                Console.WriteLine(check.ToString());
            }
        }

        public void DepositOrWithdrawBalance()
        {
            bool flag = true;
            while (flag)
            {
                Console.WriteLine("Выберите операцию");
                Console.WriteLine("1 - снять деньги");
                Console.WriteLine("2 - пополнить баланс");
                Console.Write("Ваш выбор: ");
                string input = Console.ReadLine();

                if (input == "1")
                {
                    Console.WriteLine("Введите сумму снятия: ");
                    decimal withdraw = Convert.ToDecimal(Console.ReadLine());

                    if (withdraw > Balance)
                    {
                        Console.WriteLine("Недостаточно средств");
                    }
                    else
                    {
                        Balance = Balance - withdraw;
                        BankTransaction check = new BankTransaction(-withdraw);
                        transactions.Enqueue(check);
                    }
                    break;
                }
                else if (input == "2")
                {
                    Console.WriteLine("Введите сумму пополнения: ");
                    decimal deposit = Convert.ToDecimal(Console.ReadLine());

                    Balance = Balance + deposit;
                    BankTransaction check = new BankTransaction(deposit);
                    transactions.Enqueue(check);
                    break;
                }
                else
                {
                    Console.WriteLine("Выберите 1 или 2");
                }
            }
        }

        public void TransferFrom(BankAccount sourceAccount, decimal amount)
        {
            if (amount > sourceAccount.Balance)
            {
                Console.WriteLine("Недостаточно средств на счете-источнике");
                return;
            }

            sourceAccount.Balance -= amount;
            this.Balance += amount;

            sourceAccount.transactions.Enqueue(new BankTransaction(-amount));
            this.transactions.Enqueue(new BankTransaction(amount));

            Console.WriteLine($"Перевод {amount} выполнен.");
        }

        public void Dispose()
        {
            string fileName = $"History_{UniqueAccountNumber}.txt";

            using (StreamWriter writer = new StreamWriter(fileName))
            {
                writer.WriteLine($"Счет: {UniqueAccountNumber}. Баланс: {Balance}");

                foreach (BankTransaction check in transactions)
                {
                    writer.WriteLine(check.ToString());
                }
            }

            Console.WriteLine($"[Счет {UniqueAccountNumber}] Сохранен в файл {fileName}");

            GC.SuppressFinalize(this);
        }
    }
}