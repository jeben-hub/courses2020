using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Task3
{
    class Program
    {
        static void Main(string[] args)
        {
            if (isArgsValid(args))
            {
                int computerMove = GetRandomNumber(args.Length);
                byte[] key = GetRandomKey();
                string hash = GetHash(computerMove, key);
                int userMove = Menu(args, hash);
                Console.WriteLine("Computer move: {0}", args[computerMove]);
                Refereeing(args.Length, userMove, computerMove);
                string tempKey = String.Join("", BitConverter.ToString(key).Split('-'));
                Console.WriteLine("HMAC key: {0}", tempKey);
            }
            else Console.WriteLine("Invalid arguments!");
        }
        static int Menu(string[] moves, string hash)
        {
            int choise;
            do
            {
                Console.Clear();
                Console.WriteLine("HMAC: " + hash + '\n' +
                    "Available moves: ");
                for (int i = 0; i < moves.Length; i++)
                {
                    Console.WriteLine("{0} - {1}", i + 1, moves[i]);
                }
                Console.Write("0 - Exit" + '\n' +
                    "Enter your move: ");
                if (!int.TryParse(Console.ReadLine(), out choise))
                {
                    continue;
                }
                else if (0 == choise)
                    Environment.Exit(0);
                else
                {
                    Console.WriteLine("Your move: {0}", moves[choise - 1]);
                    break;
                }
            } while (true);
            return choise - 1;
        }
        static int GetRandomNumber(int movesCount)
        {
            int number;
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] randomByte = new byte[1];
                rng.GetBytes(randomByte);
                number = (int)(randomByte[0] / 255d * movesCount);
            }
            return number;
        }
        static byte[] GetRandomKey()
        {
            byte[] key = new byte[16];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(key);
            }
            return key;
        }
        static string GetHash(int number, byte[] key)
        {
            using (HMACSHA256 hmac = new HMACSHA256(key))
            {
                byte[] data = hmac.ComputeHash(BitConverter.GetBytes(number));
                var sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
        }
        static bool isArgsValid(string[] args)
        {
            if (args.Length < 3 || (args.Length % 2) == 0)
                return false;
            var set = new HashSet<string>();
            foreach (var item in args)
                if (!set.Add(item))
                    return false;
            return true;
        }
        static void Refereeing(int movesCount, int userMove, int computerMove)
        {
            if (userMove == computerMove)
            {
                Console.WriteLine("Draw");
                return;
            }
            int midlle = (int)(movesCount / 2) + 1;
            if((computerMove > userMove && computerMove < userMove + midlle) ||
               (computerMove + movesCount > userMove && computerMove + movesCount < userMove + midlle))
            {
                Console.WriteLine("You lose :(");
                return;
            }
            Console.WriteLine("You win!");
        }
    }
}
