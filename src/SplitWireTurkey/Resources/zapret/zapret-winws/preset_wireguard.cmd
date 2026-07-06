start "zapret: wireguard" /min "%~dp0winws2.exe" ^
--wf-raw=@"%~dp0windivert.filter\windivert.wireguard.txt" ^
--filter-l7=wireguard --dpi-desync=fake
