using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        string processName = args[0];
        int maxLifetime = int.Parse(args[1]);
        int monitoringFrequency = int.Parse(args[2]);
        bool running = true;

        while (running)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            foreach (Process process in processes)
            {
                if (process.StartTime.AddMinutes(maxLifetime) < DateTime.Now)
                {
                    using (StreamWriter sw = File.AppendText("log.txt"))
                    {
                        sw.WriteLine($"Process {processName} with PID {process.Id} exceeded maximum lifetime of {maxLifetime} minutes and was killed at {DateTime.Now}");
                    }
                    process.Kill();
                }
            }
            Thread.Sleep(monitoringFrequency * 60 * 1000);
            if (Console.ReadKey().KeyChar == 'q')
                running = false;
        }
    }
}