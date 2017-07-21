using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cassandra;

namespace LoggingLevelConsole
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            /*
            Using ccm (https://github.com/pcmanus/ccm), create a 3 node cluster, start it and pause 1 node
            > ccm create test -n 3 -v 3.10
            > ccm start
            > ccm node3 stop
            */
            try
            {
                SimpleLoggingLevelTest();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        public static void SimpleLoggingLevelTest()
        {
            /*
            If change the log level to Error the console message:
            Host: 07/21/2017 10:16:40.885 -03:00 #WARNING: Host 127.0.0.3:9042 considered as DOWN.
            will not be shown
            */
            Diagnostics.CassandraTraceSwitch.Level = TraceLevel.Warning;

            // Add a standard .NET trace listener
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            
            const string ip = "127.0.0.1";
            var queryOptions = new QueryOptions();
            queryOptions.SetConsistencyLevel(ConsistencyLevel.All);
            using (var cluster = Cluster.Builder()
                .AddContactPoints(ip)
                .WithQueryOptions(queryOptions)
                .Build())
            {
                var cassandraSession = cluster.Connect();
                var rs = cassandraSession.Execute("select * from system.peers;");
                foreach (var row in rs)
                {
                    Console.WriteLine("{0} {1}", row["peer"], String.Join(",", (string[]) row["tokens"]));
                }
            }
        }
    }
}