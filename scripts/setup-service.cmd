cd ..
dotnet restore
dotnet publish -c Release -o ./publish/bin/StakeTradingBot
net stop StakeTradingBot
taskkill /F /IM mmc.exe
sc.exe delete StakeTradingBot
sc.exe create StakeTradingBot binpath= %CD%\publish\bin\StakeTradingBot\StakeTradingBot.exe start=auto
net start StakeTradingBot