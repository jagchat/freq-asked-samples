### Providing config to app

- Make use of standard IConfiguration interface through out app
- Reading configuration key/value from app.settings
- Adding more (custom) key/value in Startup
- Adding/modifying more (custom) key/value on top of app.settings (using custom config. provider)
 - Re-use values from app.settings while adding more keys using custom config. provider
- Add more config on top of above all using custom / hierarchical JSON (fetched from sql/http source) 
 - while reusing config from app.settings

### Leveraging config

- Leverage (default) DI everywhere in order to work with config
 - In Startup
 - Controller / Action
 - Views
 - in custom classes 
 - in classes available in external assemblies 
   - directly referenced
   - dynamically loaded 
   - using reflection
   - using base interface
 - Load assembly based on config and still leverage IConfiguration in dynamically loaded assembly

