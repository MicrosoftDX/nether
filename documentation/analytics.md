# Analytics

[to do]

## Configuration required for Azure Data Factory Pipeline

Upload the Hive script dau.hql (under deployment/analytics-assets) onto another blob storage account. Configure the following parameters in analyticsdeploy.parameters.json accordingly:
* hiveScriptFilePath, e.g. [container]/[file-path]
* hiveScriptStorageAccountName
* hiveScriptStorageAccountKey