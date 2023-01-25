using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using NUnit.Framework;

[TestFixture]
class ProgramTests
{
    [Test]
    public void TestProcessKilledAfterExceedingMaxLifetime()
    {
        // Arrange
        string processName = "notepad";
        int maxLifetime = 1;
        int monitoringFrequency = 1;

        // Start a new notepad process
        Process.Start(new ProcessStartInfo("notepad.exe"));

        // Wait for the process to exceed the maximum lifetime
        Thread.Sleep((maxLifetime + 1) * 60 * 1000);

        // Act
        Program.Main(new string[] { processName, maxLifetime.ToString(), monitoringFrequency.ToString() });

        // Assert
        Process[] processes = Process.GetProcessesByName(processName);
        Assert.AreEqual(0, processes.Length);
        Assert.IsTrue(File.Exists("log.txt"));
        string log = File.ReadAllText("log.txt");
        Assert.IsTrue(log.Contains("exceeded maximum lifetime"));

        // Cleanup
        File.Delete("log.txt");
    }

    [Test]
    public void TestProcessNotKilledBeforeExceedingMaxLifetime()
    {
        // Arrange
        string processName = "notepad";
        int maxLifetime = 2;
        int monitoringFrequency = 1;

        // Start a new notepad process
        Process.Start(new ProcessStartInfo("notepad.exe"));

        // Wait for the process to not exceed the maximum lifetime
        Thread.Sleep((maxLifetime - 1) * 60 * 1000);

        // Act
        Program.Main(new string[] { processName, maxLifetime.ToString(), monitoringFrequency.ToString() });

        // Assert
        Process[] processes = Process.GetProcessesByName(processName);
        Assert.AreEqual(1, processes.Length);
        Assert.IsFalse(File.Exists("log.txt"));
    }

    [Test]
    public void TestNoProcessFound()
    {
        // Arrange
        string processName = "nonexistentprocess";
        int maxLifetime = 1;
        int monitoringFrequency = 1;

        // Act
        Program.Main(new string[] { processName, maxLifetime.ToString(), monitoringFrequency.ToString() });

        // Assert
        Process[] processes = Process.GetProcessesByName(processName);
        Assert.AreEqual(0, processes.Length);
        Assert.IsFalse(File.Exists("log.txt"));
    }
}
