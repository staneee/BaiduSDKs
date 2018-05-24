using BaiduTranslateSDK;
using System;
using System.Text;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            StringBuilder input = new StringBuilder();
            input.AppendLine("Awesome-Xamarin is an amazing list for people who need a certain feature on their app, so the best ways to use are:");
            input.AppendLine("Ask for help on our Twitter");
            input.AppendLine("Simply press command + F to search for a keyword");


            if (BDTranslate.Init())
            {
                var res = BDTranslate.Translate(input.ToString(), "auto", "zh");

                if (res != null && res.trans != null)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var item in res.trans)
                    {
                        sb.AppendLine(item.dst.ToString());
                    }

                    Console.WriteLine(sb.ToString());
                }
            }

            Console.WriteLine("执行完成");
            Console.ReadKey();

           
        }
    }
}
