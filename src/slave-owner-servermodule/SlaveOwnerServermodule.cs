﻿using custom_message_based_implementation.custom_request.slave_owner_servermodule;
using custom_message_based_implementation.interfaces;
using custom_message_based_implementation.model;
using message_based_communication.model;
using message_based_communication.module;
using System;
using System.Collections.Generic;

namespace slave_owner_servermodule
{
    public class SlaveOwnerServermodule : BaseRouterModule, ISlaveOwnerServermodule
    {
        public List<Slave> Slaves { get; set; }

        public SlaveOwnerServermodule(Port portForRegistrationToRouter, ModuleType moduleType, message_based_communication.encoding.Encoding customEncoding) : base(portForRegistrationToRouter, moduleType, customEncoding)
        {
        }

        public override string CALL_ID_PREFIX => "SLAVE_OWNER_SM_CALL_ID_";

        protected override string MODULE_ID_PREFIXES => "SLAVE_OWNER_SM_MODULE_ID_";


        public override void HandleRequest(BaseRequest message)
        {
            object responsePayload = null;

            if (message is RequestGetSlave _reqGetSlave)
            {
                Console.WriteLine("Recived request for " + nameof(RequestGetSlave) );
                responsePayload = GetSlave(_reqGetSlave.Arg2AppInfo, _reqGetSlave.Arg1PrimaryKey);
            }
            else if (message is RequestGetListOfRunningApplications _GetListOFApps)
            {
                Console.WriteLine("Received request for GetListOfRunningApplications");
                responsePayload = GetListOfRunnableApplications();
            }
            else
            {
                throw new Exception("Received message that I don't know how to handle");
            }

            var response = GenerateResponseBasedOnRequestAndPayload(message, responsePayload);
            SendResponse(response);

        }


        public List<ApplicationInfo> GetListOfRunnableApplications()
        {
            //todo FIX obviously
            var list = new List<ApplicationInfo>();

            foreach (var item in Slaves)
            {
                list.Add(new ApplicationInfo()
                {
                    ApplicationName = item.ApplicationName,
                    ApplicationVersion = item.ApplicationVersion,
                    RunningOnOperatingSystem = item.OperatingSystemName
                });
            }
            
            return list;
        }

        public Slave GetSlave(ApplicationInfo appInfo, PrimaryKey primaryKey)
        {
            return FindSlave(appInfo, primaryKey);
           
        }

        private Slave FindSlave(ApplicationInfo appInfo, PrimaryKey pk)
        {
            foreach (var slave in Slaves)
            {
                if (slave.ApplicationName.Equals(appInfo.ApplicationName) &&
                    slave.ApplicationVersion.Equals(appInfo.ApplicationVersion))
                {
                    slave.SlaveConnection.OwnerPrimaryKey = pk;
                    return slave;
                }
            }

            Console.WriteLine("No slave found, with arguments: {application info: {app name: " + appInfo.ApplicationName + ", app version: " + appInfo.ApplicationVersion + "}");
            return null;
        }

        [Obsolete]
        private SlaveConnection instanciateNewSlave(ApplicationInfo appInfo, PrimaryKey primaryKey)
        {
            // this method would be used in the case where the slave owner will actually instanciate new slaves.

            //TODO make this the right way at some point, maybe not before docket is introduced
            var slaveCommInfo = new SlaveConnection()
            {
                SlaveID = new SlaveID() { ID = "1" }
                ,
                OwnerPrimaryKey = primaryKey
                ,
                ConnectionInformation = new ConnectionInformation()
                {
                    IP = new IP() { TheIP = Program.IsLocalhost ? "127.0.0.1" : "10.152.212.6" }
                    ,
                    Port = new Port() { ThePort = 60252 } //forwards to 10142
                }
                ,
                RegistrationPort = new Port() { ThePort = 60253 } //forwards to 10143
                ,
                ConnectToRecieveImagesPort = new Port() { ThePort = 60254 } //forwards to 30303
            };
            return slaveCommInfo;
        }
    }
}
