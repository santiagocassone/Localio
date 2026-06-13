# Cómo crear un nuevo sitio en Localio

## Paso 1 — Crear la carpeta del cliente

Dentro de `/Sites/` (raíz del repositorio), creá una carpeta con el identificador del sitio.  
Usá kebab-case, sin espacios ni caracteres especiales:

```
Sites/
└── mi-nuevo-cliente/     ← nombre que aparecerá en la URL: /demo/mi-nuevo-cliente
	├── site.json
	├── theme.json
	└── assets/           ← imágenes propias del cliente (opcional)
		└── hero.jpg
```

## Paso 2 — Completar site.json

Copiá el siguiente template y editá los valores:

```json
{
  "businessName": "Nombre del Negocio",
  "category": "auto-repair",
  "slogan": "Tu slogan aquí",
  "shortDescription": "Descripción corta (máx 160 caracteres para SEO).",
  "longDescription": "Descripción larga para la sección Sobre Nosotros.",

  "contact": {
	"phone": "011 XXXX-XXXX",
	"whatsApp": "11XXXXXXXX",
	"email": "info@negocio.com.ar",
	"address": "Av. Corrientes 1234",
	"zone": "Barrio, Ciudad",
	"city": "Buenos Aires",
	"googleMapsUrl": "https://maps.google.com/?q=...",
	"embedMapUrl": "https://www.google.com/maps/embed?pb=..."
  },

  "social": {
	"instagram": "https://www.instagram.com/usuario",
	"facebook":  "https://www.facebook.com/pagina"
  },

  "seo": {
	"title":       "Negocio | Barrio, Ciudad",
	"description": "Descripción SEO de hasta 160 caracteres.",
	"keywords":    "keyword1, keyword2, keyword3",
	"schemaType":  "LocalBusiness",
	"priceRange":  "$$"
  },

  "whatsApp": {
	"number":         "11XXXXXXXX",
	"defaultMessage": "Hola, vi su sitio web y quería consultar.",
	"showFloat":      true
  },

  "modules": [
	{ "type": "navbar",          "order": 1,  "enabled": true, ... },
	{ "type": "hero",            "order": 2,  "enabled": true, ... },
	{ "type": "services",        "order": 3,  "enabled": true, ... },
	{ "type": "about",           "order": 4,  "enabled": true, ... },
	{ "type": "testimonials",    "order": 5,  "enabled": true, ... },
	{ "type": "faq",             "order": 6,  "enabled": true, ... },
	{ "type": "hours",           "order": 7,  "enabled": true, ... },
	{ "type": "location",        "order": 8,  "enabled": true, ... },
	{ "type": "cta",             "order": 9,  "enabled": true, ... },
	{ "type": "whatsapp-button", "order": 10, "enabled": true, ... },
	{ "type": "footer",          "order": 11, "enabled": true, ... }
  ]
}
```

### Valores de `category` y `schemaType`

| category | schemaType | Rubro |
|----------|-----------|-------|
| `veterinary` | `VeterinaryCare` | Veterinaria |
| `dental` | `Dentist` | Odontología |
| `auto-repair` | `AutoRepair` | Taller mecánico |
| `beauty` | `BeautySalon` | Estética / peluquería |
| `gym` | `ExerciseGym` | Gimnasio |
| `restaurant` | `Restaurant` | Restaurante |
| `electrician` | `Electrician` | Electricista |
| `plumber` | `Plumber` | Plomero |
| `locksmith` | `Locksmith` | Cerrajero |
| _(otro)_ | `LocalBusiness` | Genérico |

## Paso 3 — Completar theme.json

Copiá uno de los tres presets base y ajustá a los colores del cliente:

### Preset Health (salud, veterinaria, odontología)
```json
{
  "preset": "health",
  "primaryColor": "#1E6B99",
  "secondaryColor": "#F0F7FC",
  "accentColor": "#F26419",
  "textColor": "#1A2332",
  "textMutedColor": "#5A6A7A",
  "surfaceColor": "#FFFFFF",
  "surfaceAltColor": "#F0F7FC",
  "darkColor": "#0D2840",
  "headingFont": "'Nunito', sans-serif",
  "bodyFont": "'Open Sans', sans-serif",
  "fontsUrl": "https://fonts.googleapis.com/css2?family=Nunito:wght@400;600;700;800;900&family=Open+Sans:wght@400;500;600&display=swap",
  "borderRadius": "12px",
  "borderRadiusSm": "8px",
  "borderRadiusLg": "20px",
  "shadowIntensity": "soft",
  "buttonStyle": "rounded",
  "cardStyle": "elevated",
  "heroStyle": "gradient",
  "tone": "warm"
}
```

