using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


public class Debug
{
    private readonly static string SaveFile = @$"{AppDomain.CurrentDomain.BaseDirectory}\Log\{DateTime.Now.ToFileTime()}.txt";
    private readonly static string SaveFileLocation = @$"{AppDomain.CurrentDomain.BaseDirectory}\Log";

    public static void Log(string log)
    {
        Directory.CreateDirectory(SaveFileLocation);
        File.AppendAllTextAsync(SaveFile, log + "\n");
    }
}

