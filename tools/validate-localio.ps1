<#
.SYNOPSIS
    Smoke test / regression check liviano para Localio.
    Detecta errores recurrentes en landing y demos privadas.

.DESCRIPTION
    Descarga el HTML de cada URL configurada y valida:
      - Status HTTP 200
      - Ausencia de caracteres rotos / mojibake
      - Ausencia de placeholders visibles
      - noindex + nofollow en demos privadas
      - Firma de Localio presente y no duplicada
      - Logo horizontal referenciado en la landing
      - Favicon referenciado en la landing
      - Ausencia de secciones hero o de servicios duplicadas (por clase CSS)

.PARAMETER BaseUrl
    Reemplaza el origen de todas las URLs. Util para validar contra localhost.
    Ej: -BaseUrl "http://localhost:5000"

.PARAMETER SkipTls
    Omite la validacion de certificado TLS. Util para localhost sin HTTPS.

.EXAMPLE
    powershell tools/validate-localio.ps1
    powershell tools/validate-localio.ps1 -BaseUrl "http://localhost:5202" -SkipTls

.NOTES
    Requiere: Windows PowerShell 5.1 o PowerShell 7+.
    No modifica ningun archivo. Solo lectura y reporte.
    No reemplaza revision visual ni tests de integracion.
#>

