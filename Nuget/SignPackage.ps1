$nupkg = (get-childitem .\package\*.nupkg | sort-object -Descending -Property LastWriteTime).FullName;
Write-Host "$nupkg";

# .\signtool.exe sign /v /n "West Wind Technologies" /sm /s MY /tr "http://timestamp.digicert.com" /td SHA256 /fd SHA256 "$nupkg"

Nuget.exe sign "$nupkg" -TimeStamper "http://timestamp.digicert.com" `
                        -CertificateStoreName "My" `
                        -CertificateSubjectName "West Wind Technologies"
