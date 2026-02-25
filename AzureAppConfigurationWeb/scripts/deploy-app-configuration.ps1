param (
    [Parameter(Mandatory)]$env, 
    [Parameter(Mandatory)]$configurationName, 
    [Parameter(Mandatory)]$appConfigFilePath
)

write-host "Env: $env"
write-host "ConfigurationName: $configurationName"
write-host "Config folder path: $appConfigFilePath"

 $defaultConfigPath = "$appConfigFilePath/config.default.json"

if ((Test-Path $defaultConfigPath) -eq $true) {
    write-host "Importing default config file at $defaultConfigPath"
    az appconfig kv import --name $configurationName --source file --path $defaultConfigPath --format json --content-type "application/json" --yes
}
else {
    write-host "Skipping default import. File not found: $defaultConfigPath"
}

$envConfigPath = "$appConfigFilePath/config.$env.json"

write-host "Environment config path: $envConfigPath"

if ((Test-Path $envConfigPath) -eq $true) {
	az appconfig kv import --name $configurationName --source file --path $envConfigPath --format json --content-type "application/json" --yes
}
