c:
cd\
cd "C:\Program Files (x86)\Windows Kits\10\bin\10.0.17134.0\x86"
signtool sign /t http://timestamp.digicert.com /f "C:\Users\Acketk\source\repos\VSTS\XPagesAPI\XPagesAPI\Jacobs_CodeSigning_Certificate.pfx" /p "J@cobs2016" "C:\Users\Acketk\source\repos\VSTS\XPagesAPI\XPagesAPI\bin\Debug\XPagesAPI.dll"
signtool sign /t http://timestamp.digicert.com /f "C:\Users\Acketk\source\repos\VSTS\XPagesAPI\XPagesAPI\Jacobs_CodeSigning_Certificate.pfx" /p "J@cobs2016" "C:\Users\Acketk\source\repos\VSTS\XPagesAPI\XPagesAPI\bin\Release\XPagesAPI.dll"

pause
exit