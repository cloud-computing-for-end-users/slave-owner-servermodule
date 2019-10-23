#########Dockerfile for Slave Owner########

FROM gaticks/bachelor-project:BaseUbuntuNetCore2.2 as base

# copy application 
COPY /src/slave-owner-servermodule/bin/Debug/netcoreapp2.2/publish/ /data/slave-owner/

ENTRYPOINT ["dotnet", "/data/slave-owner/slave-owner-servermodule.dll"]

# first three arguments are for self setup, last two are for infromation on the router the module connectes to
#currently ip to connect to host is 172.17.0.7
#currently ip to connect to container is 172.29.64.1


CMD ["scp:5532", "srp:5533", "rip:172.17.0.2", "rcp:5522", "rrp:5523", "isLocal:false"]