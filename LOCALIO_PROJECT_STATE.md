# Localio Project State

## 1. Fecha de actualización

Junio 2026 (actualizado tras actualizar demo Veterinaria Urquiza: servicios confirmados, horarios, petshop secundario, sin imágenes).

---

## 2. Resumen ejecutivo

Localio es un servicio web B2B para pequeños comercios y profesionales locales de Argentina. La aplicación sirve dos propósitos distintos:

1. **Landing pública** (`/`): página de ventas del propio servicio Localio, orientada a captar comercios interesados mediante WhatsApp como canal de contacto principal.
2. **Demos de sitios**: motor que renderiza sitios de ejemplo (demos genéricas) y demos privadas para prospectos reales.

El negocio funciona enviando demos privadas a comercios para que vean cómo quedaría su sitio, con la intención de convertirlos en clientes.

---

## 3. Stack técnico detectado

| Elemento | Valor detectado |
|---|---|
| Framework | ASP.NET Core Razor Pages |
| Target Framework | `net10.0` |
| Tipo de proyecto | `Microsoft.NET.Sdk.Web` |
| Nullable | `enable` |
| Implicit Usings | `enable` |
| Paquetes externos | Ninguno (sin NuGet packages adicionales detectados en `.csproj`) |
| Bootstrap | v5 incluido vía `wwwroot/lib/` (sin CDN) |
| jQuery | Incluido vía `wwwroot/lib/` |
| Google Fonts | DM Sans (landing) e Inter (demos privadas), vía CDN |
| Fuente de datos | JSON local (sin base de datos) |

---

## 4. Arquitectura real del proyecto

### Estructura de carpetas principales

```
Localio/
├── Localio.Web/               # Proyecto principal ASP.NET Core
│   ├── Helpers/               # SeoHelper, WhatsAppHelper
│   ├── Middleware/            # DemoSubdomainMiddleware
│   ├── Models/                # SiteConfig, ThemeConfig, PrivateDemoConfig, enums, opciones
│   ├── Pages/
│   │   ├── Index.cshtml       # Landing pública /
│   │   ├── Privacy.cshtml     # Página de privacidad
│   │   ├── Error.cshtml       # Página de error
│   │   ├── Demo/              # Demos genéricas (JSON)
│   │   │   ├── Index.cshtml   # /demo/{siteId}
│   │   │   └── List.cshtml    # /demos
│   │   ├── Demos/             # Demos privadas por comercio
│   │   │   └── Citivet.cshtml # /demos/citivet
│   │   └── Shared/
│   │       ├── Landing/       # Partials de la landing pública
│   │       ├── Modules/       # Partials de módulos de demos genéricas
│   │       ├── PrivateDemos/  # Partials de demos privadas
│   │       ├── _Clarity.cshtml
│   │       ├── _DemoUnavailable.cshtml
│   │       ├── _LandingLayout.cshtml
│   │       ├── _Layout.cshtml
│   │       └── _SiteLayout.cshtml
│   ├── Services/              # ISiteConfigService, IPrivateDemoService e implementaciones
│   ├── Properties/
│   │   └── PublishProfiles/   # Perfiles de publicación Azure
│   └── wwwroot/
│       ├── css/               # landing.css, localio.css, private-demos.css, site.css
│       ├── js/                # landing.js, localio.js, localio-analytics.js, site.js
│       └── images/demos/citivet/  # SVGs de la demo Citivet
├── Sites/                     # Demos genéricas (datos JSON)
│   ├── veterinaria-mascotas/
│   ├── centro-estetica/
│   └── taller-ejemplo/
└── PrivateDemos/
	└── demos.json             # Registro de demos privadas
```

### Patrón de arquitectura

- **Razor Pages** como patrón principal. Sin MVC ni Blazor.
- **Servicios singleton** registrados en DI: `SiteConfigService` (demos genéricas), `PrivateDemoService` (demos privadas).
- **Sin base de datos**: todos los datos vienen de archivos JSON locales.
- **Sin panel de administración**.
- **Sin CMS**.
- Los datos de demos genéricas viven en `Sites/{siteId}/site.json` y `theme.json`.
- Los datos de demos privadas viven en `PrivateDemos/demos.json` con cache TTL de 60 segundos.
- Tanto `Sites/` como `PrivateDemos/` están vinculados al proyecto vía `.csproj` como `Content` con `CopyToPublishDirectory`.

### Servicios principales

| Servicio | Interfaz | Descripción |
|---|---|---|
| `SiteConfigService` | `ISiteConfigService` | Carga `site.json` y `theme.json` por `siteId` con cache en memoria |
| `PrivateDemoService` | `IPrivateDemoService` | Carga `demos.json` con cache TTL 60s; maneja visibilidad, indexabilidad y sitemap |

### Modelos principales

- `SiteConfig` — configuración de un sitio genérico (nombre, contacto, SEO, módulos)
- `ThemeConfig` — colores, fuentes, radios del sitio
- `ModuleConfig` (abstracto + 22 tipos derivados) — módulos configurables de un sitio
- `PrivateDemoConfig` — configuración de una demo privada (id, slug, estado, expiración, etc.)
- `PrivateDemoStatus` — enum de estados (ver sección 10)
- `DemoSubdomainOptions` — opciones para routing por subdominio
- `AnalyticsOptions` / `ClarityOptions` — configuración de analytics

---

## 5. Rutas implementadas

