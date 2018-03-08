c:
cd\
cd "C:\Program Files (x86)\Windows Kits\10\bin\x86"
signtool sign /t http://timestamp.digicert.com /f "C:\Users\AcketK\stack\Dropbox\Programming\SVN Projects\SCM\XPagesConnector\C-Sharp\XpagesConnector\Jacobs_CodeSigning_Certificate.pfx" /p "J@cobs2016" "C:\Users\AcketK\stack\Dropbox\Programming\SVN Projects\SCM\XPagesConnector\C-Sharp\XpagesConnector\bin\Debug\XPagesConnector.dll"
signtool sign /t http://timestamp.digicert.com /f "C:\Users\AcketK\stack\Dropbox\Programming\SVN Projects\SCM\XPagesConnector\C-Sharp\XpagesConnector\Jacobs_CodeSigning_Certificate.pfx" /p "J@cobs2016" "C:\Users\AcketK\stack\Dropbox\Programming\SVN Projects\SCM\XPagesConnector\C-Sharp\XpagesConnector\bin\Release\XPagesConnector.dll"

pause
exit