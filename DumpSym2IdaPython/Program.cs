using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CommandLine;

namespace DumpSymIdaPython
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Parser.Default
                .ParseArguments<Options>(args)
                .WithParsed(Action);
        }

        private static void Action(Options options)
        {
            var symbols = ReadSourceSymbols(options.Source);

            var map = CreateSymbolsMap(symbols);

            var list = CreateSymbolsList(map, options.Prefix);

            WriteSymbolsList(list, options.Target, options.Sorted);
        }

        private static List<Symbol> ReadSourceSymbols(string path)
        {
            var symbols = new List<Symbol>();

            var text = File.ReadAllText(path);

            var matches = Regex.Matches(text, @"\$(\w{8})\s2\s(\w+)");

            foreach (Match match in matches)
            {
                var value1 = match.Groups[1].Value;
                var value2 = match.Groups[2].Value;
                var symbol = new Symbol(uint.Parse(value1, NumberStyles.HexNumber), value2);
                symbols.Add(symbol);
            }

            return symbols;
        }

        private static Dictionary<string, List<uint>> CreateSymbolsMap(List<Symbol> source)
        {
            var dictionary = new Dictionary<string, List<uint>>();

            foreach (var symbol in source)
            {
                var name = symbol.Name;
                var address = symbol.Address;

                if (dictionary.ContainsKey(name))
                {
                    dictionary[name].Add(address);
                }
                else
                {
                    dictionary.Add(name, new List<uint> {address});
                }
            }

            return dictionary;
        }

        private static List<Symbol> CreateSymbolsList(Dictionary<string, List<uint>> map, string prefix)
        {
            var symbols = new List<Symbol>();

            foreach (var (name, addresses) in map)
            {
                if (addresses.Count > 1)
                {
                    symbols.AddRange(addresses.Select((s, t) => new Symbol(s, $"{prefix}{name}_{t}")));
                }
                else
                {
                    symbols.Add(new Symbol(addresses.Single(), $"{prefix}{name}"));
                }
            }

            return symbols;
        }

        private static void WriteSymbolsList(List<Symbol> list, string path, bool sorted)
        {
            using var writer = path != null ? new StreamWriter(File.Create(path)) : Console.Out;

            writer.WriteLine("import idc");
            writer.WriteLine("import datetime");
            writer.WriteLine("print(\"DumpSymIda is about to set symbol names...\")");
            writer.WriteLine("print(datetime.datetime.now())");

            var array = sorted ? list.OrderBy(s => s.Address).ToList() : list;

            foreach (var symbol in array)
            {
                writer.WriteLine($"idc.set_name(0x{symbol.Address:X8}, \"{symbol.Name}\");");
            }
        }
    }

    internal sealed class Options
    {
        [Option('i', "source", Required = true, HelpText = "Source file containing DUMPSYM output.")]
        public string Source { get; set; }

        [Option('o', "target", Required = false, HelpText = "Target file to write IDA script to, if not set, outputs to console.")]
        public string Target { get; set; }

        [Option('p', "prefix", Required = false, HelpText = "Prefix to prepend to symbol name.")]
        public string Prefix { get; set; }

        [Option('s', "sorted", Required = false, HelpText = "Order symbols by their address.")]
        public bool Sorted { get; set; }
    }

    internal readonly struct Symbol
    {
        public uint Address { get; }

        public string Name { get; }

        public Symbol(uint address, string name)
        {
            Address = address;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public override string ToString()
        {
            return $"0x{Address:X8}, {Name}";
        }
    }
}