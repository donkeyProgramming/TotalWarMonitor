using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Session;

namespace TotalWarMonitor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var outputFilePath = "c:\\temp\\output.txt";
            if(File.Exists(outputFilePath))
            {
                Console.WriteLine("Deleting existing output file");
                File.Delete(outputFilePath);
            }
            using var writer = new StreamWriter(outputFilePath, true);
            Dictionary<ulong, string> fileLookup = new();
            Dictionary<ulong, string> fileLookup2 = new();

            //logman stop monitor_test -ets
            using (var session = new TraceEventSession("monitor_test"))
            {
                session.EnableKernelProvider(KernelTraceEventParser.Keywords.DiskFileIO |
                    KernelTraceEventParser.Keywords.DiskIO |
                                          KernelTraceEventParser.Keywords.FileIOInit |
                                          KernelTraceEventParser.Keywords.FileIO);



                session.Source.Kernel.FileIORead += (data) =>
                {
                    if (data.ProcessName == "Warhammer3")
                    {
                        if (string.IsNullOrWhiteSpace(data.FileName) == false)
                        { 
                            fileLookup2[data.FileKey] = data.FileName;
                        }

                        var res = fileLookup2.TryGetValue(data.FileKey, out var fileLookUp);

                        var fileLookUpRes = "unkown";
                        if (res)
                            fileLookUpRes = fileLookUp;

                        var str = $"[FileIORead]: [{data.TimeStamp}] FileName:{data.FileName} FileLookUp:{fileLookUpRes} FileObject:{data.FileObject} Offset:{data.Offset} Size:{data.IoSize}";
                        writer.WriteLine(str);
                        writer.Flush(); 
                        Console.WriteLine(str);
                    }
                };

                session.Source.Kernel.DiskIORead += (data) =>
                {
                    if (data.ProcessName == "Warhammer3")
                    {
                        if (string.IsNullOrWhiteSpace(data.FileName) == false)
                        {
                            fileLookup2[data.FileKey] = data.FileName;
                        }

                        var res = fileLookup2.TryGetValue(data.FileKey, out var fileLookUp);

                        var fileLookUpRes = "unkown";
                        if (res)
                            fileLookUpRes = fileLookUp;

                        var str = $"[DiskIORead]: [{data.TimeStamp}] FileName:{data.FileName} FileLookUp:{fileLookUpRes} Offset:{data.ByteOffset} Size:{data.TransferSize}";
                        writer.WriteLine(str);
                        writer.Flush();
                        Console.WriteLine(str);
                    }
                };
                session.Source.Kernel.FileIOQueryInfo += (data) =>
                {
                    if (data.ProcessName == "Warhammer3")
                    {
                        var res = fileLookup.TryGetValue(data.FileObject, out var fileLookUp);

                        var fileLookUpRes = "unkown";
                        if (res)
                            fileLookUpRes = fileLookUp;



                        var str = $"[FileIOQueryInfo]: [{data.TimeStamp}] FileName:{data.FileName} FileLookUp:{fileLookUpRes} FileObject:{data.FileObject}";
                        writer.WriteLine(str);
                        writer.Flush();
                        Console.WriteLine(str);
                    }
                };
          
                session.Source.Kernel.FileIODirEnum += (data) =>
                {
                    if (data.ProcessName == "Warhammer3")
                    {
                        var res = fileLookup.TryGetValue(data.FileObject, out var fileLookUp);

                        var fileLookUpRes = "unkown";
                        if (res)
                            fileLookUpRes = fileLookUp;

                        var str = $"[FileIODirEnum]: [{data.TimeStamp}] FileName:{data.FileName} FileLookUp:{fileLookUpRes} FileObject:{data.FileObject} Directory:{data.DirectoryName}";
                        writer.WriteLine(str);
                        writer.Flush();
                        Console.WriteLine(str);
                    }
                };

                session.Source.Kernel.FileIOCreate += (data) =>
               {
                   if (data.ProcessName == "Warhammer3")
                   {
                       var str = $"[FileIOCreate]: [{data.TimeStamp}] FileName:{data.FileName} FileObject:{data.FileObject} Disposition:{data.CreateDisposition} FileAttribute:{data.FileAttributes}";
                        writer.WriteLine(str);
                        writer.Flush(); // Ensure data is written to file
                       Console.WriteLine(str);

                       fileLookup[data.FileObject] = data.FileName;


                       if (data.FileName == @"C:\users\ole_k\appdata\roaming\the creative assembly\warhammer3\maps\db\mercenary_pool_modifiers_tables\")
                        {
                        }

                        if (data.FileName.Contains(".pack", StringComparison.InvariantCultureIgnoreCase))
                        { }

                   }
               };

                Console.WriteLine("Running - start Total war");
                session.Source.Process();
            }
        }
    }
}




/*
   session.EnableKernelProvider(KernelTraceEventParser.Keywords.FileIO);
    session.Source.Kernel.FileIOSetInformation += data =>
            {
                Console.WriteLine($"SetFilePointer - PID: {data.ProcessID}, FileObject: {data.FileObject}, New Offset: {data.Offset:X}");
            };

 FileIORead	Triggered when a process reads from a file.
FileIOWrite	Triggered when a process writes to a file.
FileIODelete	Triggered when a file is deleted.
FileIOCreate	Triggered when a file is created.
FileIOClose	Triggered when a file handle is closed.
FileIOFlush	Triggered when a file buffer is flushed to disk.
FileIORename	Triggered when a file is renamed.
FileIOSetInformation	Triggered when file attributes or pointer positions change.
 
 * 
 *             // Enable Exception Provider
            session.EnableKernelProvider(KernelTraceEventParser.Keywords.Exception);

            // Subscribe to Exception Events
            session.Source.Kernel.Exception += data =>
            {
                Console.WriteLine($"Exception: {data.ExceptionCode:X} in PID: {data.ProcessID} " +
                                  $"({data.ProcessName}) at Address: {data.ExceptionAddress:X}");
            };
 * 
 * 
 *  session.Source.Kernel.FileIOFail += data =>
            {
                if (data.Status == 0x3) // ERROR_PATH_NOT_FOUND
                {
                    Console.WriteLine($"Directory Not Found: {data.FileName} in PID: {data.ProcessID}");
                }
            };
 * 
 session.EnableProvider("Microsoft-Windows-ConsoleHost");

            session.Source.AllEvents += data =>
            {
                if (data.EventName == "ConsoleWriteEvent")
                {
                    Console.WriteLine($"Console Output Detected: {data.PayloadString(0)}");
                }
            };

 */
