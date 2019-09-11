using custom_message_based_implementation.consts;
using custom_message_based_implementation.encoding;
using custom_message_based_implementation.model;
using message_based_communication.model;
using System;
using System.Collections.Generic;

namespace slave_owner_servermodule
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Slave owner servermodule is starting...");

            checked
            {
                try
                {
                    var portToListenForRegistration = new Port() { ThePort = 5533 };

                    var server_module_connection_informtion = new ConnectionInformation()
                    {
                        IP = new IP() { TheIP = "127.0.0.1" },
                        Port = new Port() { ThePort = 5522 }
                    };

                    var slave_owner_module_conn_info = new ConnectionInformation()
                    {
                        IP = new IP() { TheIP = "127.0.0.1" },
                        Port = new Port() { ThePort = 5532 }
                    };

                    var slaveOwner = new SlaveOwnerServermodule(portToListenForRegistration, new ModuleType() { TypeID = ModuleTypeConst.MODULE_TYPE_SLAVE_OWNER }, new CustomEncoder());
                    slaveOwner.Setup(server_module_connection_informtion, new Port() { ThePort = 5523 }, slave_owner_module_conn_info, new CustomEncoder());

                    Console.WriteLine("The Slave Owner Servermodule have started successfully");

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


                }
                catch (Exception ex)
                {
                    Console.WriteLine("The slave owner Servermoudle encountered an exception while in the setup process: " + ex.Message);
                }

                Console.ReadKey();
            }
        }
    }
}
