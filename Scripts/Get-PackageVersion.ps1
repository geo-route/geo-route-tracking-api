$content = Get-Content $args[0] | ConvertFrom-Json
echo $content.version
