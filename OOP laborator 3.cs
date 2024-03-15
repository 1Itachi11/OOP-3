using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace SistemMonitorizareFisiere
{
    abstract class Fisier
    {
        public string Nume { get; set; }
        public DateTime DataCreare { get; set; }
        public DateTime DataModificare { get; set; }
        public abstract string ObtineInformatii();
        public abstract bool S-aSchimbat(DateTime momentSnapshot);
    }
    class FisierText : Fisier
    {
        public int NumarLinii { get; set; }
        public int NumarCuvinte { get; set; }
        public int NumarCaractere { get; set; }
        public override string ObtineInformatii()
        {
            return $"Nume: {Nume}, Număr Linii: {NumarLinii}, Număr Cuvinte: {NumarCuvinte}, Număr Caractere: {NumarCaractere}, Creat: {DataCreare}, Ultima Modificare: {DataModificare}";
        }
        public override bool S-aSchimbat(DateTime momentSnapshot)
        {
            return DataModificare > momentSnapshot;
        }
    }
    class FisierImagine : Fisier
    {
        public int Latime { get; set; }
        public int Inaltime { get; set; }
        public override string ObtineInformatii()
        {
            return $"Nume: {Nume}, Lățime: {Latime}, Înălțime: {Inaltime}, Creat: {DataCreare}, Ultima Modificare: {DataModificare}";
        }
        public override bool S-aSchimbat(DateTime momentSnapshot)
        {
            return DataModificare > momentSnapshot;
        }
    }
    class FisierProgram : Fisier
    {
        public int NumarLinii { get; set; }
        public int NumarClase { get; set; }
        public int NumarMetode { get; set; }
        public override string ObtineInformatii()
        {
            return $"Nume: {Nume}, Număr Linii: {NumarLinii}, Număr Clase: {NumarClase}, Număr Metode: {NumarMetode}, Creat: {DataCreare}, Ultima Modificare: {DataModificare}";
        }
        public override bool S-aSchimbat(DateTime momentSnapshot)
        {
            return DataModificare > momentSnapshot;
        }
    }

    class Program
    {
        private static string caleFolder = "C:\\Cale\\Catre\\Folder"; 
        private static DateTime ultimMomentSnapshot = DateTime.Now;
        static void Main(string[] args)
        {
            Thread firMonitorizare = new Thread(MonitorizeazaSchimbariFolder);
            firMonitorizare.Start();

            while (true)
            {
                Console.WriteLine("Introduceți acțiunea (commit/info <nume-fisier>/status):");
                string input = Console.ReadLine().Trim();
                string[] tokenuri = input.Split(' ');
                string actiune = tokenuri[0].ToLower();
                switch (actiune)
                {
                    case "commit":
                        ultimMomentSnapshot = DateTime.Now;
                        Console.WriteLine("Momentul snapshot-ului actualizat.");
                        break;
                    case "info":
                        if (tokenuri.Length > 1)
                        {
                            string numeFisier = tokenuri[1];
                            FileInfo fileInfo = new FileInfo(Path.Combine(caleFolder, numeFisier));
                            if (fileInfo.Exists)
                            {
                                Fisier fisier = ObtineDetaliiFisier(fileInfo);
                                Console.WriteLine(fisier.ObtineInformatii());
                            }
                            else
                            {
                                Console.WriteLine("Fișierul nu a fost găsit.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Vă rugăm să furnizați un nume de fișier.");
                        }
                        break;
                    case "status":
                        VerificaStatus();
                        break;
                    default:
                        Console.WriteLine("Acțiune invalidă.");
                        break;
                }
            }
        }
        static void MonitorizeazaSchimbariFolder()
        {
            while (true)
            {
                Thread.Sleep(5000); 
                DirectoryInfo director = new DirectoryInfo(caleFolder);
                IEnumerable<FileInfo> fisiere = director.GetFiles("*.*", SearchOption.AllDirectories);
                foreach (FileInfo fileInfo in fisiere)
                {
                    Fisier fisier = ObtineDetaliiFisier(fileInfo);
                    if (fisier.S-aSchimbat(ultimMomentSnapshot))
                    {
                        Console.WriteLine($"Schimbare detectată: {fisier.Nume}");
                    }
                }
            }
        }
        static Fisier ObtineDetaliiFisier(FileInfo fileInfo)
        {
            switch (fileInfo.Extension.ToLower())
            {
                case ".txt":
                    return new FisierText
                    {
                        Nume = fileInfo.Name,
                        DataCreare = fileInfo.CreationTime,
                        DataModificare = fileInfo.LastWriteTime,
                        NumarLinii = File.ReadLines(fileInfo.FullName).Count(),
                        NumarCuvinte = File.ReadAllText(fileInfo.FullName).Split(new char[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length,
                        NumarCaractere = File.ReadAllText(fileInfo.FullName).Length
                    };
                case ".jpg":
                case ".png":
                    return new FisierImagine
                    {
                        Nume = fileInfo.Name,
                        DataCreare = fileInfo.CreationTime,
                        DataModificare = fileInfo.LastWriteTime,
                        Latime = 1024, // Valoare fictivă pentru demonstrație
                        Inaltime = 860 //
                    };
                case ".py":
                case ".java":
                    return new FisierProgram
                    {
                        Nume = fileInfo.Name,
                        DataCreare = fileInfo.CreationTime,
                        DataModificare = fileInfo.LastWriteTime,
                        NumarLinii = File.ReadLines(fileInfo.FullName).Count(),
                        NumarClase = 0,
                        NumarMetode = 0
                    };
                default:
                    return null;
            }
        }
        static void VerificaStatus()
        {
            DirectoryInfo director = new DirectoryInfo(caleFolder);
            IEnumerable<FileInfo> fisiere = director.GetFiles("*.*", SearchOption.AllDirectories);

            foreach (FileInfo fileInfo in fisiere)
            {
                Fisier fisier = ObtineDetaliiFisier(fileInfo);
                if (fisier.S-aSchimbat(ultimMomentSnapshot))
                {
                    Console.WriteLine($"Fișierul {fisier.Nume} s-a schimbat de la ultimul snapshot.");
                }
            }
        }
    }
}
