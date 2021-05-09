using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;

namespace PokemonBattleOnline.Game
{
    public static class UserData
    {
        private static string FileName;

        public static void Load(string fileName)
        {
            FileName = fileName;
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    var serializer = new DataContractSerializer(typeof(PokemonTeam[]));
                    Current = (PokemonTeam[])serializer.ReadObject(XmlReader.Create(fs, new XmlReaderSettings()
                    {
                        ConformanceLevel = ConformanceLevel.Fragment,
                        CloseInput = false,
                        IgnoreWhitespace = false
                    }));
                }
            }
            catch
            {
                Current = Enumerable.Empty<PokemonTeam>();
            }
        }

        public static IEnumerable<PokemonTeam> Current
        { get; private set; }

        public static PokemonTeam ImportTeam(string source)
        {
            var pms = new PokemonData[6];
            Xxporter.Import(source, pms);
            return new PokemonTeam(pms);
        }
        public static PokemonData ImportPokemon(string source)
        {
            var target = new PokemonData[1];
            Xxporter.Import(source, target);
            return target[0];
        }
        public static string Export(PokemonData pm)
        {
            var sb = new StringBuilder();
            Xxporter.Export(sb, pm);
            return sb.ToString();
        }
        public static string Export(IEnumerable<PokemonData> pms)
        {
            var sb = new StringBuilder();
            bool first = true;
            foreach (var pm in pms)
            {
                if (first) first = false;
                else
                {
                    sb.AppendLine();
                    sb.AppendLine();
                }
                Xxporter.Export(sb, pm);
            }
            return sb.ToString();
        }

        public static void Save(IEnumerable<PokemonTeam> teams)
        {
            Current = teams.ToArray();
            var dir = Path.GetDirectoryName(FileName);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            using (var f = new FileStream(FileName, FileMode.Create, FileAccess.Write))
            {
                var writer = XmlWriter.Create(f, new XmlWriterSettings()
                {
                    CloseOutput = false,
                    ConformanceLevel = ConformanceLevel.Fragment,
                    NamespaceHandling = NamespaceHandling.OmitDuplicates,
                    Indent = false,
                    OmitXmlDeclaration = true
                });
                var serializer = new DataContractSerializer(Current.GetType());
                serializer.WriteStartObject(writer, Current);
                writer.WriteAttributeString("xmlns", PBOMarks.MS, null, PBOMarks.STANDARD);
                writer.WriteAttributeString("xmlns", PBOMarks.A, null, PBOMarks.ARRAY);
                serializer.WriteObjectContent(writer, Current);
                serializer.WriteEndObject(writer);
                writer.Flush();
            }
        }
    }
}
