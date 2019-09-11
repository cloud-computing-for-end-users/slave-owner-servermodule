using custom_message_based_implementation.custom_request.slave_owner_servermodule;
using custom_message_based_implementation.interfaces;
using custom_message_based_implementation.model;
using message_based_communication.encoding;
using message_based_communication.model;
using message_based_communication.module;
using System;
using System.Collections.Generic;
using System.Text;

namespace slave_owner_servermodule
{
    public class SlaveOwnerServermodule : BaseRouterModule, ISlaveOwnerServermodule
    {
        public SlaveOwnerServermodule(Port portForRegistrationToRouter, ModuleType moduleType, message_based_communication.encoding.Encoding customEncoding) : base(portForRegistrationToRouter, moduleType, customEncoding)
        {
        }

        public override string CALL_ID_PREFIX => "SLAVE_OWNER_SM_CALL_ID_";

        protected override string MODULE_ID_PREFIXES => "SLAVE_OWNER_SM_MODULE_ID_";


        public override void HandleRequest(BaseRequest message)
        {
            object responsePayload = null;

            if(message is RequestGetSlave _reqGetSlave)
            {
                responsePayload = GetSlave(_reqGetSlave.Arg2AppInfo, _reqGetSlave.Arg1PrimaryKey);
            }
            else if(message is RequestGetListOfRunningApplications _GetListOFApps)
            {
                Console.WriteLine("Recived request for GetListOfRunningApplications");
                responsePayload = GetListOfRunnableApplications();
            }
            else
            {
                throw new Exception("Reviced message that I don't know how to handle");
            }

            var response = GenerateResponseBasedOnRequestAndPayload(message, responsePayload);
            SendResponse(response);

        }


        public List<ApplicationInfo> GetListOfRunnableApplications()
        {
            //todo FIX obviously
            var list = new List<ApplicationInfo>();
            list.Add(new ApplicationInfo() {ApplicationName ="kenneths app 1", ApplicationVersion = "Version awsome", RunningOnOperatingSystem="KOS1" });
            list.Add(new ApplicationInfo() {ApplicationName ="kenneths app 2", ApplicationVersion = "Version more awsome", RunningOnOperatingSystem="KOS0" });
            list.Add(new ApplicationInfo() {ApplicationName ="kenneths app 3", ApplicationVersion = "Version most awsome", RunningOnOperatingSystem="KOS2" });

            return list;
        }

        public SlaveConnection GetSlave(ApplicationInfo appInfo, PrimaryKey primaryKey)
        {
            //TODO FIX THIS M8
            return new SlaveConnection()
            {
                IP = new IP() { TheIP = "0.0.0..0.000." },
                OwnerPrimaryKey = new PrimaryKey() { TheKey = -1 },
                Port = new Port() { ThePort=-1},
                SlaveID = new SlaveID() {ID="-1" }
            };
        }

    }
}
