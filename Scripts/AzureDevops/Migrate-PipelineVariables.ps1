

$organisation = "https://dev.azure.com/<your-organisation>"
$project = "<your-project-name>"


# Ado-PipelineVarsList -PipelineName "pipeline-sample"
# az pipelines variable list --pipeline-name $pipelineName --organization $organisation --project $project --output json
function Ado-PipelineVarsList {
    param(
        [Parameter(Mandatory = $true)]
        [string]$PipelineName
    )
        $result = az pipelines variable list `
            --pipeline-name $PipelineName `
            --organization $organisation `
            --project $project `
            --output json
        
    $variables = $result | ConvertFrom-Json 
    
    $variableHash = [ordered]@{}

    foreach ($property in ($variables.PSObject.Properties)) {                
        if ($property.value.isSecret -eq $true) {
            $variableHash[$property.name] = "SECRET"
        } else {
            $variableHash[$property.name] = $property.value.value
        }
    }
            
    return $variableHash
}


# Ado-VariableGroupItemExists 2 "RESOURCE_GROUP_NAME_DEV"
# az pipelines variable-group variable list --group-id $targetGroupId --organization $organisation --project $project --query "contains(keys(@), 'RESOURCE_GROUP_NAME_DEV')" --output json
function Ado-VariableGroupItemExists {
    param(
        [Parameter(Mandatory = $true)]
        [string]$TargetGroupId,
        [Parameter(Mandatory = $true)]
        [string]$VariableName
    )
        $result = az pipelines variable-group variable list `
            --group-id $TargetGroupId `
            --organization $organisation `
            --project $project `
            --query "contains(keys(@), '$VariableName')" `
            --output json
        
    $containsVariable = $result | ConvertFrom-Json
            
    return $containsVariable
}


# Ado-VariableGroupAddItem 2 "Test" "Value"
# az pipelines variable-group variable create --group-id 2 --name "Test" --value "Value" --organization $organisation --project $project --output json
function Ado-VariableGroupAddItem {
    param(
        [Parameter(Mandatory = $true)]
        [string]$TargetGroupId,
        [Parameter(Mandatory = $true)]
        [string]$VariableName,
        [Parameter(Mandatory = $true)]
        [string]$VariableValue
    )
        $result = az pipelines variable-group variable create `
            --group-id $targetGroupId `
            --name $VariableName `
            --value $VariableValue `
            --organization $organisation `
            --project $project `
            --output json
        
    $uploadResult = $result | ConvertFrom-Json
            
    return $uploadResult
}


function Ado-MigrateVars() {
    param(
        [Parameter(Mandatory = $true)]
        [string]$PipelineName,
        [Parameter(Mandatory = $true)]
        [string]$VariableGroupId
    )

    $pipelineVars = Ado-PipelineVarsList -PipelineName "pipeline-sample" 

    foreach ($varName in $pipelineVars.Keys) {
    
        $varValue = $pipelineVars[$varName]
    
        $exists = Ado-VariableGroupItemExists -TargetGroupId $VariableGroupId -VariableName $varName
    
        if ($exists) {
            Write-Host "$varName - Already exists in variable group" -ForegroundColor DarkGray
        }
        else {
           
            # Ask user if they want to upload this variable
            Write-Host "Would you like to upload '$varName' with value '$varValue' to the variable group? (y/n)" -ForegroundColor Yellow -NoNewline
            $response = Read-Host " "            
        
            if ($response -eq "y") {
                # Upload the variable
                Ado-VariableGroupAddItem -TargetGroupId $VariableGroupId -VariableName $varName -VariableValue $varValue
                Write-Host "$varName - UPLOADED!" -ForegroundColor Yellow
            }
            else {
                Write-Host "$varName - SKIPPED!"  -ForegroundColor DarkGray
            }
        }
    }
}
