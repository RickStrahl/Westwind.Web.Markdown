Copy-Item ..\Westwind.Web.Markdown\bin\Release\Westwind.Web.Markdown.dll .\Package\lib\net40\
Copy-Item ..\Westwind.Web.Markdown\bin\Release\Westwind.Web.Markdown.xml .\Package\lib\net40\


signtool.exe sign /v /n "West Wind Technologies" /sm /s MY /tr "http://timestamp.digicert.com" /td SHA256 /fd SHA256 ".\Package\lib\net40\Westwind.Web.Markdown.dll"

# Nuget.exe sign ".\Package\lib\net40\Westwind.Web.Markdown.dll" -TimeStamper "http://timestamp.digicert.com" `
#                         -CertificateStoreName "My" `
#                         -CertificateSubjectName "West Wind Technologies"