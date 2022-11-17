using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace lab7
{
    internal class Program
    {
        public static string path = "Logger.txt"; // Путь к файлу для логирования
        public static string chessman; // Переменная вида шахматной фигуры
        static void Main(string[] args)
        {
            string message = "Пользователь запустил программу"; // Сообщение, передаваемое в лог-файл при запуске
            WriteInFile(path, message);

            Console.WriteLine("Программа отвечает на вопрос: может ли шахматная фигура выполнить ход по координатам");
            Console.WriteLine("k — вертик. координата (счет слева направо), y — гориз. координата (счет снизу вверх)");

            int k = InputPoints("k");
            int l = InputPoints("l");
            int m = InputPoints("m");
            int n = InputPoints("n");

            while (k == m && l == n) // Запустится, если пользователь введет одинаковые координаты
            {
                Console.WriteLine("Координаты должны быть разными");
                n = InputPoints("n");
            }

            if (ColorCheck(k, l, m, n) == true) Console.WriteLine("Клетки одного цвета");
            else Console.WriteLine("Клетки разных цветов");

            while (true)
            {
                Console.Write("Введите фигуру (конь, ладья, ферзь, слон): ");
                chessman = Console.ReadLine();
                WriteInFile(path, $"Пользователь ввел: {chessman}");

                if (chessman == "конь" || chessman == "ладья" || chessman == "ферзь" || chessman == "слон")
                {
                    if (GeneralCheck(k, l, m, n, chessman) == true)
                    {
                        Console.WriteLine("Ход возможен");
                    }

                    else
                    {
                        Console.WriteLine("Ход невозможен");

                        if (chessman == "ладья")
                        {
                            Console.WriteLine($"Можно попробовать через промежуточный ход. Подходят точки: [{k}, {n}] или [{m}, {l}]");
                        }
                        else if (chessman == "слон")
                        {
                            Console.WriteLine(CheckSecondForBishop(k, l, m, n));
                        }
                        else if (chessman == "ферзь")
                        {
                            Console.WriteLine($"Можно попробовать через промежуточный ход. Подходят точки: [{k}, {n}] или [{m}, {l}]");
                            Console.WriteLine(CheckSecondForBishop(k, l, m, n));
                        }
                    }
                    break;
                }
            }
            Console.Write("Работа программы завершена. Для выхода из консоли нажмите Enter");
            Console.ReadLine();

            message = "Работа программы завершена"; // Сообщение, передаваемое в лог-файл при завершении работы программы
            WriteInFile(path, message);
        }

        private static void WriteInFile(string path, string message) // Метод записи в лог-файл
        {
            if (!File.Exists(path)) // Проверка существования файла для логирования
            {
                using var sw = new StreamWriter(path); // Создание лог-файла (вызывается, если файл еще не создан)
                sw.WriteLine(DateTime.Now.ToString() + " " + message); // Запись в лог-файл времени обращения и сообщения
            }
            else
            {
                using var sw = File.AppendText(path); // Преобразует читаемый файл в файл для дозаписи
                sw.WriteLine(DateTime.Now.ToString() + " " + message);
            }
        }

        private static string CheckSecondForBishop(int k, int l, int m, int n)
        {
            if (ColorCheck(k, l, m, n))
            {
                for (int i = 1; i <= 8; i++) // Цикл для подбора точки
                {
                    for (int j = 1; j <= 8; j++)
                    {
                        if (ColorCheck(k, l, i, j) == true && BishopCheck(k, l, i, j) == true && BishopCheck(i, j, m, n))
                        {
                            return $"Подойдет ход через промежуточную точку с координатами [{i}, {j}]";
                        }
                    }
                }
            }
            return "Невозможно и с 2х ходов";
        }

        private static bool GeneralCheck(int k, int l, int m, int n, string chessman) // Общая проверка (состоит из других)
        {
            if (chessman == "конь")
            {
                return KnightCheck(k, l, m, n);
            }
            else if (chessman == "ладья")
            {
                return RookCheck(k, l, m, n);
            }
            else if (chessman == "ферзь")
            {
                return QueenCheck(k, l, m, n);
            }
            else
            {
                return BishopCheck(k, l, m, n);
            }
        }

        private static int InputPoints(string coordinate) // Метод поочередного ввода координат
        {
            int number = 1; // Переменная проверки принадлежности координаты к [1; 8]

            while (true) // Цикл обработки ввода
            {
                Console.Write($"Введите {coordinate} (от 1 до 8): ");
                string input = Console.ReadLine();
                WriteInFile(path, $"Пользователь ввел: {input}");

                try
                {
                    number = int.Parse(input);
                    
                    if (number >= 1 && number <= 8) break;
                    else Console.WriteLine("Число должно быть от 1 до 8");
                }
                catch (Exception)
                {
                    Console.WriteLine("Ошибка ввода. Попробуйте еще раз");
                }
            }
            return number;
        }
        public static bool ColorCheck(int k, int l, int m, int n) // Проверка совпадения цвета исходной точки и конечной
        {
            if ((k + l) % 2 == (m + n) % 2)
            {
                return true;
            }
            return false;
        }

        public static bool KnightCheck(int k, int l, int m, int n) // Проверка хода для коня
        {
            if (Math.Pow((m - k), 2) + Math.Pow((n - l), 2) == 5)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool RookCheck(int k, int l, int m, int n) // Проверка хода для ладьи
        {
            if (k == m || n == l)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool BishopCheck(int k, int l, int m, int n) // Проверка хода для слона
        {
            if (Math.Pow((m - k), 2) == Math.Pow((n - l), 2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool QueenCheck(int k, int l, int m, int n) // Проверка хода для ферзя
        {
            if (RookCheck(k, l, m, n) || BishopCheck(k, l, m, n))
            {
                return true;
            }
            else
            {
                return false;
            }    
        }
    }
}
