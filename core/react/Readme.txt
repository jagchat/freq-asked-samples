Demonstrates on having React App (created through VS template) embedded in ASP.NET Core MVC View
------------------------------------------------------------------------------------------

to run and debug based on ASP.NET Core MVC Views
- comment "spa.UseReactDevelopmentServer(npmScript: "start");" in startup.cs (for ASP.NET/iis)
- run "npm install" and "npm run build" in "ClientApp" folder (for production react app)
- run and open "http://<yourserver>:<port>/Demo" (for MVC View with embedded MVC app)

to run and debug just React app (without ASP.NET Core MVC Views)
- uncomment "spa.UseReactDevelopmentServer(npmScript: "start");" (for webpack dev server)
- the above will delete "build" folder we may have created earlier (during running)
- run and open "http://<yourserver>:<port>" (serves index.html of react app from webpack dev server) 
- NOTE: IIS will still be running (in parallel) for ajax requests.  But, react app (using in-memory cache) is served through webpack dev server


