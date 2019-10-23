using System;
using custom_message_based_implementation.model;
using message_based_communication.model;

namespace GeneratingSlaveConnectionJSON
{
    class Program
    {
        static void Main(string[] args)
        {
            var slave = new Slave()
            {
                SlaveConnection = new SlaveConnection()
                {
                    ConnectToRecieveImagesPort = new Port() { ThePort = 60254 }
                    ,
                    ConnectionInformation = new ConnectionInformation()
                    {
                        Port = new Port() { ThePort = 60252 }
                        ,
                        IP = new IP() { TheIP = "127.0.0.1" } // if of the computer (kenneth) that runs the server
                    }
                    ,
                    RegistrationPort = new Port() { ThePort = 60253 }
                }
                ,
                ApplicationName = "Word"
                ,
                OperatingSystemName = "Windows 10"
                
            };
            Console.WriteLine(slave.ToJSON());
            Console.WriteLine("Converted JSON back to slave: " + Slave.FromJSON(slave.ToJSON()));
            Console.ReadKey();
        }


        private static string GenerateJsonString(SlaveConnection conn)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(conn);
        }
    }
}
