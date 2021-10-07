dotnet build src/SFA.DAS.Apim.Developer.Database/SFA.DAS.Apim.Developer.Database.csproj

#https://docs.microsoft.com/en-us/sql/tools/sqlpackage/sqlpackage-download?view=sql-server-ver15
sudo spctl --master-disable
sqlpackage \
    /Action:Publish \
    /SourceFile:src/SFA.DAS.Apim.Developer.Database/bin/Debug/netstandard2.0/SFA.DAS.Apim.Developer.Database.dacpac \
    /TargetServerName:. \
    /TargetDatabaseName:<databasename> \
    /TargetUser:<username> \
    /TargetPassword:<password>
sudo spctl --master-enable