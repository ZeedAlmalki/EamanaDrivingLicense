using Microsoft.Win32;
using System.Diagnostics;
using System;
using System.Runtime.CompilerServices;

public class SharedDVLD
{
    private static string LoggingSourceName = "DVLD";
    public static string RegisteryPath = @"HKEY_CURRENT_USER\SOFTWARE\DVLD";

    public static void ConfiguredSystemToAcceptLogging()
    {
        try
        {
            if (Registry.GetValue(RegisteryPath, "EventSourceCreated", null) as string == "1")
                return;

            if (!EventLog.SourceExists(LoggingSourceName))
            {
                EventLog.CreateEventSource(LoggingSourceName, "Application");
            }
            Registry.SetValue(RegisteryPath, "EventSourceCreated", "1", RegistryValueKind.String);
        }
        catch (UnauthorizedAccessException ex)
        {
            
        }
    }

    public static void RegisterLogInEventHandler(Exception ex, [CallerMemberName] string FunctionName = "")
    {
        ConfiguredSystemToAcceptLogging();
        try
        {
            EventLog.WriteEntry(LoggingSourceName, FunctionName + ": " + ex.Message, EventLogEntryType.Error);
        }
        catch (Exception ex2)
        {
            Console.WriteLine(ex2.Message);
        }
    }
}