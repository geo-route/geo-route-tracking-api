$content = Get-Content $args[0]
$xml = [Xml] $content
$assemblyVersion = [string] $xml.Project.PropertyGroup.AssemblyVersion
$assemblyVersion = $assemblyVersion.Trim()
$length = $assemblyVersion.Length


if($length -lt 7) {
    $version = $assemblyVersion;
} else {
    $version = $assemblyVersion.Substring(0, $length - 2)
}

echo $version
