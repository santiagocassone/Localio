# Demo Subdomains — Guía operativa

Documento de referencia para el sistema de subdominios por demo privada de Localio.

---

## ¿Cómo funciona?

Cuando `EnableDemoSubdomains = true`, el middleware `DemoSubdomainMiddleware` intercepta
requests a `{slug}.localio.com.ar` y los reescribe internamente a `/demos/{slug}`.
La misma Razor Page renderiza la demo usando el estado configurado en `PrivateDemos/demos.json`.

No hay duplicación de código: `/demos/citivet` y `citivet.localio.com.ar` ejecutan
exactamente la misma lógica de visibilidad, robots e indexabilidad.

---

## Fase 1 — Alta manual de subdominio (recomendada)

Proceso para activar `citivet.localio.com.ar` como ejemplo.

### 1. Cloudflare — CNAME

Agregar en la zona `localio.com.ar`:

| Campo         | Valor                                    |
|---------------|------------------------------------------|
| Type          | CNAME                                    |
| Name          | `citivet`                                |
| Target        | `localio-web-prod.azurewebsites.net`     |
| Proxy status  | Proxied (nube naranja)                   |
| TTL           | Auto                                     |

### 2. Azure App Service — Custom domain

En el App Service `localio-web-prod`:

1. **Custom domains** → Add custom domain
2. Ingresar: `citivet.localio.com.ar`
3. Verificar ownership (TXT o CNAME ya debe estar en Cloudflare)
4. Confirmar

### 3. Azure App Service — Certificado

1. **Certificates** → Managed certificates → Add certificate
2. Seleccionar: `citivet.localio.com.ar`
3. Agregar **SNI SSL binding** para ese dominio

### 4. App Service Configuration — Variables de entorno

En **Configuration → Application settings** agregar:

| Name                          | Value              |
|-------------------------------|--------------------|
| `Localio__RootDomain`         | `localio.com.ar`   |
| `Localio__EnableDemoSubdomains` | `true`           |

> Las variables de entorno con `__` corresponden a secciones anidadas de configuración.
> Equivalen a `appsettings.json` → `Localio.RootDomain` y `Localio.EnableDemoSubdomains`.

### 5. PrivateDemos/demos.json

Verificar que Citivet tenga:

```json
{
  "slug": "citivet",
  "status": "ActivePrivate",
  "publicUrl": "https://citivet.localio.com.ar"
}
```

### 6. Verificación

- `https://citivet.localio.com.ar/` → muestra la demo de Citivet
- `https://localio.com.ar/demos/citivet` → sigue funcionando igual
- Ambas URLs respetan el estado de `PrivateDemos/demos.json`
- Con `status: ActivePrivate` → ambas renderizan `noindex, nofollow`
- Con `status: ActivePublic` → ambas quedan indexables

---

## Comportamiento por estado

| Estado         | Visible | Meta robots         | En sitemap |
|----------------|---------|---------------------|------------|
| `Draft`        | No      | noindex, nofollow   | No         |
| `ActivePrivate`| Sí      | noindex, nofollow   | No         |
| `ActivePublic` | Sí      | (sin meta robots)   | Sí         |
| `Paused`       | No      | noindex, nofollow   | No         |
| `Expired`      | No      | noindex, nofollow   | No         |
| `Converted`    | Sí      | (sin meta robots)   | Sí         |
| `Rejected`     | No      | noindex, nofollow   | No         |

Si `expiresAt` está vencido, la demo se trata como no visible aunque el estado sea `ActivePrivate` o `ActivePublic`.

---

## Paths en subdominio

Solo se acepta el path raíz `/` en subdominios de demo:

- `citivet.localio.com.ar/` → ✅ renderiza la demo
- `citivet.localio.com.ar/algo` → 404 (sin redirect para evitar loops)

---

## Hosts reservados

Los siguientes subdominios están excluidos del routing de demos y se pasan al pipeline normal:

```
www, demo, demos, admin, api, app, mail, ftp, localhost
```

---

## Desarrollo local

`EnableDemoSubdomains` está `false` por defecto en `appsettings.json`.

Para probar en local:

1. Agregar en el archivo `hosts` del sistema:
   ```
   127.0.0.1  citivet.localio.local
   ```
2. Configurar `appsettings.Development.json`:
   ```json
   {
	 "Localio": {
	   "RootDomain": "localio.local",
	   "EnableDemoSubdomains": true
	 }
   }
   ```
3. El perfil de Kestrel debe incluir el binding al puerto HTTP (ver `launchSettings.json`).

---

## Fase 2 — Wildcard DNS (futura, no implementada)

Para escalar a múltiples subdominios sin configuración manual por demo:

### DNS Cloudflare

Agregar una entrada wildcard:

| Campo   | Valor                                |
|---------|--------------------------------------|
| Type    | CNAME                                |
| Name    | `*`                                  |
| Target  | `localio-web-prod.azurewebsites.net` |

### Azure App Service

- Agregar el custom domain `*.localio.com.ar`
- Requiere un **certificado wildcard** (no disponible con Managed Certificates de Azure)
- Opciones: Let's Encrypt wildcard via ACME, o certificado comercial subido manualmente

### Consideraciones

- Un certificado wildcard cubre un solo nivel: `*.localio.com.ar` cubre `citivet.localio.com.ar`
  pero **no** `sub.citivet.localio.com.ar`
- En Azure App Service los certificados wildcard no pueden ser gestionados automáticamente;
  requieren renovación manual o script de automatización
- Evaluar volumen: si hay más de ~10 demos activas simultáneamente, el wildcard se amortiza

### Recomendación actual

Mantener Fase 1 (manual) hasta tener al menos 5-10 demos activas.
La automatización de alta de subdominios puede implementarse luego con Azure CLI o Bicep.

---

## Notas de seguridad

- Los slugs de subdominio están validados por regex: solo letras, dígitos y guiones
- Un slug no encontrado en `demos.json` renderiza la pantalla "Esta demo ya no se encuentra disponible"
  con `noindex, nofollow` — nunca expone errores técnicos
- El middleware está inactivo si `EnableDemoSubdomains = false`, sin overhead en requests normales
