$ErrorActionPreference = "Continue"
$base = "http://localhost:5000/api"
$results = New-Object System.Collections.Generic.List[object]

function Add-Result($name, $ok, $detail) {
    $script:results.Add([pscustomobject]@{ Test = $name; Status = if ($ok) { "OK" } else { "FAIL" }; Detail = $detail })
    if ($ok) { Write-Host "[OK] $name" -ForegroundColor Green }
    else { Write-Host "[FAIL] $name - $detail" -ForegroundColor Red }
}

function Api($method, $uri, $body, $headers) {
    $params = @{ Uri = $uri; Method = $method; Headers = $headers; UseBasicParsing = $true }
    if ($body) { $params.ContentType = "application/json"; $params.Body = $body }
    $r = Invoke-WebRequest @params
    return $r.Content | ConvertFrom-Json
}

function ApiExpectFail($method, $uri, $body, $headers, $expectedCode) {
    try {
        $params = @{ Uri = $uri; Method = $method; Headers = $headers; UseBasicParsing = $true }
        if ($body) { $params.ContentType = "application/json"; $params.Body = $body }
        Invoke-WebRequest @params | Out-Null
        throw "Expected HTTP $expectedCode but succeeded"
    } catch {
        $code = [int]$_.Exception.Response.StatusCode
        if ($code -ne $expectedCode) { throw "Expected $expectedCode got $code" }
    }
}

$email = "autotest$(Get-Random)@podm.test"
$pass = "Test1234!"
$h = @{}
$state = $null
$eventId = $null
$choiceId = $null
$parkId = $null

Write-Host ""
Write-Host "=== People of Dark Mind - API tests ===" -ForegroundColor Cyan
Write-Host ""

try {
    $r = Api POST "$base/auth/register" (@{ email = $email; password = $pass; displayName = "AutoTest" } | ConvertTo-Json) @{}
    $h = @{ Authorization = "Bearer $($r.token)" }
    Add-Result "POST /auth/register" $true $null
} catch { Add-Result "POST /auth/register" $false $_.Exception.Message }

try {
    $r = Api POST "$base/auth/login" (@{ email = $email; password = $pass } | ConvertTo-Json) @{}
    Add-Result "POST /auth/login" ($null -ne $r.token) $null
} catch { Add-Result "POST /auth/login" $false $_.Exception.Message }

try {
    $r = Invoke-WebRequest -Uri "$base/auth/me" -Headers $h -UseBasicParsing
    Add-Result "GET /auth/me" ($r.StatusCode -eq 200) $null
} catch { Add-Result "GET /auth/me" $false $_.Exception.Message }

try {
    $r = Api POST "$base/game/new" '{"characterName":"Tester"}' $h
    Add-Result "POST /game/new" ($null -ne $r.id) $null
} catch { Add-Result "POST /game/new" $false $_.Exception.Message }

try {
    $state = Api GET "$base/game/state" $null $h
    Add-Result "GET /game/state" ($null -ne $state.character) $null
} catch { Add-Result "GET /game/state" $false $_.Exception.Message }

try {
    $r = Api GET "$base/game/character" $null $h
    Add-Result "GET /game/character" ($r.name -eq "Tester") $null
} catch { Add-Result "GET /game/character" $false $_.Exception.Message }

try {
    $locs = Api GET "$base/game/locations" $null $h
    $parkId = ($locs | Where-Object { $_.code -eq "park" }).id
    Add-Result "GET /game/locations" ($locs.Count -ge 5) $null
} catch { Add-Result "GET /game/locations" $false $_.Exception.Message }

try {
    $r = Api GET "$base/game/npcs" $null $h
    Add-Result "GET /game/npcs" ($r.Count -ge 3) $null
} catch { Add-Result "GET /game/npcs" $false $_.Exception.Message }

try {
    Api GET "$base/game/evidence" $null $h | Out-Null
    Add-Result "GET /game/evidence" $true $null
} catch { Add-Result "GET /game/evidence" $false $_.Exception.Message }

try {
    $r = Api GET "$base/game/journal" $null $h
    Add-Result "GET /game/journal" ($r.Count -ge 1) $null
} catch { Add-Result "GET /game/journal" $false $_.Exception.Message }

try {
    Api GET "$base/game/saves" $null $h | Out-Null
    Add-Result "GET /game/saves" $true $null
} catch { Add-Result "GET /game/saves" $false $_.Exception.Message }

try {
    $state = Api POST "$base/game/action" '{"actionType":"Rest"}' $h
    Add-Result "POST /game/action Rest" ($state.character.stats.energy -ge 90) $null
} catch { Add-Result "POST /game/action Rest" $false $_.Exception.Message }

try {
    $state = Api POST "$base/game/random-event" $null $h
    $eventId = $state.currentEvent.id
    $choiceId = $state.currentEvent.choices[0].id
    Add-Result "POST /game/random-event" ($null -ne $eventId) $null
} catch { Add-Result "POST /game/random-event" $false $_.Exception.Message }

try {
    $body = (@{ eventId = $eventId; choiceId = $choiceId } | ConvertTo-Json)
    $state = Api POST "$base/game/choice" $body $h
    $ok = ($null -eq $state.currentEvent) -and ($state.character.storyProgress -ge 1)
    Add-Result "POST /game/choice" $ok $null
} catch { Add-Result "POST /game/choice" $false $_.Exception.Message }

try {
    ApiExpectFail POST "$base/game/choice" (@{ eventId = $eventId; choiceId = $choiceId } | ConvertTo-Json) $h 400
    Add-Result "POST /game/choice duplicate 400" $true $null
} catch { Add-Result "POST /game/choice duplicate 400" $false $_.Exception.Message }

try {
    $rnd = Api POST "$base/game/random-event" $null $h
    if (-not $rnd.currentEvent) { throw "random-event did not start" }
    ApiExpectFail POST "$base/game/action" '{"actionType":"Rest"}' $h 400
    $body = (@{ eventId = $rnd.currentEvent.id; choiceId = $rnd.currentEvent.choices[0].id } | ConvertTo-Json)
    Api POST "$base/game/choice" $body $h | Out-Null
    Add-Result "POST /game/action blocked when event" $true $null
} catch { Add-Result "POST /game/action blocked when event" $false $_.Exception.Message }

try {
    $state = Api POST "$base/game/travel/$parkId" $null $h
    Add-Result "POST /game/travel" ($state.character.currentLocation.code -eq "park") $null
} catch { Add-Result "POST /game/travel" $false $_.Exception.Message }

try {
    ApiExpectFail POST "$base/game/new" '{"characterName":"X"}' $h 400
    Add-Result "POST /game/new duplicate 400" $true $null
} catch { Add-Result "POST /game/new duplicate 400" $false $_.Exception.Message }

Write-Host ""
Write-Host "=== Summary ===" -ForegroundColor Cyan
$results | Format-Table -AutoSize
$failed = @($results | Where-Object { $_.Status -eq "FAIL" }).Count
$ok = @($results | Where-Object { $_.Status -eq "OK" }).Count
Write-Host "Passed: $ok / $($results.Count)"
if ($failed -gt 0) { exit 1 }
