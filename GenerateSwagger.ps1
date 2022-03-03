dotnet build task-service.sln
cp .\TaskService.Main\ProtobufClient.xml ProtobufClient.xml
dotnet swagger tofile --yaml --output doc/swagger.yaml .\TaskService.Main\bin\Debug\net6.0\TaskService.dll v1
rm .\ProtobufClient.xml

