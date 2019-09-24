@SET demo_domain=desharp-demo-basic.local
@SET demo_port=80
@SET localhost_ip=127.0.0.1
@SET site_path=%cd%




@CD %windir%\system32\inetsrv\
@Appcmd add site /name:%demo_domain% /physicalPath:%site_path% /bindings:http/*:%demo_port%:%demo_domain%



@ECHO: >> %windir%\System32\Drivers\Etc\Hosts
@ECHO: >> %windir%\System32\Drivers\Etc\Hosts
@ECHO %localhost_ip% %demo_domain% >> %windir%\System32\Drivers\Etc\Hosts
@ECHO: >> %windir%\System32\Drivers\Etc\Hosts
@ipconfig/flushdns



@ECHO:
@ECHO If you don't see any error, open your browser to see demo app on: 
@ECHO http://%demo_domain%:%demo_port%/
@ECHO:




@PAUSE