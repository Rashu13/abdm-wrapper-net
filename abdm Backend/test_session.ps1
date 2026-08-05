$headers = @{
    "Content-Type" = "application/json"
    "X-CM-ID" = "sbx"
    "REQUEST-ID" = [guid]::NewGuid().ToString()
    "TIMESTAMP" = (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
}
$body = @{
    clientId = "SBXID_008903"
    clientSecret = "15dbb11f-c446-4bae-88c4-28b71470ab74"
    grantType = "client_credentials"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "https://dev.abdm.gov.in/api/hiecm/gateway/v3/sessions" -Method Post -Headers $headers -Body $body
    $response | ConvertTo-Json
} catch {
    Write-Host "Error:"
    $streamReader = [System.IO.StreamReader]::new($_.Exception.Response.GetResponseStream())
    $ErrResp = $streamReader.ReadToEnd()
    $streamReader.Close()
    Write-Host $ErrResp
}
