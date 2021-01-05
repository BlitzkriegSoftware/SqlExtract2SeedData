<#
    Update VERIFICATION.txt
#>

[string]$VFILE="C:\code\blitz\SqlExtract2SeedData\SqlExtract2SeedData\Choco\SqlExtract2SeedData\tools\VERIFICATION.txt"
[string]$NUPKG="C:\code\blitz\SqlExtract2SeedData\SqlExtract2SeedData\bin\Debug\SqlExtract2SeedData.1.1.2.nupkg"

[string]$TEMP="c:\temp\t.txt"
[string]$WORK="c:\temp\w.txt"

[string[]]$ALGO='CRC32','CRC64','SHA256','SHA1','BLAKE2sp'

"VERIFICATION`n"  | out-file -Encoding utf8bom $WORK 

foreach ($a in $ALGO) {
  
    $t = ( 7z h "-scrc${a}" "${NUPKG}" )
    $t | Out-File -Encoding utf8bom $TEMP

    if ( $a -eq $ALGO[0] ) {
        $b="Name"
        [string]$line=Split-Path $NUPKG -leaf
        "${b}: ${line}" | out-file -Encoding utf8bom -Append $WORK  
        
        $b="Size"
        [string]$line=Get-Content $TEMP | Select-String $b
        $index=$line.IndexOf(':')
        $line=$line.SubString($index + 1)
        $line=$line.Trim()
        [long]$kib=[Math]::Round([convert]::ToInt64($line, 10) / 1024, 0, 2)
        "${b}: ${line} bytes (${kib} KiB)" | out-file -Encoding utf8bom -Append $WORK  
    }

    $b='for data:'
    [string]$line=Get-Content $TEMP | Select-String $b
    $index=$line.IndexOf(':')
    $line=$line.SubString($index + 1)
    $line=$line.Trim()
    "${a}: ${line}" | out-file -Encoding utf8bom -Append $WORK
}

Copy-Item -Path $WORK -Destination $VFILE -Force

Write-Output "Updated: ${VFILE}"