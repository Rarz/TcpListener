# TcpListener
I was too lazy to get out of bed and shut down another pc and didn't want to bother with a remote session, so threw together a simple Windows Service program to listen to a tcp port and shut down the pc when it gets the correct keyphrase. 

* Dotnet 5, uses the BackgroundWorker template.
* There's a batch file to register the service and delete the service. Make sure you correct the path to the executable file before using the file.
* Don't forget to update your Windows firewall to allow access to the port you configured.
* I trigger it using the ncat tool from Nmap: `echo my_command_code | ncat PC-NAME 12345` for example.
