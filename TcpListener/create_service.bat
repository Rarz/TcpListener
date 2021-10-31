sc.exe create "TcpListenerService" binPath="E:\Projects\projects\Csharp\TcpListener\TcpListener\bin\Release\net5.0\publish\TcpListener.exe" start=auto
sc.exe description TcpListenerService "Listens to a specific port and shuts down the pc when given the correct command"
sc.exe start TcpListenerService