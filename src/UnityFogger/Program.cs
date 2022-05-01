using System;
using System.IO;

namespace UnityFogger
{
    internal class Program
    {
        static readonly byte[] signature = {0x4D, 0x5A},
                      patch = {0x4D, 0x61, 0x73, 0x6B, 0x00, 0x00, 0x00, 0x00};
        const int offset = 23859608;

        static void Main()
        {
            Console.Title = "Unity Fogger";

            while(true)
            {
                var path = Console.ReadLine();
                byte[] player = null;

                if (!File.Exists(path))
                {
                    Console.WriteLine("The specified file does not exist.");
                    continue;
                }

                try
                {
                    player = File.ReadAllBytes(path);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }

                if (player[0] != signature[0] && player[1] != signature[1])
                {
                    Console.WriteLine("The specified file is not a valid DLL.");
                    continue;
                }

                if (!path.ToLower().EndsWith("unityplayer.dll"))
                {
                    Console.WriteLine("This utility is only intended for Unity's player library, using this utility on other assemblies may result in corruption.");
                    continue;
                }

                for (int i = offset, j = 0; i < offset + patch.Length; i++, j++)
                    player[i] = patch[j];

                try
                {
                    File.WriteAllBytes(path, player);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }

                Console.WriteLine("Operation completed succesfully. Press any key to exit . . .");
                Console.ReadKey();
                break;
            }
        }
    }
}