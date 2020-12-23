using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EventBusFuncion
{
    public class Program1
    {
        static void Main(string[] args)
        {
            string file = @"C:\Users\beatriz.grande\Desktop\SquidLog\SquidLog\access.log";

            var result = TextFile(file);
            Connect("127.0.0.1", result);
        }

        static List<Object> TextFile(string file)
        {
            var Lines = new List<string>();
            var result = new List<Object>();

            try
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    string log;
                    while ((log = sr.ReadLine()) != null)
                    {
                        Lines.Add(sr.ReadLine());

                    }
                }

                Parallel.For(0, Lines.Count, x =>
                {
                    result.Add(ToObject(Lines[x]));
                });
            }
            finally
            {
                if (Lines != null)
                {
                    Lines = null;
                }
            }
            GC.Collect();
            return result;
        }

        static Object ToObject(string row)
        {
            var result = new Object();
            result.Time = Time(row);
            result.Duration = Duration(row);
            result.IP = IP(row);
            result.ResultCode = ResultCode(row);
            result.Bytes = Bytes(row);
            result.Methods = Methods(row);
            result.URL = Url(row);
            result.User = User(row);
            result.Type = Type(row);            
            return result;
        }
        
        static DateTime Time(string subjectString)
        {
            var regexObj = new Regex(@".{0,14}\.{0,14}\s|\s");
            Match matchResults = regexObj.Match(subjectString);
            return UnixTimeStampToDateTime(matchResults.Value);
        }

        private static DateTime UnixTimeStampToDateTime(string unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds((long)Convert.ToDouble(unixTimeStamp));
            return dtDateTime;
        }

        static string Duration(string subjectString)
        {
            var regexObj = new Regex(@"([0-9]{6}|[0-9]{5}|[0-9]{3})\s");
            Match matchResults = regexObj.Match(subjectString);
            return matchResults.Value;
        }

        static string IP(string subjectString)
        {
            var regexObj = new Regex(@"((\d{2}|\d{3})\.(\d{2}|\d{3})\.(\d{3}|\d{2})\.(\d{3}|\d{2})\s)|([a-z]{2}\d[a-z]\-\d{2}\-\d{2}\-\d{3}\-\d{2}\.[a-z]{2}\d[a-z]{7}\.[a-z]{3})");
            Match matchResults = regexObj.Match(subjectString);
            return matchResults.Value;
        }

        static string ResultCode(string subjectString)
        {
            var regexObj = new Regex(@"[A-Z]{3}[\._]([A-Z]{4}|[A-Z]{5})[\/]\d{3}\s");
            Match matchResults = regexObj.Match(subjectString);
            return matchResults.Value;
        }

        static string Bytes(string subjectString)
        {
            var regexObj = new Regex(@"((\d{4}|\d{3})|(\d))\s");
            Match matchResults = regexObj.Match(subjectString);
            return matchResults.Value;
        }

        static string Methods(string subjectString)
        {
            var regexObj = new Regex(@"([A-Z]{3}|[A-Z]{7})\s");
            Match matchResults = regexObj.Match(subjectString);
            return matchResults.Value;
        }

        static string Url(string subjectString)
        {
            var regexObj = new Regex(@"((http:[\/][\/]([\w_-]+(?:(?:\.[\w_-]+)+))([\w.,@?^=%&:/~+#-]*[\w@?^=%&/~+#-])?)|[a-z]{5}\.[a-z]{3}\.[a-z]{3}\:\d{3})");
            Match matchResults = regexObj.Match(subjectString);
            return matchResults.Value;
        }

        static string User(string subjectString)
        {
            var regexObj = new Regex(@"\s\-\s([A-Z]{6})[\/](\d{3}|\d{2})\.(\d{3}|\d{2})\.(\d{3}|\d{2})\.(\d{3})|([A-Z]{4})[\/]\-\s");
            Match matchResults = regexObj.Match(subjectString);
            return matchResults.Value;
        }

        static string Type(string subjectString)
        {
            var regexObj = new Regex(@".(\s|\s)([a-z]{4}[\/][a-z]{4})");
            Match matchResults = regexObj.Match(subjectString);
            return matchResults.Value;
        }
       
   

        static void Connect(String server, List<Object> result)
        {
            try
            {
                Int32 port = 13000;
                TcpClient client = new TcpClient(server, port);

                BinaryFormatter binFormatter = new BinaryFormatter();
                var mStream = new MemoryStream();
                binFormatter.Serialize(mStream, result);


                Byte[] data = mStream.ToArray();
                NetworkStream stream = client.GetStream();

                stream.Write(data, 0, data.Length);

                Console.WriteLine("Sent: {0}", result);

                String responseData = String.Empty;

                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);
                Console.Read();

                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

            Console.WriteLine("\n Press Enter to continue...");
        }
    }
}

