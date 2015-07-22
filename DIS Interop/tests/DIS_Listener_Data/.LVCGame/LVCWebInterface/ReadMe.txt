The LVC Game web interface needs certain permissions to run on Windows Vista and Windows 7.
This can be achieved by turning User Access Control (UAC) off or by manually allowing 
the web interface to use a port (default is port [LVCWebInterfacePort]).

If turning UAC off is not acceptable, a small batch file called portScript.bat has been 
provided which will set access to the required default port. For the script to run 
you need to run it as administrator. That is, right click on the file and select "Run 
as administrator".

PLEASE NOTE: Should you change the port you run the web interface on in LVCWebInterface.config,
then you will need to modify the script file and re-run it with the changed port.
