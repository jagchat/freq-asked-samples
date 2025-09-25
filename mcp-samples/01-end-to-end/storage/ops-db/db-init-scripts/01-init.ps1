#NOTE: USED AS PART OF OTHER SCRIPTS
$red = "`e[31m"
$noColor = "`e[0m"

$dbScriptsPath = "./storage/ops-db/scripts"
$schemaScriptsPath = Join-Path -Path $dbScriptsPath -ChildPath "schema-scripts"
$dataScriptsPath = Join-Path -Path $dbScriptsPath -ChildPath "data-scripts"

# Set environment variable for password
[System.Environment]::SetEnvironmentVariable("PGPASSWORD", $password, [System.EnvironmentVariableTarget]::Process)

#----------------------------- Zap
Write-Host "Zap: Started.." -ForegroundColor Cyan

& "psql" -U $user -h $server -p $port -d $dbName -c "DROP SCHEMA demo_ops CASCADE"
if ($LASTEXITCODE -ne 0) {
    Write-Host "${red}Error: Something went wrong!${noColor}"
}

& "psql" -U $user -h $server -p $port -d $dbName -c "DROP FUNCTION sync_lastmod"
if ($LASTEXITCODE -ne 0) {
    Write-Host "${red}Error: Something went wrong!${noColor}"
}

#----------------------------- System Db Objects
Write-Host "System Db Objects: Started.." -ForegroundColor Cyan
$sysDbObjectsPath = Join-Path -Path $schemaScriptsPath -ChildPath "000-sys-db-objects"

$sqlFile = Join-Path -Path $sysDbObjectsPath -ChildPath "0000-trgr-sync_lastmod.sql"
& "psql" -U $user -h $server -p $port -d $dbName -f $sqlFile
if ($LASTEXITCODE -ne 0) {
    Write-Host "${red}Error: Something went wrong!${noColor}"
}

#----------------------------- demo_ops Schema
Write-Host "demo_ops Schema: Started.." -ForegroundColor Cyan
$schemaDemoOpsSchemaPath = Join-Path -Path $schemaScriptsPath -ChildPath "001-demo_ops"

# Get all SQL files in the folder, sorted by filename
$sqlFiles = Get-ChildItem -Path $schemaDemoOpsSchemaPath -Filter "*.sql" | Sort-Object Name

# Loop through each file
foreach ($file in $sqlFiles) {
    # Execute the SQL file
    & "psql" -U $user -h $server -p $port -d $dbName -f $file.FullName
    
    # Check if the execution was successful
    if ($LASTEXITCODE -ne 0) {
        Write-Host "${red}Error: Something went wrong with file: $($file.Name)!${noColor}"
    } else {
        Write-Host "${green}Successfully executed: $($file.Name)${noColor}"
    }
}

#----------------------------- demo_ops Data
Write-Host "demo_ops Data: Started.." -ForegroundColor Cyan
$schemaDemoOpsDataPath = $dataScriptsPath 

# Get all SQL files in the folder, sorted by filename
$sqlFiles = Get-ChildItem -Path $dataScriptsPath -Filter "*.sql" | Sort-Object Name

# Loop through each file
foreach ($file in $sqlFiles) {
    # Execute the SQL file
    & "psql" -U $user -h $server -p $port -d $dbName -f $file.FullName
    
    # Check if the execution was successful
    if ($LASTEXITCODE -ne 0) {
        Write-Host "${red}Error: Something went wrong with file: $($file.Name)!${noColor}"
    } else {
        Write-Host "${green}Successfully executed: $($file.Name)${noColor}"
    }
}

# Unset the environment variable after execution
[System.Environment]::SetEnvironmentVariable("PGPASSWORD", $null, [System.EnvironmentVariableTarget]::Process)