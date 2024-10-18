using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace EasySaveV2.app.viewModel
{
    class TestViewModel
    {
        /// <summary>
        /// Test of getHashOfFile()
        /// </summary>
        /// <returns>bool depend of the result of the test</returns>
        static public bool testGetHashOfFile()
        {
            string test1 = UtilsController.getHashOfFile(File.OpenRead(@"D:/ez.txt"));
            if (test1 == "b17ec9630d5eb858fef11e77d1c2330efba85698".ToUpper())
            {
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Test of listFolders()
        /// </summary>
        static public void testListFolders()
        {
            Directory.CreateDirectory(@"D:/test/test1Dir");
            List<string> filestotransfer = new List<string>();
            UtilsController.listFolders(@"D:/test", ref filestotransfer);
        }

        /// <summary>
        /// Test of listFiles()
        /// </summary>
        static public void testListFiles()
        {
            File.Create(@"D:/test/test1");
            List<string> filestotransfer = new List<string>();
            UtilsController.listFiles(@"D:/test", ref filestotransfer);
        }

        /// <summary>
        /// Test of stringListNotInList
        /// </summary>
        /// <returns>bool depend of the result of the test</returns>
        static public bool testStringListNotInList()
        {
            List<string> source = new List<string>();
            List<string> destination = new List<string>();
            List<string> result = new List<string>();
            source.Add("a.txt");
            source.Add("b.txt");

            destination.Add("a.txt");
            destination.Add("c.txt");
            destination.Add("d.txt");

            result.Add("c.txt");
            result.Add("d.txt");

            List<string> reelResults = UtilsController.stringListNotInList(source, destination);

            if (result.Count != reelResults.Count)
                return false;
            for (int i = 0; i < result.Count; i++)
                if (!result[i].Equals(reelResults[i]))
                    return false;
            return true;

        }

        static public void testSortPrioritaryFiles()
        {
            List<string> files = new List<string>();
            UtilsController.listFiles(@"F:\temp\src", ref files);
            Trace.WriteLine("Without Sort :");

            foreach (string file in files)
            {
                Trace.WriteLine(file);
            }

            files = UtilsController.sortPrioritaryFiles(files);
            Trace.WriteLine("With Sort :");

            foreach (string file in files)
            {
                Trace.WriteLine(file);
            }

        }

    }

}