| Ruta | Archivo | Descripción |
|---|---|---|
| `/` | `Pages/Index.cshtml` | Landing pública de Localio |
| `/demos` | `Pages/Demo/List.cshtml` | Lista de demos genéricas disponibles |
| `/demo/{siteId}` | `Pages/Demo/Index.cshtml` | Demo genérica por id de sitio |
| `/demos/citivet` | `Pages/Demos/Citivet.cshtml` | Demo privada de Veterinaria Citivet |
| `/demos/urquiza` | `Pages/Demos/Urquiza.cshtml` | Demo privada de Veterinaria Urquiza |
| `/Error` | `Pages/Error.cshtml` | Página de error genérica |
| `/Privacy` | `Pages/Privacy.cshtml` | Página de privacidad (contenido no relevado) |

**Subdominios — código listo, activación pendiente de DNS + Azure:**
El middleware `DemoSubdomainMiddleware` reescribe internamente `{slug}.localio.com.ar` → `/demos/{slug}` sin escribir en la respuesta; Razor Pages renderiza la página normalmente.
`EnableDemoSubdomains = false` en `appsettings.json` (correcto para desarrollo). En producción se activa vía variable de entorno `Localio__EnableDemoSubdomains=true`.
Subdominio listo para activar: **`urquiza.localio.com.ar`** → `/demos/urquiza`. Solo falta configurar DNS (registro CNAME) y hostname en Azure App Service.

**No existen** rutas para: panel admin, API REST, login, registro, ni panel de gestión de demos.

---

## 6. Landing pública

**Estado: Implementado.**

- **Archivo principal:** `Pages/Index.cshtml` con layout `_LandingLayout.cshtml`
- **PageModel:** `IndexModel` (constantes de WhatsApp, SEO, formulario de contacto)
- **Composición por partials** en `Pages/Shared/Landing/`:

| Partial | ID de sección | Descripción |
|---|---|---|
| `_Header.cshtml` | `header` | Header con logo y nav |
| `_Hero.cshtml` | (primer bloque) | Hero con título, subtítulo, CTAs y mockup SVG |
| `_Problem.cshtml` | — | Sección de problema/contexto |
| `_Benefits.cshtml` | — | Beneficios de tener un sitio |
| `_Demos.cshtml` | `#ejemplos` | Rubros de ejemplo (genérico, sin demo real) |
| `_ForWho.cshtml` | — | Para quién es Localio |
| `_Includes.cshtml` | `#que-incluye` | Qué puede incluir un sitio |
| `_Process.cshtml` | `#proceso` | Proceso en 3 pasos |
| `_Trust.cshtml` | — | Por qué Localio (claridad, confianza, mobile) |
| `_CtaFinal.cshtml` | `#contacto` | CTA final + formulario de contacto |
| `_Footer.cshtml` | — | Footer |

**WhatsApp:** número `5491121746236`, mensaje preconfigurado, enlace `wa.me`. CTA principal en hero y sección final.

**Email:** `contacto@localio.com.ar` definido como constante en `IndexModel`.

**Tally:** URL `https://tally.so/r/EkaBbq` centralizada en `IndexModel.TallyFormUrl`. **Implementado**: visible en la landing como CTA secundario en el hero y en la sección final de contacto. Abre en nueva pestaña con `target="_blank" rel="noopener noreferrer"`. Tracking: `data-analytics-event="click_tally"`.

**Formulario de contacto:** markup implementado en `_CtaFinal.cshtml` con campos `Name`, `BusinessName`, `Category`, `Contact`, `Message`. El `POST` valida el modelo y setea `FormSubmitted = true`, pero **el envío real por email/SMTP no está implementado** (hay un TODO explícito en `IndexModel.OnPost()`).

**Sticky CTA mobile:** implementado en `_LandingLayout.cshtml`, botón de WhatsApp visible solo en pantallas pequeñas vía CSS.

**SEO:**
- `title`: "Localio — Sitios web para comercios locales"
- `description`: definida en `IndexModel.SeoDescription`
- `canonical`: `https://localio.com.ar/`
- Open Graph y Twitter Card implementados en el layout
- **No hay `noindex`** en la landing (correcto, es página pública)

**Assets de landing:**
- `wwwroot/css/landing.css` — estilos propios del landing con `asp-append-version="true"`
- `wwwroot/js/landing.js` — comportamiento (header scroll, mobile nav, fade-in)
- `wwwroot/js/localio-analytics.js` — tracking de eventos
- Fuente: DM Sans vía Google Fonts CDN
- **Identidad visual:** ver `wwwroot/images/brand/` (assets de marca aplicados en header, footer y firma de demos)

---

## 7a. Identidad visual y assets de marca

**Estado: Implementado (junio 2025).**

**Carpeta:** `wwwroot/images/brand/`

**Assets disponibles:**
| Archivo | Uso |
|---|---|
| `localio-icon-512.png` | Ícono de alta resolución |
| `localio-icon-256.png` | Ícono mediano |
| `favicon.ico` | Favicon multi-tamaño |
| `favicon-32.png` | Favicon PNG 32px |
| `favicon-64.png` | Favicon PNG 64px |
| `apple-touch-icon.png` | Touch icon iOS |
| `localio-logo-horizontal-transparent.png` | Logo en header (fondo claro) y og:image |
| `localio-logo-horizontal-light.png` | Logo sobre fondos oscuros alternativos |
| `localio-logo-horizontal-dark.png` | Logo en footer landing y firma demos privadas |
| `localio-whatsapp-profile.png` | Perfil WhatsApp (uso manual) |
| `localio-whatsapp-profile-white-bg.png` | Perfil WhatsApp con fondo blanco (uso manual, recomendado) |

