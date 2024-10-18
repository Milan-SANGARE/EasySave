using System;
using System.IO;
using System.Collections;

namespace CryptoSoft
{
    class Program
    {
        static int Main(string[] args)
        {
            int ArgLen = args.Length;
            string source = "";
            string destination = "";

            // seting the source and destination of the files
            for (int i = 0; i < ArgLen; i++)
            {
                if (args[i] == "source" && i + 1 < ArgLen)
                {
                    source = args[i + 1];
                    i++;
                }
                else if (args[i] == "destination" && i + 1 < ArgLen)
                {
                    destination = args[i + 1];
                    i++;
                }
            }

            // Looking if th destination and the source have been filled
            if (source.Length == 0 || destination.Length == 0)
            {
                Console.WriteLine("Missing arguments, either the source or the destination is empty.");
                return -1;
            }

            // Looking if the file we need to crypt exist
            else if (!File.Exists(source))
            {
                Console.WriteLine("Source file doesn't exist.");
                return -1;
            }


            


            try
            {
                DateTime startTimeFile = DateTime.Now;
                
                //we convert the file we need to crypt to bytes and create a the file that will be crypted with the key
                byte[] to_crypt = File.ReadAllBytes(source);
                byte[] crypted = new byte[to_crypt.Length];

                //we set a key by default
                byte[] byteKey = new byte[8] { 9, 122, 23, 35, 75, 42, 166, 200 };
                BitArray bitKey = new BitArray(byteKey);

                //we crypting the file
                for (int i = 0; i < to_crypt.Length; i++)
                {
                    crypted[i] = (byte)(to_crypt[i] ^ byteKey[i % key.Length]);
                }

                // we send the file to the selected destination
                File.WriteAllBytes(destination, crypted);

                TimeSpan cryptTime = DateTime.Now - startTimeFile;
                return (int)cryptTime.TotalMilliseconds;

            }
            catch
            {
                Console.WriteLine("Cannot crypt this file.");
                return -1;
            }
        }
    }
}
