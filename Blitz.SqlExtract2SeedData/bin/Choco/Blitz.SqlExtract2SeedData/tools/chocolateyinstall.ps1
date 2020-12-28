$packageName = 'blitz.sqlextract2seeddata'
$url = 'https://github.com/BlitzkriegSoftware/SqlExtract2SeedData/releases/download/v1.1.2/BlitzSqlExtract2SeedData.exe'
$checksum="24EE4CCB81C71AF4C52C1882A203BA831971F330E192B4410C5983FC2445A3C9"
$fileType = 'exe'
$silentArgs = '/SILENT'
$validExitCodes = @(0)

Install-ChocolateyPackage "packageName" "$fileType" "$silentArgs" "$url"  -validExitCodes  $validExitCodes -checksum $checksum -checksumType "sha256"