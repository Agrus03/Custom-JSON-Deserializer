using System;

namespace ConsoleApp
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("🚀 Запускаємо наш кастомний JSON-парсер...\n");

            // Шлях до файлу
            string filePath = "Transaction.json"; 

            try
            {
                // Викликаємо парсер
                Wallet myWallet = CustomJsonParser.Deserialize<Wallet>(filePath);

                // Виводимо результат
                Console.WriteLine("✅ Парсинг успішний! Ось розпаковані дані:\n");
                
                // Звичайний текст
                Console.WriteLine($"ID Гаманця: {myWallet.WalletId}");

                // Вкладений об'єкт (Balance)
                if (myWallet.Balance != null)
                {
                    Console.WriteLine($"Валюта: {myWallet.Balance.Currency}");
                    Console.WriteLine($"Доступний баланс: {myWallet.Balance.Available}");
                    Console.WriteLine($"Заблокований баланс: {myWallet.Balance.Blocked}");
                }

                // Масив об'єктів (Transactions)
                if (myWallet.Transactions != null)
                {
                    Console.WriteLine($"\nКількість транзакцій: {myWallet.Transactions.Count}");
                    foreach (var tx in myWallet.Transactions)
                    {
                        Console.WriteLine($" 🔹 Транзакція {tx.TransactionId}: {tx.Amount} {tx.Currency} (Статус: {tx.Status})");
                        
                        // Можемо навіть дістати дані з Metadata!
                        if (tx.Metadata != null)
                        {
                            Console.WriteLine($"    Джерело: {tx.Metadata.Source}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Упс! Виникла критична помилка:");
                Console.WriteLine($"ЩО СТАЛОСЯ: {ex.Message}");
                Console.WriteLine($"ДЕ СТАЛОСЯ (Слід):\n{ex.StackTrace}");
            }

            Console.WriteLine("\nНатисни Enter, щоб вийти...");
            Console.ReadLine();
        }
    }
}