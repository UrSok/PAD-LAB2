echo Starting MovieApi 2
cd MovieAPI\bin\Debug\netcoreapp3.1
start MovieApi.exe "MovieApi 1"

echo Starting MovieApi 1
cd .. 
cd .. 
cd .. 
cd ..
cd MovieAPI\bin\Release\netcoreapp3.1
start MovieApi.exe "MovieApi 2"

echo Starting Proxy
cd .. 
cd .. 
cd .. 
cd ..
cd Proxy\bin\Debug\netcoreapp3.1
start Proxy.exe

echo Starting SyncNode
cd .. 
cd .. 
cd .. 
cd ..
cd SyncNode\bin\Debug\netcoreapp3.1
start SyncNode.exe