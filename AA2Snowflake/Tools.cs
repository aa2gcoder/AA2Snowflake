﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SB3Utility;
using AA2Install;
using Microsoft.VisualBasic.FileIO;
using System.Drawing;
using Paloma;

namespace AA2Snowflake
{
    public static class Tools
    {
        public static Encoding ShiftJIS => Encoding.GetEncoding(932);

        public static MemoryStream GetStreamFromSubfile(IWriteFile iw)
        {
            MemoryStream mem = new MemoryStream();
            iw.WriteTo(mem);
            mem.Position = 0;
            return mem;
        }

        public static MemoryStream GetStreamFromFile(string file)
        {
            MemoryStream mem = new MemoryStream();
            using (FileStream b = new FileStream(file, FileMode.Open))
                b.CopyTo(mem);
            mem.Position = 0;
            return mem;
        }

        public static void RefreshPPs()
        {
            PP.jg2e00_00_00 = new ppParser(Paths.AA2Edit + @"\jg2e00_00_00.pp", new ppFormat_AA2());
            PP.jg2e01_00_00 = new ppParser(Paths.AA2Edit + @"\jg2e01_00_00.pp", new ppFormat_AA2());
            PP.jg2e06_00_00 = new ppParser(Paths.AA2Edit + @"\jg2e06_00_00.pp", new ppFormat_AA2());
            PP.jg2p01_00_00 = new ppParser(Paths.AA2Play + @"\jg2p01_00_00.pp", new ppFormat_AA2());
        }

        public static void BackupFile(string file)
        {
            string name = file.Remove(0, file.LastIndexOf('\\'));
            FileSystem.CopyFile(file, Paths.BACKUP + name, UIOption.AllDialogs);
        }

        public static void RestoreFile(string file)
        {
            string name = file.Remove(0, file.LastIndexOf('\\'));
            FileSystem.CopyFile(Paths.BACKUP + name, file, UIOption.AllDialogs);
            RefreshPPs();
        }

        public static void DeleteBackup(string file)
        {
            string name = file.Remove(0, file.LastIndexOf('\\'));
            FileSystem.DeleteFile(Paths.BACKUP + name, UIOption.AllDialogs, RecycleOption.DeletePermanently);
        }

        public static byte[] GetBytesFromStream(Stream str)
        {
            str.Position = 0;
            byte[] buffer = new byte[str.Length];
            str.Read(buffer, 0, (int)str.Length);
            return buffer;
        }

        public static MemSubfile ManipulateLst(IWriteFile lst, int column, string replacement)
        {
            byte[] buffer;
            using (MemoryStream mem = GetStreamFromSubfile(lst))
                buffer = GetBytesFromStream(mem);

            string slst = ShiftJIS.GetString(buffer);

            StringBuilder str = new StringBuilder();
            foreach (string line in slst.Split(new string[] { "\r\n" }, StringSplitOptions.None))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] set = line.Split('\t');
                set[column - 1] = replacement;

                str.Append(set.Aggregate((i, j) => i + "\t" + j) + "\r\n");
            }

            return new MemSubfile(new MemoryStream(ShiftJIS.GetBytes(str.ToString())), lst.Name);
        }

#warning i really should merge lst reading/manipulation with aa2data
        public static string GetLstValue(IWriteFile lst, int column)
        {
            byte[] buffer;
            using (MemoryStream mem = GetStreamFromSubfile(lst))
                buffer = GetBytesFromStream(mem);

            string slst = ShiftJIS.GetString(buffer);
            string line = slst.Split(new string[] { "\r\n" }, StringSplitOptions.None)[0];
            string[] set = line.Split('\t');
            return set[column - 1];
        }

        public static Bitmap LoadTGA(Stream stream)
        {
            Bitmap bit;
            using (TargaImage t = new TargaImage(stream))
            using (Bitmap b = t.Image)
                bit = (Bitmap)t.Image.Clone();

            return bit;
        }

        public static Bitmap LoadTGA(string filename)
        {
            Bitmap bit;
            using (TargaImage t = new TargaImage(filename))
            using (Bitmap b = t.Image)
                bit = (Bitmap)t.Image.Clone();

            return bit;
        }

        public static string GetFilename(this string path, char seperator = '\\')
        {
            return path.Remove(0, path.LastIndexOf(seperator) + 1);
        }

        public static string RemoveFilename(this string path, char seperator = '\\')
        {
            return path.Remove(path.LastIndexOf(seperator));
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? value : defaultValue;
        }

        public static int IndexOfKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            for (int index = 0; index < dictionary.Count; index++)
                if (Equals(dictionary.ElementAt(index).Key, key))
                    return index;

            return -1;
        }

        public const double radian = 180 / Math.PI;
        public static float RadiansToDegrees(this float radians)
        {
            return (float)(radians * radian);
        }

        public static float DegreesToRadians(this float degrees)
        {
            return (float)(degrees / radian);
        }

        public static IEnumerable<int> PatternAt(byte[] source, byte[] pattern)
        {
            for (int i = 0; i < source.Length; i++)
            {
                if (source.Skip(i).Take(pattern.Length).SequenceEqual(pattern))
                {
                    yield return i;
                }
            }
        }
        public static byte[] ToByteArray(this Stream str)
        {
            str.Position = 0;
            byte[] buffer = new byte[str.Length];
            str.Read(buffer, 0, (int)str.Length);
            return buffer;
        }
    }

    public static class PP
    {
        public static ppParser jg2e00_00_00 = new ppParser(Paths.AA2Edit + @"\jg2e00_00_00.pp", new ppFormat_AA2());
        public static ppParser jg2e01_00_00 = new ppParser(Paths.AA2Edit + @"\jg2e01_00_00.pp", new ppFormat_AA2());
        public static ppParser jg2e06_00_00 = new ppParser(Paths.AA2Edit + @"\jg2e06_00_00.pp", new ppFormat_AA2());
        public static ppParser jg2p01_00_00 = new ppParser(Paths.AA2Play + @"\jg2p01_00_00.pp", new ppFormat_AA2());
    }
}
