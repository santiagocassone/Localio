# Localio — Generador de Sitios para Comercios Locales

Sistema modular en **ASP.NET Core / Razor Pages (.NET 10)** para crear sitios web profesionales para comercios locales sin escribir código nuevo por cliente. Cada sitio se configura con archivos JSON.

## 🚀 Demos disponibles

| Demo | URL | Preset |
|------|-----|--------|
| Veterinaria San Jorge | `/demo/veterinaria-mascotas` | Health — azul, Nunito |
| Taller Hernández | `/demo/taller-ejemplo` | Auto — rojo, Oswald |
| Lumière Beauty Studio | `/demo/centro-estetica` | Beauty — púrpura, Playfair |
| Listado de todos | `/demos` | — |

## ⚡ Inicio rápido

```powershell
cd Localio.Web
dotnet run
# Abrí https://localhost:{puerto}/demos
```

## 🗂️ Estructura del proyecto

```
Localio/
├── Localio.Web/
│   ├── Helpers/
│   │   ├── SeoHelper.cs          # JSON-LD, meta tags, CSS variables
│   │   └── WhatsAppHelper.cs     # Generador de URLs wa.me
│   ├── Models/
│   │   ├── SiteConfig.cs         # Modelo principal + ContactInfo, SeoConfig, etc.
│   │   ├── ThemeConfig.cs        # Paleta, tipografías, radio, sombras
│   │   └── Modules/
│   │       └── ModuleConfigs.cs  # 22 módulos con [JsonPolymorphic]
│   ├── Pages/
│   │   ├── Demo/
│   │   │   ├── Index.cshtml      # Ruta: /demo/{SiteId}
│   │   │   └── List.cshtml       # Ruta: /demos (listado)
│   │   └── Shared/
│   │       ├── _SiteLayout.cshtml
│   │       └── Modules/          # 22 partial views
│   ├── Services/
│   │   ├── ISiteConfigService.cs
│   │   └── SiteConfigService.cs  # Carga + cache en memoria
│   └── wwwroot/
│       ├── css/localio.css       # Sistema CSS premium (~700 líneas)
│       └── js/localio.js         # JS mínimo (~130 líneas)
└── Sites/                        # ← Configuración de clientes
	├── veterinaria-mascotas/
	│   ├── site.json
	│   └── theme.json
	├── taller-ejemplo/
	│   ├── site.json
	│   └── theme.json
	└── centro-estetica/
		├── site.json
		└── theme.json
```

## 🧩 Módulos disponibles

| Tipo JSON | Descripción |
|-----------|-------------|
| `navbar` | Navegación sticky con menú móvil |
| `hero` | Sección principal (gradient / split / image) |
| `services` | Grid de servicios con cards |
| `featured-services` | Servicios destacados en cards horizontales |
| `about` | Sobre el negocio con imagen y puntos clave |
| `benefits` | Diferenciadores con íconos |
| `gallery` | Galería de imágenes responsive |
| `testimonials` | Reseñas con estrellas y avatar |
| `faq` | Acordeón con `<details>` (sin JS) |
| `hours` | Horarios de atención con card |
| `location` | Mapa embebido + panel de datos |
| `whatsapp-button` | Botón flotante con pulso animado |
| `contact-form` | Formulario configurable |
| `cta` | Call to action full-width |
| `catalog` | Catálogo con tabs por categoría |
| `staff` | Equipo de profesionales |
| `promotions` | Tarjetas de promociones |
| `emergency` | Banner de urgencias (rojo) |
| `before-after` | Comparador antes/después |
| `brands` | Marcas/logos en fila |
| `payment` | Métodos de pago |
| `footer` | Footer con columnas, redes y contacto |

## 🎨 Presets de theme.json

| Preset | Rubro | Fuentes | Estilo |
|--------|-------|---------|--------|
| `health` | Veterinaria, odontología, salud | Nunito + Open Sans | Cálido, confiable |
| `auto` | Talleres, mecánica | Oswald + Roboto | Fuerte, técnico |
| `beauty` | Estética, peluquería | Playfair Display + Raleway | Elegante, premium |

## 📖 Crear un nuevo sitio

Ver [docs/crear-nuevo-sitio.md](docs/crear-nuevo-sitio.md)

## 🚢 Deploy

### Opción A — Azure App Service
```powershell
dotnet publish -c Release -o ./publish
# Subir ./publish + carpeta Sites/ al servidor
```

### Opción B — VPS / shared hosting (Linux)
```bash
dotnet publish -c Release -o ./publish
# Copiar Sites/ junto al publish
# Configurar ASPNETCORE_ENVIRONMENT=Production
# Configurar Localio__SitesPath=/ruta/absoluta/Sites
```

### Variable de entorno para Sites en producción
```
Localio__SitesPath=/var/www/localio/Sites
```

## 📋 Requisitos
- .NET 10 SDK
- Acceso a internet para Google Fonts (o Self-host las fuentes)