**Integrado en:**
- **Header landing** (`_Header.cshtml`): `localio-logo-horizontal-transparent.png`, 34px altura, responsive a 28px en mobile.
- **Footer landing** (`_Footer.cshtml`): `localio-logo-horizontal-dark.png`, 30px, sobre fondo oscuro `--c-bg-dark`.
- **Favicon** (`_LandingLayout.cshtml`, `Citivet.cshtml`, `Urquiza.cshtml`): `favicon.ico` + `favicon-32.png` + `apple-touch-icon.png`.
- **Firma demos privadas** (`_CitivetDemo.cshtml`, `_UrquizaDemo.cshtml`): `localio-logo-horizontal-dark.png`, 14px, opacidad 45%, junto a texto "Demo privada creada por".
- **og:image por defecto** (`_LandingLayout.cshtml`): `localio-logo-horizontal-transparent.png`.

**Paleta de marca:**
- Verde petróleo: `#1F6F68`
- Verde oscuro: `#174F4A`
- Verde salvia suave: `#DDEBE5`
- Fondo claro cálido: `#FAFAF7`
- Beige cálido: `#F2EDE3`
- WhatsApp: `#25D366` (solo CTAs de contacto, no identidad)

**Para perfil de WhatsApp:** usar `localio-whatsapp-profile-white-bg.png` (cuadrado, se recorta mejor en círculo, sin problemas de transparencia).

---

## 7. Demos genéricas

**Estado: Implementado.**

- **Ruta:** `/demo/{siteId}`
- **Lista:** `/demos`
- **Motor:** `SiteConfigService` carga `Sites/{siteId}/site.json` (con cache en memoria sin TTL fijo — singleton con dict)
- **Módulos disponibles:** 22 tipos (`navbar`, `hero`, `services`, `featured-services`, `about`, `benefits`, `gallery`, `testimonials`, `faq`, `hours`, `location`, `whatsapp-button`, `contact-form`, `cta`, `catalog`, `staff`, `promotions`, `emergency`, `before-after`, `brands`, `payment`, `footer`)
- **Renderizado:** `Pages/Demo/Index.cshtml` itera `Site.ActiveModules` y renderiza el partial correspondiente
- **Tema:** `ThemeConfig` genera CSS custom properties vía `SeoHelper.GenerateThemeCss()`
- **JSON-LD / SEO:** `SeoHelper.GenerateJsonLd()` genera Schema.org por categoría de negocio
- **Layout:** `_SiteLayout.cshtml`

**Sitios de ejemplo disponibles en `/Sites`:**

| Carpeta | Negocio (ficticio) | Rubro |
|---|---|---|
| `veterinaria-mascotas` | Veterinaria San Jorge | Veterinary |
| `centro-estetica` | Lumière Beauty Studio | Beauty |
| `taller-ejemplo` | Taller Hernández | Auto-repair |

Estos sitios contienen datos **ficticios** usados como demos internas o de ejemplo.

---

## 8. Demos privadas

**Estado: Implementado. Demos registradas: Citivet (`Paused`) y Urquiza (`ActivePrivate`).**

### Lo que existe

- `PrivateDemos/demos.json` — registro con dos demos: `citivet` (estado: `Paused`) y `urquiza` (estado: `ActivePrivate`)
- `PrivateDemoService` — carga, cache TTL 60s, reglas de visibilidad/indexabilidad
- `IPrivateDemoService` — interfaz con métodos: `GetBySlugAsync`, `IsPubliclyVisible`, `IsIndexable`, `ShouldRenderNoIndex`, `CanBeIncludedInSitemap`
- `Pages/Demos/Citivet.cshtml` — página de la demo privada en `/demos/citivet`
- `Pages/Shared/PrivateDemos/_CitivetDemo.cshtml` — partial HTML de la demo Citivet
- `Pages/Demos/Urquiza.cshtml` — página de la demo privada en `/demos/urquiza`
- `Pages/Demos/Urquiza.cshtml.cs` — PageModel con constantes validadas de Veterinaria Urquiza
- `Pages/Shared/PrivateDemos/_UrquizaDemo.cshtml` — partial HTML de la demo Urquiza
- `Pages/Shared/_DemoUnavailable.cshtml` — pantalla de "demo no disponible" reutilizable
- `Models/PrivateDemos/PrivateDemoConfig.cs` — modelo con campos: id, slug, businessName, businessType, status, publicUrl, contactName, contactPhone, contactEmail, notes, createdAt, updatedAt, expiresAt, isIndexed
- `Models/PrivateDemos/PrivateDemoStatus.cs` — enum de estados (ver sección 10)
- `Models/PrivateDemos/PrivateDemoStatusConverter.cs` — normaliza `"Active"` → `ActivePrivate` para compatibilidad

### Lo que no existe (pendiente)

- No existe una ruta genérica `/demos/{slug}` que sirva cualquier demo privada desde JSON. Cada demo privada requiere su propia página Razor (`Pages/Demos/{NombreComercio}.cshtml`) y su partial.
- No existe panel de gestión de demos.
- No existe sistema automático para crear/activar demos nuevas sin tocar código.

---

## 9. Citivet

**Estado: Implementado como demo privada. Estado actual en JSON: `Paused`.**

- **Ruta:** `/demos/citivet`
- **Acceso por subdominio:** `citivet.localio.com.ar` (cuando `EnableDemoSubdomains = true`, pendiente de activar)
- **Archivo de página:** `Pages/Demos/Citivet.cshtml`
- **PageModel:** `Pages/Demos/Citivet.cshtml.cs` (`CitivetModel`)
- **Partial del contenido:** `Pages/Shared/PrivateDemos/_CitivetDemo.cshtml`
- **Layout:** sin layout compartido (`Layout = null`); HTML self-contained
- **CSS:** `wwwroot/css/private-demos.css` + Inter font vía Google Fonts CDN

**Datos del comercio (hardcodeados como constantes en `CitivetModel`):**

