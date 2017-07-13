using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
//using System.Linq;//net framework 3.5

namespace XlsxToLua
{
    public class DirectoryAllFiles
    {
        static List<FileInformation> FileList = new List<FileInformation>();
        public static List<FileInformation> GetAllFiles(DirectoryInfo dir)
        {
            FileInfo[] allFile = dir.GetFiles();
            foreach (FileInfo fi in allFile)
            {
                FileList.Add(new FileInformation { FileName = fi.Name, FilePath = fi.FullName });
            }
            DirectoryInfo[] allDir = dir.GetDirectories();
            foreach (DirectoryInfo d in allDir)
            {
                GetAllFiles(d);
            }
            return FileList;
        }

        public static List<FileInformation> GetAllFiles(DirectoryInfo dir,string str)
        {
            FileInfo[] allFile = dir.GetFiles(str);
            foreach (FileInfo fi in allFile)
            {
                FileList.Add(new FileInformation { FileName = fi.Name, FilePath = fi.FullName });
            }
            DirectoryInfo[] allDir = dir.GetDirectories();
            foreach (DirectoryInfo d in allDir)
            {
                GetAllFiles(d,str);
            }
            return FileList;
        }
    }
}