[CmdletBinding()]
param(
    [string]$BaseUrl = "",
    [switch]$SkipTls
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Write-OK   { param($msg) Write-Host "  [OK]    $msg" -ForegroundColor Green  }
function Write-FAIL { param($msg) Write-Host "  [FAIL]  $msg" -ForegroundColor Red    }
function Write-WARN { param($msg) Write-Host "  [WARN]  $msg" -ForegroundColor Yellow }
function Write-HEAD { param($msg) Write-Host "`n$msg"         -ForegroundColor Cyan   }

$Pages = @(
    @{ Url = "https://localio.com.ar/";              Label = "Landing principal";  IsLanding = $true;  IsDemo = $false }
    @{ Url = "https://localio.com.ar/demos/citivet"; Label = "Demo Citivet";       IsLanding = $false; IsDemo = $true  }
    @{ Url = "https://localio.com.ar/demos/urquiza"; Label = "Demo Urquiza";       IsLanding = $false; IsDemo = $true  }
    @{ Url = "https://urquiza.localio.com.ar";       Label = "Subdominio Urquiza"; IsLanding = $false; IsDemo = $true  }
)

if ($BaseUrl -ne "") {
    $BaseUrl = $BaseUrl.TrimEnd("/")
    $Pages = $Pages | ForEach-Object {
        $entry  = $_
        $uri    = [System.Uri]$entry.Url
        $entry.Url = $BaseUrl + $uri.PathAndQuery
        $entry
    }
}

$MojibakeItems = @(
    @{ Pattern = "Ã";         Label = "mojibake A~ (doble-encoding UTF-8/Latin-1)"  }
    @{ Pattern = "Â";         Label = "mojibake A^ (byte 0xC2 mal decodificado)"    }
    @{ Pattern = [char]0xFFFD; Label = "caracter de reemplazo U+FFFD"               }
    @{ Pattern = "&Atilde;";  Label = "entidad HTML &Atilde; (encoding incorrecto)"  }
    @{ Pattern = "&Acirc;";   Label = "entidad HTML &Acirc; (encoding incorrecto)"   }
)

# Textos que no deben aparecer como contenido visible en el HTML.
# Se buscan como contenido de nodo (entre > y <) para evitar falsos positivos
# con atributos HTML como placeholder="..." o clases CSS.
# Los patrones completos de frases largas se buscan directamente (mas especificos).
$PlaceholderTexts = @(
    @{ Text = "foto pendiente";                    AsFullPhrase = $false }
    @{ Text = "imagen pendiente";                  AsFullPhrase = $false }
    @{ Text = "Foto principal";                    AsFullPhrase = $false }
    @{ Text = "Foto mascota";                      AsFullPhrase = $false }
    @{ Text = "Foto detalle";                      AsFullPhrase = $false }
    @{ Text = "Un espacio cercano para tu mascota"; AsFullPhrase = $true  }
)

$DemoSectionChecks = @(
    @{ Class = "pd-hero";            Max = 1; Label = "hero (pd-hero)"                         }
    @{ Class = "pd-services-grid";   Max = 1; Label = "grilla de servicios (pd-services-grid)" }
    @{ Class = "pd-footer__credits"; Max = 1; Label = "firma Localio (pd-footer__credits)"     }
)

function Get-PageHtml {
    param([string]$Url, [bool]$SkipCert)
    try {
        if ($SkipCert) {
            # PowerShell 6+ / 7+
            if ($PSVersionTable.PSVersion.Major -ge 6) {
                $response = Invoke-WebRequest -Uri $Url -UseBasicParsing -TimeoutSec 30 -SkipCertificateCheck -UserAgent "LocalioValidator/1.0"
            }
            else {
                # Windows PowerShell 5.1: deshabilitar validacion TLS globalmente para esta sesion
                [System.Net.ServicePointManager]::ServerCertificateValidationCallback = { $true }
                $response = Invoke-WebRequest -Uri $Url -UseBasicParsing -TimeoutSec 30 -UserAgent "LocalioValidator/1.0"
                [System.Net.ServicePointManager]::ServerCertificateValidationCallback = $null
            }
        }
        else {
            $response = Invoke-WebRequest -Uri $Url -UseBasicParsing -TimeoutSec 30 -UserAgent "LocalioValidator/1.0"
        }
        return @{ Ok = $true; Status = [int]$response.StatusCode; Html = $response.Content }
    }
    catch {
        $status = 0
        if ($_.Exception.Response) { $status = [int]$_.Exception.Response.StatusCode }
        return @{ Ok = $false; Status = $status; Html = ""; Error = $_.Exception.Message }
    }
}

$totalErrors = 0

foreach ($page in $Pages) {
    Write-HEAD "-- $($page.Label)  [$($page.Url)]"

    $result = Get-PageHtml -Url $page.Url -SkipCert $SkipTls.IsPresent

    if (-not $result.Ok) {
        Write-FAIL "No se pudo conectar: $($result.Error)"
        $totalErrors++
        continue
    }

    if ($result.Status -eq 200) {
        Write-OK "HTTP $($result.Status)"
    }
    else {
        Write-FAIL "HTTP $($result.Status) -- se esperaba 200"
        $totalErrors++
        if ([string]::IsNullOrWhiteSpace($result.Html)) { continue }
    }

    $html = $result.Html

    foreach ($item in $MojibakeItems) {
        $count = ([regex]::Matches($html, [regex]::Escape($item.Pattern))).Count
        if ($count -gt 0) {
            $label = $item.Label
            Write-FAIL "Mojibake ($count): $label"
            $totalErrors++
        }
    }

    foreach ($ph in $PlaceholderTexts) {
        $t = $ph.Text
        if ($ph.AsFullPhrase) {
            $found = $html -match [regex]::Escape($t)
        }
        else {
            # Busca el texto entre etiquetas para evitar falsos positivos con atributos HTML
            $found = $html -match ('>[^<]*' + [regex]::Escape($t) + '[^<]*<')
        }
        if ($found) {
            Write-FAIL "Placeholder visible encontrado: '$t'"
            $totalErrors++
        }
    }

    if ($page.IsLanding) {
        $logoAsset = "/images/brand/localio-logo-horizontal-transparent.png"
        if ($html -match [regex]::Escape($logoAsset)) {
            Write-OK "Logo horizontal presente"
        }
        else {
            Write-FAIL "Logo horizontal no encontrado: $logoAsset"
            $totalErrors++
        }

        $faviconAsset = "/images/brand/favicon.ico"
        if ($html -match [regex]::Escape($faviconAsset)) {
            Write-OK "Favicon presente"
        }
        else {
            Write-FAIL "Favicon no encontrado: $faviconAsset"
            $totalErrors++
        }
    }

    if ($page.IsDemo) {
        if ($html -match "noindex") {
            Write-OK "noindex presente"
        }
        else {
            Write-FAIL "noindex AUSENTE -- la demo puede ser indexada"
            $totalErrors++
        }

        if ($html -match "nofollow") {
            Write-OK "nofollow presente"
        }
        else {
            Write-FAIL "nofollow AUSENTE"
            $totalErrors++
        }

        $firmaText  = "Demo privada creada por"
        $firmaCount = ([regex]::Matches($html, [regex]::Escape($firmaText))).Count
        if ($firmaCount -eq 0) {
            Write-FAIL "Firma de Localio AUSENTE"
            $totalErrors++
        }
        elseif ($firmaCount -gt 1) {
            Write-FAIL "Firma de Localio DUPLICADA: $firmaCount ocurrencias"
            $totalErrors++
        }
        else {
            Write-OK "Firma de Localio presente (1 ocurrencia)"
        }

        if ($html -match ">Localio<") {
            Write-OK "Texto 'Localio' presente como nodo de texto en el DOM"
        }
        else {
            Write-WARN "Texto 'Localio' no hallado como nodo DOM -- revisar accesibilidad de la firma"
        }

        foreach ($check in $DemoSectionChecks) {
            $escaped = [regex]::Escape($check.Class)
            # Busca la clase exacta: seguida de comilla de cierre o espacio (no sufijo como pd-hero-panel)
            $pattern = 'class="' + $escaped + '([ "])'
            $found   = ([regex]::Matches($html, $pattern)).Count
            $lbl     = $check.Label
            $maxVal  = $check.Max

            if ($found -gt $maxVal) {
                Write-FAIL "Seccion DUPLICADA -- $lbl : $found ocurrencias (max $maxVal)"
                $totalErrors++
            }
            elseif ($found -eq 0) {
                Write-WARN "Clase '$($check.Class)' no encontrada -- seccion ausente o clase renombrada"
            }
            else {
                Write-OK "$lbl : $found ocurrencia(s)"
            }
        }
    }
}

Write-HEAD "=========================================="

if ($totalErrors -eq 0) {
    Write-Host "`n  RESULTADO: OK -- Sin errores detectados`n" -ForegroundColor Green
    exit 0
}
else {
    Write-Host "`n  RESULTADO: $totalErrors ERROR(ES) -- revisar [FAIL] arriba`n" -ForegroundColor Red
    exit 1
}