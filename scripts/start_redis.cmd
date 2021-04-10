setx DB_RUS "localhost:6000"
setx DB_EU "localhost:6001"
setx DB_OTHER "localhost:6002"

cd "E:\Redis-x64-3.2.100"
start redis-server
start redis-server --port 6000
start redis-server --port 6001
start redis-server --port 6002

rem start redis-cli -p 6000
rem start redis-cli -p 6001
rem start redis-cli -p 6002