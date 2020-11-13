namespace ControllingStationSandbox
{
    using Iec608705104.Tests;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using Console = Colorful.Console;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var payloadDir = args.Length > 0 ? args[0] : null;
            var payloads = new Payloads(payloadDir).Value;
            var keys = AssignConsoleKeys(payloads.Keys.ToArray());
            var names = keys.ToDictionary(x => x.Value, x => x.Key);

            using var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            NetworkStream stream = null;
            bool flush = true;

            Help(keys, flush);

            do
            {
                var key = Console.ReadKey().Key;
                Console.Write("\b \b");
                try
                {
                    switch (key)
                    {
                        case ConsoleKey.A:
                            stream = Connect(socket, args);
                            break;

                        case ConsoleKey.B:
                            socket.Close();
                            break;

                        case ConsoleKey.C:
                            flush = !flush;
                            Console.WriteLine($"auto flush {(flush ? "ON" : "OFF")}", Color.White);
                            break;

                        case ConsoleKey.D:
                            Flush(stream);
                            break;

                        default:
                            if (names.TryGetValue(key, out var name) && payloads.TryGetValue(name, out var payloadCase))
                            {
                                Send(stream, payloadCase.Bytes, flush);
                            }

                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR {ex.GetType().Name}: {ex.Message}", Color.Red);
                }

                if (key == ConsoleKey.Escape)
                {
                    break;
                }
            }
            while (true);

            stream?.Dispose();
            socket.Close();
        }

        private static NetworkStream Connect(Socket socket, string[] args)
        {
            var ip = IPAddress.Loopback;
            if (args.Length > 1)
            {
                var ips = Dns.GetHostAddresses(args[1]);
                if (ips.Length > 0)
                {
                    ip = ips[0];
                }
            }

            var port = 2404;
            if (args.Length > 2 && int.TryParse(args[2], out var p))
            {
                port = p;
            }

            var endpoint = new IPEndPoint(ip, port);
            socket.Connect(endpoint);
            Console.WriteLine($"Connected to {endpoint}", Color.Gray);

            return new NetworkStream(socket);
        }

        private static void Send(NetworkStream ns, byte[] buffer, bool flush)
        {
            static string Describe(byte[] bytes, bool flushToggle)
            {
                var sb = new StringBuilder(8 + (bytes.Length * 3));
                sb.Append('[');
                foreach (var b in bytes)
                {
                    sb.Append(' ');
                    sb.Append(b.ToString("X2"));
                }

                sb.Append(flushToggle ? " ]! " : " ] ");
                sb.Append(bytes.Length);
                return sb.ToString();
            }

            if (ns == null)
            {
                Console.WriteLine("ERROR Can't send anything on non-connected stream & socket.", Color.Red);
                return;
            }

            ns.Write(buffer);
            if (flush)
            {
                ns.Flush();
            }

            Console.WriteLine(Describe(buffer, flush));
        }

        private static void Flush(NetworkStream ns)
        {
            if (ns == null)
            {
                Console.WriteLine("ERROR Can't flush non-connected stream & socket.", Color.Red);
                return;
            }

            ns.Flush();
        }

        private static void Help(Dictionary<string, ConsoleKey> keys, bool flush)
        {
            const string delim = @"----------------------------";
            Console.WriteLine(delim, Color.Gray);
            Console.WriteLine("A   => connect", Color.Gray);
            Console.WriteLine("B   => close", Color.Gray);
            Console.WriteLine("C   => flush toggle", Color.Gray);
            Console.WriteLine("D   => force flush", Color.Gray);
            Console.WriteLine(delim, Color.Gray);
            foreach (var name in keys.Keys)
            {
                Console.WriteLine($"{keys[name]}   => send {name}", Color.Gray);
            }

            Console.WriteLine(delim, Color.Gray);
            Console.WriteLine("Esc => exit", Color.Gray);
            Console.WriteLine(delim, Color.Gray);
            Console.WriteLine($"(auto flush: {(flush ? "ON" : "OFF")})", Color.Gray);
            Console.WriteLine();
        }

        private static Dictionary<string, ConsoleKey> AssignConsoleKeys(string[] names)
        {
            var chars = "EFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            var max = chars.Length > names.Length ? names.Length : chars.Length;
            var dict = new Dictionary<string, ConsoleKey>(max);
            for (int i = 0; i < max; i++)
            {
                var key = Enum.Parse<ConsoleKey>(chars[i].ToString());
                dict.Add(names[i], key);
            }

            return dict;
        }
    }
}
