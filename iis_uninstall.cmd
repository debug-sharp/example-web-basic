@SET demo_domain=desharp-demo-basic.local
@SET localhost_ip=127.0.0.1
@SET site_path=%cd%




@CD %windir%\system32\inetsrv\
@Appcmd delete site %demo_domain%



@SET hosts_fullpath=%windir%\System32\Drivers\Etc\Hosts
@powershell -Command "(gc %hosts_fullpath%) -replace '%localhost_ip% %demo_domain%', '' | Out-File %hosts_fullpath%" > NUL



@ECHO:
@ECHO If you don't see any error, demo app has been uninstalled: 
@ECHO http://%demo_domain%:%demo_port%/
@ECHO:




@PAUSE