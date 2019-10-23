using custom_message_based_implementation.consts;
using custom_message_based_implementation.encoding;
using custom_message_based_implementation.model;
using message_based_communication.model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using NetMQ;
using NLog;

namespace slave_owner_servermodule
{
    public class Program
    {
        public static bool IsLocalhost = true;

        private const string SELF_IP = "sip";
        private const string SELF_COMM_PORT = "scp";
        private const string SELF_REG_PORT = "srp";
        private const string ROUTER_IP = "rip";
        private const string ROUTER_COMM_PORT = "rcp";
        private const string ROUTER_REG_PORT = "rrp";
        private const string IS_LOCALHOST = "isLocal";


        static void Main(string[] args)
        {
            Console.WriteLine("Slave owner servermodule is starting...");

            checked
            {
                try
                {
                    List<Slave> slaves = new List<Slave>();


                    SetupNLog();
                    //default netowrk settings
                    var portToListenForRegistration = new Port() { ThePort = 5533 };
                    var portToRegisterOn = new Port() { ThePort = 5523 };

                    var router_conn_info = new ConnectionInformation()
                    {
                        IP = new IP() { TheIP = null },
                        Port = new Port() { ThePort = 5522 }
                    };

                    var self_conn_info = new ConnectionInformation()
                    {
                        IP = new IP() { TheIP = null }, // is set after the reading of the system args because isLocalhost might change
                        Port = new Port() { ThePort = 5532 } // todo port stuff
                    };

                    //setting network infromation with sys args
                    int counter = 0;
                    while( counter < args.Length)
                    {
                        var arg = args[counter];

                        if (arg.StartsWith("'{") || arg.StartsWith("{")) // this is not fool proof but it should work.
                        {
                            var slave = Slave.FromJSON(arg);
                            slaves.Add(slave);
                            Console.WriteLine("Adding slave: " + slave.ToJSON());
                            counter++;
                            continue;
                        }



                        var split = arg.Split(":");
                        if (2 != split.Length)
                        {
                            throw new ArgumentException("Got badly formatted system arguments");
                        }


                        if (split[0].Equals(SELF_IP)) // set self ip
                        {
                            self_conn_info.IP.TheIP = split[1];
                            Console.WriteLine("Overriding self ip with: " + split[1]);
                        }
                        else if (split[0].Equals(SELF_COMM_PORT)) // set self communication port
                        {
                            self_conn_info.Port.ThePort = Convert.ToInt32(split[1]);
                            Console.WriteLine("Overriding self communication port with: " + split[1]);
                        }
                        else if (split[0].Equals(SELF_REG_PORT)) // set self registration port
                        {
                            portToListenForRegistration.ThePort = Convert.ToInt32(split[1]);
                            Console.WriteLine("Overriding register to self port with: " + split[1]);
                        }
                        else if (split[0].Equals(ROUTER_IP)) // set router ip
                        {
                            router_conn_info.IP.TheIP = split[1];
                            Console.WriteLine("Overriding router ip with: " + split[1]);
                        }
                        else if (split[0].Equals(ROUTER_COMM_PORT)) // set router communication port
                        {
                            router_conn_info.Port.ThePort = Convert.ToInt32(split[1]);
                            Console.WriteLine("Overriding router communication port with: " + split[1]);
                        }
                        else if (split[0].Equals(ROUTER_REG_PORT)) // set router registration port
                        {
                            portToRegisterOn.ThePort = Convert.ToInt32(split[1]);
                            Console.WriteLine("Overriding router registration port with: " + split[1]);
                        }
                        else if (split[0].Equals(IS_LOCALHOST))
                        {
                            Console.WriteLine("Overwriting is local host with:" + split[1]);

                            if (split[1].Equals("true", StringComparison.InvariantCultureIgnoreCase))
                            {
                                IsLocalhost = true;
                            }
                            else if (split[1].Equals("false", StringComparison.InvariantCultureIgnoreCase))
                            {
                                IsLocalhost = false;
                            }
                            else
                            {
                                throw new Exception("ERROR IN SYSTEM ARGUMENTS");
                            }
                        }

                        counter++;
                    }

                    if (null == self_conn_info.IP.TheIP)
                    {
                        self_conn_info.IP.TheIP = IsLocalhost ? "127.0.0.1" : SlaveOwnerServermodule.GetIP();
                    }

                    if (null == router_conn_info.IP.TheIP)
                    {
                        router_conn_info.IP.TheIP = IsLocalhost ? "127.0.0.1" : "fill in ip";
                    }
                    

                    if (0 == slaves.Count) // if no slaves added through sys arg add this dummy slave for getting 1 app to how in the GetListOfApplications
                    {
                        slaves.Add(new Slave()
                        {
                            SlaveConnection = null,
                            ApplicationName = "DEFAULT",
                            OperatingSystemName = "DEFAULT OS",
                            ApplicationVersion = "VERSION DEFAULT"
                        });
                    }

                    

                    Console.WriteLine(
                        "\n\n Using the following network parameters: \n self network info: \n{ IP: " + self_conn_info.IP.TheIP + ", comm_port: " + self_conn_info.Port.ThePort + ", reg_port: " + portToListenForRegistration.ThePort + " }"
                        + "\n and router network infromation: \n{ IP: " + router_conn_info.IP.TheIP + ", comm_port: " + router_conn_info.Port.ThePort + ", reg_port: " + portToRegisterOn.ThePort + "}"
                        + "\n\n");

                    var slaveOwner = new SlaveOwnerServermodule(portToListenForRegistration, new ModuleType() { TypeID = ModuleTypeConst.MODULE_TYPE_SLAVE_OWNER }, new CustomEncoder());
                    //setting slaves on the slave owner
                    slaveOwner.Slaves = slaves;

                    slaveOwner.Setup(router_conn_info, portToRegisterOn, self_conn_info, new CustomEncoder());

                    Console.WriteLine(
                        "\n\nSlave Owner Servermodule has started successfully");

                    //test
                    ////var list = slaveOwner.GetListOfRunnableApplications();
                    ////var encoded = CustomEncoder.EncodeResponse(new Response()
                    ////{
                    ////    CallID = null,
                    ////    Payload = new Payload() { ThePayload = new List<ApplicationInfo>()},
                    ////    SenderModuleID = null,
                    ////    TargetModuleID = null
                    ////});

                    ////var decoded = new CustomEncoder().DecodeIntoSendable(encoded); ;
                    ////var response = decoded as Response;

                    Console.WriteLine("Putting main thread to sleep in a loop"); // used because read key can't be used when running in docker
                    while (true)
                    {
                        Thread.Sleep(1000);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("The slave owner Servermoudle encountered an exception while in the setup process: " + ex.Message);
                }
            }
        }
        private static void SetupNLog()
        {
            var config = new NLog.Config.LoggingConfiguration();
            var logFile = "slave-owner-module-log.txt";

            /*
            var rootFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            if (File.Exists(Path.Combine(rootFolder, logFile)))
            {  
                File.Delete(Path.Combine(rootFolder, logFile));
            }
            */

            // Targets where to log to: File and Console
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = logFile };

            // Rules for mapping loggers to targets            
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            // Apply config           
            LogManager.Configuration = config;
        }

    }
}
