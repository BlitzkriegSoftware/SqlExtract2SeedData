<#
    Update VERIFICATION.txt
#>

[string]$VFILE="C:\code\blitz\SqlExtract2SeedData\SqlExtract2SeedData\Choco\SqlExtract2SeedData\tools\VERIFICATION.txt"
[string]$NUPKG="C:\code\blitz\SqlExtract2SeedData\SqlExtract2SeedData\Choco\SqlExtract2SeedData\tools\SqlExtract2SeedData.exe"
[string]$PDBPK="C:\code\blitz\SqlExtract2SeedData\SqlExtract2SeedData\Choco\SqlExtract2SeedData\tools\SqlExtract2SeedData.pdb"

[string[]]$files = @($NUPKG,$PDBPK)

[string]$TEMP="c:\temp\t.txt"
[string]$WORK="c:\temp\w.txt"

[string[]]$ALGO='CRC32','CRC64','SHA256','SHA1','BLAKE2sp'

"VERIFICATION`n"  | out-file -Encoding utf8 $WORK 

foreach($fl in $files) {

    foreach ($a in $ALGO) {
  
        $t = ( 7z h "-scrc${a}" "${fl}" )
        $t | Out-File -Encoding utf8 $TEMP

        if ( $a -eq $ALGO[0] ) {
            $b="Name"
            [string]$line=Split-Path $fl -leaf
            "${b}: ${line}" | out-file -Encoding utf8 -Append $WORK  
        
            $b="Size"
            [string]$line=Get-Content $TEMP | Select-String $b
            $index=$line.IndexOf(':')
            $line=$line.SubString($index + 1)
            $line=$line.Trim()
            [long]$kib=[Math]::Round([convert]::ToInt64($line, 10) / 1024, 0)
            "${b}: ${line} bytes (${kib} KiB)" | out-file -Encoding utf8 -Append $WORK  
        }

        $b='for data:'
        [string]$line=Get-Content $TEMP | Select-String $b
        $index=$line.IndexOf(':')
        $line=$line.SubString($index + 1)
        $line=$line.Trim()
        "${a}: ${line}" | out-file -Encoding utf8 -Append $WORK
    }

    "" | out-file -Encoding utf8 -Append $WORK
}

Copy-Item -Path $WORK -Destination $VFILE -Force

Write-Output "Updated: ${VFILE}"