| Campo | Valor |
|---|---|
| BusinessName | Veterinaria Citivet |
| BusinessTagline | Fisioterapia, acupuntura y bienestar veterinario en Villa Crespo |
| Address | Julián Álvarez 211, Villa Crespo, CABA |
| AddressDetail | De 16:30 a 20 hs, con turno previo |
| HomeServiceArea | Recoleta |
| WhatsApp | 5491144390976 |
| Instagram | veterinaria_citivet |
| ContactName | Dra. Judith Groisman |
| MapsUrl | Google Maps URL implementada |
| PageTitle | "Veterinaria Citivet — Fisioterapia, acupuntura y bienestar animal en Villa Crespo, CABA" |

**CTAs detectadas:** WhatsApp + Instagram + Google Maps

**Imágenes/Assets:**
- `wwwroot/images/demos/citivet/clinic-placeholder.svg`
- `wwwroot/images/demos/citivet/hero-veterinary-care.svg`
- `wwwroot/images/demos/citivet/service-acupuncture.svg`
- `wwwroot/images/demos/citivet/service-clinic.svg`
- `wwwroot/images/demos/citivet/service-physiotherapy.svg`

**SEO:**
- `noindex, nofollow` activo cuando el estado es `Draft`, `ActivePrivate`, `Paused`, `Expired` o `Rejected`
- `index, follow` cuando el estado es `ActivePublic` o `Converted`
- Estado actual en `demos.json`: `Paused` → noindex activo

**Visibilidad actual:** `Paused` → `IsPubliclyVisible` = false → se muestra la pantalla `_DemoUnavailable`.

**Pendientes sobre Citivet:**
- No está vinculada desde la landing pública (correcto, es privada)
- El subdominio `citivet.localio.com.ar` requiere configuración de DNS + hostname en Azure (igual que urquiza)
- Los datos del comercio están hardcodeados en el PageModel, no en `demos.json`

---

## 10. Urquiza

**Estado: Implementado como demo privada. Estado actual en JSON: `ActivePrivate`. Demo corregida: versión vieja eliminada, una sola versión sin imágenes ni placeholders. Código listo para subdominio.**

- **Ruta directa:** `/demos/urquiza`
- **URL pública (subdominio):** `https://urquiza.localio.com.ar` (código listo; requiere DNS CNAME + hostname en Azure)
- **publicUrl en demos.json:** `https://urquiza.localio.com.ar`
- **Acceso por subdominio:** reescritura interna `urquiza.localio.com.ar` → `/demos/urquiza` vía `DemoSubdomainMiddleware` (activa cuando `Localio__EnableDemoSubdomains=true`)
- **Archivo de página:** `Pages/Demos/Urquiza.cshtml`
- **PageModel:** `Pages/Demos/Urquiza.cshtml.cs` (`UrquizaModel`)
- **Partial del contenido:** `Pages/Shared/PrivateDemos/_UrquizaDemo.cshtml`
- **Layout:** sin layout compartido (`Layout = null`); HTML self-contained
- **CSS:** `wwwroot/css/private-demos.css` + Inter font vía Google Fonts CDN

**Datos del comercio (validados y hardcodeados como constantes en `UrquizaModel`):**

| Campo | Valor |
|---|---|
| BusinessName | Veterinaria Urquiza |
| BusinessTagline | Atención veterinaria integral en Villa Urquiza |
| Address | Dr. Pedro Ignacio Rivera 5245, Villa Urquiza, CABA |
| WhatsApp | 5491133196217 |
| WhatsAppDisplay | 11 3319-6217 |
| Instagram | @urquizavet |
| InstagramUrl | https://www.instagram.com/urquizavet/?hl=es |
| Rating | 4.8 |
| ReviewsCount | 114 |
| HoursWeekdays | Lunes a viernes: 10:00 a 13:00 · 16:00 a 20:00 |
| HoursSaturday | Sábados: 10:00 a 13:00 |
| MapsUrl | Google Maps URL implementada |
| PageTitle | "Veterinaria Urquiza — Atención veterinaria integral en Villa Urquiza, CABA" |

**Servicios confirmados en la demo (exactamente 9):**
Clínica médica · Cardiología · Cirugía · Dermatología · Endocrinología · Radiología · Ecografía · Fisioterapia · Farmacia

**CTAs implementadas:** WhatsApp + Instagram + Google Maps

**Secciones del partial (`_UrquizaDemo.cshtml`) — versión única y limpia (669 líneas):**
Header nav (Servicios, Confianza, Ubicación, Contacto) · Hero sin imágenes con panel premium (íconos SVG + badges) · Trust 4 cards (rating, reseñas, Instagram, contacto) · Servicios 9 cards en grilla 3×3 con íconos Lucide inline · Petshop secundario 3 cards · About con bullets actualizados · Horarios en pd-hours-card · Ubicación/contacto con links validados · CTA final WhatsApp · Footer con "Demo privada creada por Localio" · CTA sticky mobile

**Assets:** Sin imágenes fotográficas. Sin placeholders. La demo funciona íntegramente con íconos SVG inline.

**SEO:**
- `noindex, nofollow` activo (estado `ActivePrivate`)
- No incluida en sitemap
- No enlazada desde la landing pública

**Visibilidad actual:** `ActivePrivate` → `IsPubliclyVisible` = true → se muestra la demo.

**Pendientes sobre Urquiza (fuera del código):**
- No vinculada desde la landing pública (correcto, es privada)
- Activar subdominio en producción: configurar CNAME `urquiza` → Azure App Service hostname + agregar custom domain en Azure Portal + setear `Localio__EnableDemoSubdomains=true` en App Settings de Azure

---

## 11. Estados de demos

**Estado: Implementado completamente en código. Citivet en estado `Paused`.**

### Enum `PrivateDemoStatus`

