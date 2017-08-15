#
# This script adds tab completion for IdentityTestClient.exe in PowerShell
#
# NOTES: 
#       - this has a dependency on TabExpansionPlusPlus (https://www.powershellgallery.com/packages/TabExpansionPlusPlus)
#       - this requires that you have the exe. To build this run the steps below which will create .\bin\Release\netcoreapp1.1\win10-x64\IdentityTestClient.exe
#           - `dotnet build --configuration Release`
#           - `dotnet publish --configuration Release --runtime win10-x64`
#
# Ensure that TabExpansionPlusPlus is loaded (`Import-Module TabExpansionPlusPlus`)
# load this script `. completion.ps1`
#

function IdentityTestClientCompletion {
    param($wordToComplete, $commandAst)

    $commandTree = Get-CompletionPrivateData -Key IdentityClientTestExeCompletionCommandTree
    if ($null -eq $commandTree) {
        Set-Alias -Name nct -Value New-CommandTree

        $commandTree = & {
            # TODO --root-url switch
            # This breaks the completion if uncommented!
            # nct -Argument '--root-url' ''
            nct client-creds "client credential flow" {
                nct -Argument '--client-id' 'client id'
                nct -Argument '--client-secret' 'client secret'
            }
            nct facebook-token "facebook user access token flow" {
                nct -Argument '--client-id' 'client id'
                nct -Argument '--client-secret' 'client secret'
                nct -Argument '--facebook-token' 'facebook user access token'
            }
            nct facebook-token-raw "facebook user access token flow (raw HttpClient)" {
                nct -Argument '--client-id' 'client id'
                nct -Argument '--client-secret' 'client secret'
                nct -Argument '--facebook-token' 'facebook user access token'
            }
            nct guest "guest flow" {
                nct -Argument '--client-id' 'client id'
                nct -Argument '--client-secret' 'client secret'
                nct -Argument '--guest-id' 'guest identifier'
            }
            nct guest-raw "guest flow (raw HttpClient)" {
                nct -Argument '--client-id' 'client id'
                nct -Argument '--client-secret' 'client secret'
                nct -Argument '--guest-id' 'guest identifier'
            }
            nct resource-owner "resource owner (username + password) flow"  {
                nct -Argument '--client-id' 'client id'
                nct -Argument '--client-secret' 'client secret'
                nct -Argument '--username' 'username'
                nct -Argument '--password' 'password'
            }
            nct guest-to-facebook "guest flow to facebook" {
                nct -Argument '--client-id' 'client id'
                nct -Argument '--client-secret' 'client secret'
                nct -Argument '--guest-id' 'guest identifier'
                nct -Argument '--facebook-token' 'facebook user access token'
            }
            nct guest-to-resource-owner "guest flow to resource owner (username + password)" {
                nct -Argument '--client-id' 'client id'
                nct -Argument '--client-secret' 'client secret'
                nct -Argument '--guest-id' 'guest identifier'
                nct -Argument '--username' 'username'
                nct -Argument '--password' 'password'
            }
        }

        Set-CompletionPrivateData -Key IdentityClientTestExeCompletionCommandTree -Value $commandTree
    }

    Get-CommandTreeCompletion $wordToComplete $commandAst $commandTree
}


Register-ArgumentCompleter `
    -Command 'IdentityTestClient' `
    -Native `
    -Description 'Complete arguments to IdentityTestClient.exe' `
    -ScriptBlock $function:IdentityTestClientCompletion