### Preset Auto (taller, mecánica, detailing)
```json
{
  "preset": "auto",
  "primaryColor": "#C0392B",
  "accentColor": "#E67E22",
  "darkColor": "#1A1A2E",
  "headingFont": "'Oswald', sans-serif",
  "bodyFont": "'Roboto', sans-serif",
  "fontsUrl": "https://fonts.googleapis.com/css2?family=Oswald:wght@400;500;600;700&family=Roboto:wght@400;500;700&display=swap",
  "borderRadius": "6px",
  "borderRadiusSm": "4px",
  "borderRadiusLg": "8px",
  "shadowIntensity": "strong",
  "buttonStyle": "sharp",
  "cardStyle": "elevated",
  "heroStyle": "gradient",
  "tone": "bold"
}
```

### Preset Beauty (estética, peluquería, spa)
```json
{
  "preset": "beauty",
  "primaryColor": "#8B4FA0",
  "accentColor": "#C9956C",
  "darkColor": "#2C1810",
  "headingFont": "'Playfair Display', serif",
  "bodyFont": "'Raleway', sans-serif",
  "fontsUrl": "https://fonts.googleapis.com/css2?family=Playfair+Display:wght@400;600;700;800&family=Raleway:wght@400;500;600&display=swap",
  "borderRadius": "20px",
  "borderRadiusSm": "12px",
  "borderRadiusLg": "28px",
  "shadowIntensity": "soft",
  "buttonStyle": "pill",
  "cardStyle": "elevated",
  "heroStyle": "gradient",
  "tone": "elegant"
}
```

### Tokens de theme.json

| Propiedad | Opciones | Efecto |
|-----------|----------|--------|
| `shadowIntensity` | `soft` / `medium` / `strong` / `none` | Intensidad de sombras en cards |
| `buttonStyle` | `rounded` / `sharp` / `pill` | Forma de todos los botones |
| `cardStyle` | `elevated` / `flat` / `outlined` / `dark` | Estilo de todas las cards |
| `heroStyle` | `gradient` / `split` / `image` / `solid` | Layout del Hero |
| `tone` | `warm` / `cool` / `bold` / `elegant` / `minimal` | Clase en `<body>` (disponible para CSS extra) |

## Paso 4 — Ver la demo

```powershell
dotnet run --project Localio.Web
# Abrí: https://localhost:{puerto}/demo/mi-nuevo-cliente
```

## Paso 5 — Activar / desactivar / reordenar módulos

Solo cambiá `"enabled": false` o el valor de `"order"` en cada módulo del array `modules`.  
No hay que tocar ningún archivo de código.

```json
{ "type": "gallery", "order": 6, "enabled": false }
```

## Paso 6 — Agregar imágenes del cliente

Colocá las imágenes en `Sites/mi-nuevo-cliente/assets/` y referenciá la ruta relativa al sitio web.

> **Importante:** el servidor debe poder acceder a los archivos. En la primera versión, las imágenes del cliente pueden quedar en `wwwroot/sites/{siteId}/` para que sean servidas estáticamente. En producción, considerá un CDN.

## ✅ Checklist para nuevo cliente

- [ ] Carpeta creada en `Sites/{siteId}/`
- [ ] `site.json` completo (businessName, contact, whatsApp, modules)
- [ ] `theme.json` con preset elegido y colores de marca
- [ ] Módulos ordenados y habilitados según la necesidad del cliente
- [ ] SEO config completa (title, description, schemaType)
- [ ] WhatsApp number en formato sin guiones ni espacios: `1145678900`
- [ ] Demo verificada en `/demo/{siteId}`
- [ ] Revisión mobile en 375px y 768px

## 💡 Tips

- El número de WhatsApp debe ser solo dígitos, sin `+` ni espacios: `1145678900`
- El `embedMapUrl` de Google Maps se obtiene en Maps → Compartir → Insertar un mapa → copiar la URL del `src` del iframe
- El módulo `emergency` usa un fondo rojo independientemente del theme — ideal para talleres, cerrajeros, electricistas
- El módulo `catalog` soporta tabs por categoría: si hay una sola categoría, no se muestran los tabs
- Los módulos `before-after` y `gallery` usan `loading="lazy"` automáticamente