| Estado | Visible | Indexable | En Sitemap | Descripción |
|---|---|---|---|---|
| `Draft` | No | No | No | Interna, no enviada al prospecto |
| `ActivePrivate` | Sí | No | No | Enviada al prospecto, visible por link, noindex |
| `ActivePublic` | Sí | Sí | Sí | Autorizada por el comercio para buscadores |
| `Paused` | No | No | No | Temporalmente desactivada |
| `Expired` | No | No | No | Venció `ExpiresAt` |
| `Converted` | Sí | Sí | Sí | Cliente activo |
| `Rejected` | No | No | No | Descartada |
| `Active` (obsoleto) | — | — | — | Solo para compatibilidad; se normaliza a `ActivePrivate` |

### Regla extra

Si `ExpiresAt` está vencido, la demo no es visible ni indexable, independientemente del estado.

### Implementación

Las reglas están implementadas en `PrivateDemoService`: `IsPubliclyVisible()`, `IsIndexable()`, `CanBeIncludedInSitemap()`, `ShouldRenderNoIndex()`.

---

## 12. SEO / indexación

### Landing pública (`/`)

- **Implementado:** `<title>`, `<meta description>`, `<meta keywords>`, Open Graph, Twitter Card, `<link rel="canonical">` a `https://localio.com.ar/`
- **No tiene** `noindex` (correcto)
- `_LandingLayout.cshtml` gestiona todos los metatags desde `ViewData`

### Demos genéricas (`/demo/{siteId}`)

- SEO gestionado por `_SiteLayout.cshtml` usando `SeoHelper.BuildPageTitle()`, `SeoHelper.BuildDescription()`, `SeoHelper.GenerateJsonLd()`
- JSON-LD Schema.org por categoría (VeterinaryCare, BeautySalon, AutoRepair, etc.)
- No se detectó control explícito de `noindex` en demos genéricas desde el código inspeccionado

### Demos privadas (`/demos/citivet`)

- `noindex, nofollow` emitido condicionalmente según `CitivetModel.ShouldRenderNoIndex` (delegado a `PrivateDemoService`)

### `robots.txt`

- **No verificable / No detectado**: no existe archivo `robots.txt` en `wwwroot/` ni ruta que lo sirva en el código inspeccionado.

### `sitemap.xml`

- **No verificable / No detectado**: la interfaz `IPrivateDemoService.CanBeIncludedInSitemap()` existe en el código pero no hay ninguna ruta ni controlador que genere y sirva un `sitemap.xml`. La lógica existe, el endpoint no.

---

## 13. Subdominios

### Código implementado

- `DemoSubdomainMiddleware` detecta el host `{slug}.localio.com.ar` y reescribe el path a `/demos/{slug}`
- Solo actúa en el path raíz `/`; assets y subrutas pasan directamente al pipeline
- Hosts reservados (ignorados): `www`, `demo`, `demos`, `admin`, `api`, `app`, `mail`, `ftp`, `localhost`
- Slugs válidos: regex `^[a-z0-9][a-z0-9\-]*[a-z0-9]$|^[a-z0-9]$`

### Configuración detectada en repo

- `Localio:RootDomain = "localio.com.ar"` (en `appsettings.json`)
- `Localio:EnableDemoSubdomains = false` (deshabilitado por defecto)
- Variables de entorno para producción: `Localio__RootDomain`, `Localio__EnableDemoSubdomains`
- Comentario en código: "Activar EnableDemoSubdomains en Azure una vez configurados DNS y certs"

### Estado actual

- El middleware está registrado en el pipeline (`Program.cs`) pero inactivo (`EnableDemoSubdomains = false`)
- `citivet.localio.com.ar` sería soportado por código una vez habilitado, porque reescribiría a `/demos/citivet`

### Pendiente

- Configuración DNS en Cloudflare/Azure para wildcard `*.localio.com.ar`: **no verificable desde el repo**
- Certificados SSL wildcard: **no verificable desde el repo**
- Activación de `EnableDemoSubdomains = true` en producción: pendiente

---

## 14. Analytics / métricas

### Microsoft Clarity

**Estado: Implementado, deshabilitado por defecto.**

- Partial `_Clarity.cshtml`: renderiza el script de Clarity solo si `Analytics:Clarity:Enabled = true` y `ProjectId` no está vacío
- En `appsettings.json`: `"Enabled": false`, `"ProjectId": ""` (sin ProjectId real)
- En producción: activar con variables de entorno `Analytics__Clarity__Enabled=true` y `Analytics__Clarity__ProjectId=<id>`
- El ProjectId real **no está en el repo** (correcto, es sensible)
- `_Clarity.cshtml` está incluido en `_LandingLayout.cshtml` y en `Pages/Demos/Citivet.cshtml`

### `localio-analytics.js`

**Estado: Implementado.**

- API pública: `window.localioAnalytics.track(eventName, extra)`
- Lee metadata desde `data-page-type`, `data-demo-id`, `data-business-name` del elemento con `[data-page-type]`
- Envía eventos a Clarity vía `window.clarity('event', ...)` y `window.clarity('set', ...)`
- **Click tracking:** delegado en `document`, captura clics en `[data-analytics-event]`
- **Section visibility tracking:** `IntersectionObserver` sobre `[data-analytics-section="contact"]`, dispara `section_contact_view` una vez por carga
- Eventos usados en landing: `click_whatsapp`, `click_como_funciona`, `section_contact_view`

---

## 15. Cache / assets

### CSS propios

| Archivo | Uso | `asp-append-version` |
|---|---|---|
| `wwwroot/css/landing.css` | Landing pública | Sí |
| `wwwroot/css/private-demos.css` | Demos privadas (Citivet) | Sí |
| `wwwroot/css/localio.css` | Uso general (verificar) | No verificado |
| `wwwroot/css/site.css` | Default ASP.NET Core | No verificado |

### JS propios

