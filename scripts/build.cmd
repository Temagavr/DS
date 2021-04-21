cd ../Common
start /wait dotnet build

cd ../Valuator
start /wait dotnet build

cd ../RankCalculator
start /wait dotnet build

cd ../EventLogger
start /wait dotnet build

exit