## A quick web api template to include following features

- Default automatic Swagger UI support for Web API
- Serilog support for logs
 - Configuration for Console, File and Seq in appsettings.json
 - Log files auto roll every day
 - Logger in a separate class library
 - Dependency Injection support for all controllers
 - Verbose level support
 - HTTP Logging support

### Serilog with Seq

- Start Seq Docker container:
```
docker run -d --restart unless-stopped --name seq -e ACCEPT_EULA=Y -p 5341:80 datalust/seq:latest
```
- Uncomment following in appsettings.json:
```
  {
	"Name": "Seq",
	"Args": {
	  "serverUrl": "http://localhost:5341"
	}
  }
``` 
- open browser at http://localhost:5341.  Run ASP.NET web API application and do a quick api test using swagger.  Click refresh (arrow button) in Seq UI

TIP: check this for quick tricks to use Seq: https://github.com/datalust/seq-cheat-sheets/blob/main/src/seq-cheat-sheet.md

### Log4Net Log Levels

- Serilog log level types are different from Log4Net.  Some log viewers may not identify Serilog log level types.
- Following are the steps to translate Serilog log levels to Log4Net:
 - uncomment following line (in logging.cs):
 ```
 .Enrich.With<Log4NetLevelMapperEnricher>()
 ```
 - modify Serilog outputTemplate as needed in appsettings.json:
 ```
 "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}  {CorrelationId} {Log4NetLevel} {Message} {Properties} {NewLine}{Exception}"
 ```