| Archivo | Uso | `asp-append-version` |
|---|---|---|
| `wwwroot/js/landing.js` | Landing pública | Sí |
| `wwwroot/js/localio-analytics.js` | Landing pública + demos | Sí |
| `wwwroot/js/localio.js` | Uso general (verificar) | No verificado |
| `wwwroot/js/site.js` | Default ASP.NET Core | No verificado |

### Imágenes

- `wwwroot/images/demos/citivet/` — 5 archivos SVG para la demo Citivet

### Bootstrap y jQuery

- Incluidos como archivos estáticos en `wwwroot/lib/` (sin CDN externo)
- Bootstrap v5 (grid, reboot, utilities, full bundle)
- jQuery + jQuery Validation + Unobtrusive Validation

### `MapStaticAssets` / `WithStaticAssets`

- El pipeline usa `app.MapStaticAssets()` y `app.MapRazorPages().WithStaticAssets()` (fingerprinting y compresión de assets de .NET 10)
- `app.UseStaticFiles()` se registra **antes** del middleware de subdominios para garantizar que los assets se sirvan correctamente incluso con rewrite de host

---

## 16. Deploy / infraestructura documentada

### Azure App Service

**Detectado en PublishProfiles:**

| Campo | Valor |
|---|---|
| Nombre de la app | `localio-web-prod` |
| Resource Group | `rg-localio-prod` |
| Subscription ID | `5bbea6d4-75e5-4690-bd7a-3504a3e51f70` |
| Región | `centralus-01` |
| URL de Azure | `localio-web-prod-axdce7fghqe9gshw.centralus-01.azurewebsites.net` |
| OS | Linux (`IsLinux=true`) |
| Métodos de publish disponibles | Web Deploy (MSDeploy), Zip Deploy, FTP |

**Nota:** La URL de Azure está expuesta en los `.pubxml` pero no hay passwords ni tokens en el repo. Los perfiles tienen `_SavePWD = true` lo que implica que la contraseña se guarda localmente, **no en el repo**.

### Puerto en Azure App Service

- El código en `Program.cs` detecta la variable de entorno `PORT` o `WEBSITES_PORT` y configura Kestrel en `0.0.0.0:{port}` automáticamente para Azure Linux.

### Variables de entorno esperadas en producción

| Variable | Propósito |
|---|---|
| `PORT` / `WEBSITES_PORT` | Puerto de Kestrel en Azure |
| `Localio__RootDomain` | Dominio raíz para subdominios |
| `Localio__EnableDemoSubdomains` | Activar routing por subdominio |
| `Analytics__Clarity__Enabled` | Habilitar Clarity |
| `Analytics__Clarity__ProjectId` | ID del proyecto de Clarity |

### Dominio `localio.com.ar`

- Referenciado como canónico en `IndexModel` (`PageCanonical = "https://localio.com.ar/"`)
- Configurado como `RootDomain` en `appsettings.json`
- La configuración DNS de Cloudflare o Azure para este dominio **no es verificable desde el repo**

### Cloudflare

- **No verificable desde el repo**: no hay archivos de configuración de Cloudflare en el repositorio.

---

## 17. Decisiones técnicas detectadas

Las siguientes decisiones están reflejadas directamente en código, comentarios o estructura del repo:

1. **JSON local en lugar de base de datos**: Los datos de sitios (`Sites/`) y demos privadas (`PrivateDemos/`) son archivos JSON. Los comentarios `// TODO: Demo Manager v0 — migrar a tabla Demo en base de datos cuando corresponda` confirman que es una decisión deliberada y transitoria.

2. **Sin panel de administración**: No existe ninguna ruta ni controlador de admin. La gestión se hace editando JSON directamente.

3. **Sin CMS**: No hay integración con ningún sistema de gestión de contenidos.

4. **Microsoft Clarity como herramienta de analytics**: Elegida y documentada en el código. Activación por variable de entorno.

5. **Subdominios manuales**: Cada subdominio de demo se activa configurando DNS externamente; el middleware hace el rewrite internamente.

6. **`UseStaticFiles` antes del middleware de rewrite**: Decisión explícita documentada en `Program.cs` para que los assets lleguen directamente sin ser interceptados por el rewrite de subdominio.

7. **Demos privadas con noindex por defecto**: Solo `ActivePublic` y `Converted` son indexables. Cualquier otro estado produce `noindex, nofollow`.

8. **Demos privadas NO vinculadas desde la landing pública**: Comentarios explícitos en `Citivet.cshtml` ("NO enlazar desde la landing pública").

9. **WhatsApp como canal de contacto principal**: Toda la landing está orientada a derivar al WhatsApp, no a un formulario. El formulario existe como alternativa pero el envío real no está implementado.

10. **Kestrel configurado dinámicamente para Azure**: Detección de `PORT`/`WEBSITES_PORT` en `Program.cs`.

---

## 18. Pendientes detectados

### Urgente

- **Formulario de contacto sin envío real**: `IndexModel.OnPost()` tiene un TODO explícito. El form valida y setea `FormSubmitted = true` pero no envía nada. Opciones documentadas en el TODO: MailKit, SendGrid, Resend, Azure Communication Services, FormSpree.
- **`robots.txt` no existe**: No hay control de indexación a nivel de raíz del sitio. Cualquier crawler puede acceder a `/demos` y `/demo/{siteId}`.
- **`sitemap.xml` no existe**: La lógica `CanBeIncludedInSitemap()` está implementada pero no hay endpoint que sirva el sitemap.

### Recomendado

