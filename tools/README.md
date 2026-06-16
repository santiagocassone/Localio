# tools/

Herramientas de soporte para el proyecto Localio.Web.

---

## validate-localio.ps1

Script liviano de smoke test / regression check para detectar errores recurrentes
en la landing principal y en las demos privadas de Localio.

No reemplaza la revision visual ni tests de integracion.
Sirve para verificar rapidamente que las condiciones basicas del sitio se cumplan
despues de cada cambio importante.

---

### Requisitos

- Windows PowerShell 5.1 o PowerShell 7+
- Conexion a internet (descarga el HTML de las URLs configuradas)

---

### Como ejecutar

Desde la raiz del repositorio:

```powershell
powershell tools/validate-localio.ps1
```

Contra localhost (app corriendo en modo desarrollo):

```powershell
powershell tools/validate-localio.ps1 -BaseUrl "http://localhost:5202" -SkipTls
```

El script:
- Devuelve exit code 0 si no hay errores.
- Devuelve exit code 1 si encuentra uno o mas errores.
- Muestra [OK], [FAIL] y [WARN] con color en la consola.

---

### Que valida

| Categoria             | Que busca                                                               |
|-----------------------|-------------------------------------------------------------------------|
| HTTP                  | Status 200 en todas las URLs                                           |
| Mojibake / encoding   | Caracteres Ã, Â, U+FFFD, &Atilde;, &Acirc; en el HTML renderizado     |
| Placeholders          | Textos como "foto pendiente", "Foto mascota", "imagen pendiente", etc. |
| noindex / nofollow    | Presencia de ambas directivas en demos privadas                        |
| Firma Localio         | "Demo privada creada por" presente y sin duplicar en demos             |
| Logo landing          | /images/brand/localio-logo-horizontal-transparent.png referenciado     |
| Favicon               | /images/brand/favicon.ico referenciado                                 |
| Secciones duplicadas  | Mas de 1 pd-hero, pd-services-grid o pd-footer__credits en demos      |

---

### Que NO valida

- Apariencia visual (colores, tamanos, layout)
- Funcionamiento de formularios y botones
- Velocidad de carga o performance
- SEO mas alla de noindex/nofollow
- Contenido de texto (servicios, horarios, precios)
- Imprimir correctamente en todos los browsers
- Comportamiento en mobile / responsive
- Correctitud semantica del HTML

Para eso, revisar visualmente en browser (desktop y mobile).

---

### URLs configuradas por defecto

| URL                              | Tipo           |
|----------------------------------|----------------|
| https://localio.com.ar/          | Landing        |
| https://localio.com.ar/demos/citivet | Demo privada |
| https://localio.com.ar/demos/urquiza | Demo privada |
| https://urquiza.localio.com.ar   | Subdominio demo |

Para cambiar las URLs, editar el array `$Pages` al comienzo del script.
Para agregar una nueva URL de demo privada, copiar una entrada existente con `IsDemo = $true`.
Para agregar una URL de landing o pagina publica, usar `IsLanding = $true` o ambos en $false.

---

### Como agregar una nueva demo privada

Abrir `validate-localio.ps1` y agregar una entrada en el array `$Pages`:

```powershell
@{ Url = "https://localio.com.ar/demos/mi-nueva-demo"; Label = "Demo NombreComercio"; IsLanding = $false; IsDemo = $true }
```

Si tiene subdominio, agregar tambien:

```powershell
@{ Url = "https://minuevademo.localio.com.ar"; Label = "Subdominio NombreComercio"; IsLanding = $false; IsDemo = $true }
```

---

### Nota sobre la advertencia [WARN] de texto Localio

El script busca `>Localio<` en el DOM. Si el markup tiene saltos de linea alrededor
del `<strong>Localio</strong>`, el WARN puede aparecer aunque la firma este
correctamente implementada. Es una advertencia de mantenimiento, no un error.
Verificar visualmente si aparece este WARN.

---

### Contexto

Este script fue creado para minimizar el uso de GitHub Copilot en auditorias
recurrentes. En lugar de pedir a Copilot que revise manualmente cada vez,
ejecutar este script y corregir solo los [FAIL] reportados.

Ver tambien: LOCALIO_PROJECT_STATE.md seccion 20 (regla 18) y .github/copilot-instructions.md.