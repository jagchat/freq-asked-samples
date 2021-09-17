# Durable Task Framework Samples

> - SQL Server must be made available for 01, 02, 03, 04 samples
> - If Docker is installed, execute powershell scripts (Docker\start-db-step-01, Docker\start-db-step-02)

### Samples Info

* 01 - Sample
    * Basic Orchestration with Single Task Activity
        * Activity accepts a number and returns square of it
* 02 - Sample
    * Orchestration with Single Task Activity
        * Activity accepts array of numbers and returns sum of those
* 03 - Sample
    * Orchestration with Two Task Activities
        * First Activity generates few numbers
        * Second Activity accepts an array of numbers and returns sum of those
        * logging enabled using "appsettings.json"
        * logging NOT enabled in orchestration / activity
* 04 - Sample
    * Same as 03
    * Refactored for DI
    * enabled NLog
    * logging enabled everywhere including orchestration / activity