- **Activar `EnableDemoSubdomains` en producción**: Requiere configurar DNS wildcard y certificado SSL en Azure/Cloudflare. El código está listo.
- **Activar Microsoft Clarity en producción**: Requiere setear las variables de entorno con el ProjectId real.
- **Migrar datos de negocio de Citivet de constantes hardcodeadas a `demos.json`**: Actualmente el `CitivetModel` tiene los datos del comercio como constantes en C#, desvinculados del JSON.
- **Agregar `robots.txt` que bloquee `/demos/` y `/demo/`** hasta que se defina la política SEO para las demos genéricas.
- **Definir política SEO para `/demos` y `/demo/{siteId}`**: Actualmente no tiene `noindex` ni `robots.txt` que las controle.

### Futuro

- Migrar `PrivateDemos/demos.json` a tabla en base de datos (TODOs en código)
- Sistema genérico de demos privadas por slug (sin necesidad de crear una Razor Page por cada comercio)
- Panel básico de gestión de demos (actualmente todo es edición manual de JSON)
- Implementar og:image real para la landing

---

## 19. Riesgos / puntos de cuidado

1. **Indexar demos privadas por error**: Si se activa `EnableDemoSubdomains` o se comparte la URL de `/demos/citivet` sin que el estado sea `ActivePrivate`, y sin `robots.txt`, Google podría rastrearla. Mitigado parcialmente por el `noindex` meta tag, pero `robots.txt` agregaría una capa extra.

2. **`/demos` y `/demo/{siteId}` no tienen `noindex`**: Si los datos de ejemplo contienen nombres de negocios ficticios reales o datos que no se quiere indexar, podrían aparecer en buscadores.

3. **Formulario de contacto silencioso**: El form de la landing acepta datos del usuario y los descarta. Si un prospecto lo completa creyendo que fue enviado, se pierde el lead. Riesgo de negocio activo.

4. **Datos de Citivet hardcodeados en `CitivetModel`**: Si cambia un dato del comercio (teléfono, dirección), requiere modificar el código C# y redesplegar, no solo editar `demos.json`.

5. **Cache de `SiteConfigService` sin TTL**: El singleton de `SiteConfigService` cachea en un `Dictionary` sin expiración. Un cambio en `site.json` en producción requiere reiniciar la app para que se refleje.

6. **Rutas de subdominios reservados**: La lista está hardcodeada en el middleware. Si se necesita agregar un subdominio reservado nuevo (p.ej. `status`, `cdn`), requiere cambio de código.

7. **Sin protección de `/demos` en desarrollo**: La ruta `/demos` es pública y lista los sitios disponibles. En producción podría revelar demos internas si no se controla el acceso.

8. **PublishProfiles en el repo**: Los archivos `.pubxml` están en el repo con la URL del SCM endpoint de Azure y el nombre de usuario. No hay contraseñas, pero la URL del Kudu expone información de la cuenta de Azure.

9. **Duplicación silenciosa de contenido en demos privadas**: Al modificar una demo, es posible agregar nueva versión sin eliminar la anterior, resultando en dos headers, dos heroes, dos footers o secciones de servicios duplicadas renderizadas en el HTML. Ya ocurrió en Citivet y Urquiza. Mitigado con la regla anti-duplicados de la sección 20 (regla 16).

10. **Encoding UTF-8 en demos privadas**: Los archivos `.cshtml` de demos privadas pueden quedar con doble encoding (Latin-1→UTF-8) si son editados con herramientas que no respetan el charset. El resultado visible es mojibake como `Ã­` en lugar de `í`, `Ã±` en lugar de `ñ`, etc. Ya ocurrió en Citivet y en `_UrquizaDemo.cshtml` (junio 2025). Mitigado con la regla de validación de encoding de la sección 20 (regla 17).

---

## 20. Instrucciones para futuros asistentes

Al trabajar en este proyecto, tener en cuenta las siguientes reglas operativas:

1. **Responder en español** siempre, a menos que el usuario indique lo contrario.

2. **Actuar como arquitecto .NET 10 / C# / Razor Pages para Localio**. No asumir Blazor, MVC, ni patrones de otros frameworks.

3. **Priorizar bajo costo y baja complejidad operativa**. El proyecto corre en Azure App Service con datos en JSON. No agregar base de datos, CMS, panel admin ni dependencias externas salvo pedido explícito.

4. **No sobreingeniería**. Preferir cambios mínimos y mantenibles.

5. **Cuidar Mobile Safari**. No generar scroll horizontal. Usar `min-height: 100dvh` donde corresponda. Testar con viewport reducido.

6. **`noindex, nofollow` en demos privadas** por defecto, salvo que el estado sea `ActivePublic` o `Converted`, o que el usuario autorice explícitamente.

7. **No duplicar lógica de visibilidad/indexabilidad**: está centralizada en `PrivateDemoService`. No replicarla en las páginas.

8. **No romper la landing pública** (`/`). Es la única página pública de venta del servicio.

9. **No romper `/demos` ni `/demo/{siteId}`**. Son las demos de ejemplo usadas internamente.

10. **No enlazar demos privadas desde la landing pública**. Están separadas intencionalmente.

11. **No inventar datos de negocios**: no generar reseñas, matrículas, trayectoria, horarios, servicios ni precios que no estén confirmados por el usuario.

12. **Usar `asp-append-version="true"`** en todo CSS y JS propio para cache busting.

13. **Mantener SVGs inline** en la landing (no usar Font Awesome ni librerías de iconos externas).

14. **Si se agregan demos privadas nuevas**, seguir el patrón de `Pages/Demos/Citivet.cshtml` + partial en `Pages/Shared/PrivateDemos/` + entrada en `PrivateDemos/demos.json`.

15. **Cuando se realicen cambios relevantes**, actualizar `LOCALIO_PROJECT_STATE.md` para reflejar el estado real actualizado del proyecto.

