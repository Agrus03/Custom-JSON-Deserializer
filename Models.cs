using System;
using System.Collections.Generic;

namespace ConsoleApp
{
    // ГОЛОВНИЙ КОНТЕЙНЕР JSON-файлу
    public class Wallet
    {
        public string? WalletId { get; set; }
        public Balance? Balance { get; set; }
        // Використовуємо List, бо транзакцій може бути багато
        public List<Transaction>? Transactions { get; set; } 
    }
    
    // Nested obj для зберігання фінансових показників
    public class Balance
    {
        public string? Currency { get; set; }
        public decimal Available { get; set; }
        public decimal Blocked { get; set; }
    }
    
    // Модель операції в гаманці
    public class Transaction
    {
        public string? TransactionId { get; set; }
        public string? Type { get; set; }
        public decimal Amount { get; set; }
        public string? Currency { get; set; }
        public string? Status { get; set; }
        public string? Timestamp { get; set; }
        public Metadata? Metadata { get; set; } 
    }
    
    // Додаткові дані конкретної транзакції
    public class Metadata
    {
        public string? Source { get; set; }
        public string? Reference { get; set; }
    }
}