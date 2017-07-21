using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cassandra;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;

namespace LoggingLevel
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            /*
            Using ccm, create a 3 node cluster, start it and pause 1 node
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
            Diagnostics.CassandraTraceSwitch.Level = TraceLevel.Warning;

            // Add a standard .NET trace listener
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            
            const string ip = "127.0.0.1";
            var queryOptions = new QueryOptions();
            queryOptions.SetConsistencyLevel(ConsistencyLevel.All);
            var socketOptions = new SocketOptions();
            socketOptions.SetConnectTimeoutMillis(1000);
            using (var cluster = Cluster.Builder()
                .AddContactPoints(ip)
                .WithQueryOptions(queryOptions)
                .WithSocketOptions(socketOptions)
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