16. **Regla anti-duplicados para demos privadas — obligatoria dentro del alcance del cambio:** Cuando se modifique una demo privada existente, tratar la tarea como reemplazo/refactor, no como agregado incremental. No agregar una segunda versión de una sección sin eliminar la anterior. No ocultar contenido viejo con CSS. No dejar contenido viejo renderizado en la ruta afectada.

    **Alcance normal de la validación:**
    - La demo modificada y las secciones tocadas.
    - Partials/layouts compartidos, solo si fueron modificados en esta tarea.
    - HTML renderizado de la ruta afectada (p. ej. `/demos/{slug}`).
    - Subdominio correspondiente, si existe.

    No revisar otras demos ni todo el proyecto salvo que: se haya modificado un layout o partial compartido usado por varias demos; se haya modificado CSS/JS global; o el cambio sea explícitamente global.

    **Checklist acotado — duplicaciones (solo para la demo afectada):**
    - La demo afectada tiene un solo hero.
    - La demo afectada tiene una sola sección principal de servicios, salvo decisión explícita.
    - La demo afectada no tiene dos bloques de contacto equivalentes.
    - La demo afectada no tiene dos footers.
    - No queda contenido viejo renderizado en la ruta afectada.
    - No quedan placeholders o textos obsoletos en la ruta afectada.
    - No se revisan demos no afectadas salvo que se haya tocado un componente compartido.

17. **Validación de encoding UTF-8 en demos privadas — obligatoria dentro del alcance del cambio:** Antes de dar por terminado un cambio en una demo privada, validar que los caracteres en español se rendericen correctamente en los archivos y rutas afectadas. Este problema ya ocurrió en **Citivet** y **Veterinaria Urquiza** (junio 2025).

    **Alcance normal de la validación:**
    - Archivos modificados de esa demo (`.cshtml`, `.cs`, `.json`, `.css` según corresponda).
    - Partials/layouts compartidos, solo si fueron modificados en esta tarea.
    - HTML renderizado de la ruta afectada (p. ej. `/demos/{slug}`).
    - Subdominio correspondiente, si existe.

    No revisar todo el proyecto salvo que: se haya modificado un layout o partial compartido; se haya modificado CSS/JS global; se detecten caracteres rotos en un componente compartido; o el cambio sea global.

    **Patrones de mojibake a buscar en los archivos y HTML afectados:**
    `Ã`, `Â`, `�`, `ClÃ`, `SÃ`, `atenciÃ`, `ubicaciÃ`, `calificaciÃ`, `reseÃ`, `artÃ`, `mÃ`, `CardiologÃ`, `CirugÃ`, `DermatologÃ`, `EndocrinologÃ`, `RadiologÃ`, `EcografÃ`

    **Confirmar que se rendericen correctamente (en la ruta afectada):**
    Clínica, Cardiología, Cirugía, Dermatología, Endocrinología, Radiología, Ecografía, atención, ubicación, calificación, reseñas, artículos, Sábados, Cómo, veterinaria

    **Reglas de corrección:**
    - No resolver quitando acentos. No reemplazar `Clínica` por `Clinica`.
    - No eliminar la `ñ`. El objetivo es español correcto en UTF-8.
    - Confirmar que los archivos `.cshtml`, `.cs`, `.json` y `.css` **modificados** estén guardados como UTF-8 sin BOM.
    - Si la demo usa `Layout = null` (HTML self-contained), confirmar que tenga `<meta charset="utf-8">` dentro del `<head>`, antes de cualquier texto visible con acentos.
    - Confirmar que la respuesta HTTP tenga `Content-Type: text/html; charset=utf-8`.
    - Validar tanto `/demos/{slug}` como el subdominio `{slug}.localio.com.ar` si existe.

    **Checklist de cierre — encoding (solo para la ruta afectada):**
    - [ ] No aparecen `Ã`, `Â` ni `�` en el HTML renderizado de la ruta afectada.
    - [ ] Los acentos y ñ se ven correctamente en el browser.
    - [ ] No se eliminaron acentos como falsa solución.
    - [ ] Los archivos **modificados** están guardados como UTF-8 sin BOM.
    - [ ] La página self-contained incluye `<meta charset="utf-8">` en el `<head>`.

    **Diagnóstico rápido de mojibake en PowerShell:**
    ```powershell
    $utf8 = [System.Text.Encoding]::UTF8
    $text = [System.IO.File]::ReadAllText("ruta\al\archivo.cshtml", $utf8)
    [regex]::Matches($text, 'Ã[^\s]').Count   # debe ser 0
    [regex]::Matches($text, 'Â[^\s]').Count   # debe ser 0
    [regex]::Matches($text, '\uFFFD').Count    # debe ser 0
    ```

    **Corrección de doble-encoding (UTF-8 guardado como Latin-1 y releído como UTF-8):**
    ```powershell
    $utf8 = [System.Text.Encoding]::UTF8
    $latin1 = [System.Text.Encoding]::GetEncoding(28591)
    $raw = [System.IO.File]::ReadAllBytes("archivo_dañado.cshtml")
    if ($raw[0] -eq 0xEF -and $raw[1] -eq 0xBB -and $raw[2] -eq 0xBF) { $raw = $raw[3..($raw.Length-1)] }
    $wrongText = $utf8.GetString($raw)          # texto mojibake
    $fixedBytes = $latin1.GetBytes($wrongText)   # convierte de vuelta a bytes UTF-8 originales
    $fixedText  = $utf8.GetString($fixedBytes)   # texto correcto en español
    $utf8NoBom  = New-Object System.Text.UTF8Encoding $false
    [System.IO.File]::WriteAllText("archivo_dañado.cshtml", $fixedText, $utf8NoBom)
    ```

    Este problema ocurrió en `_UrquizaDemo.cshtml` en junio 2025 y fue corregido con la técnica anterior.
