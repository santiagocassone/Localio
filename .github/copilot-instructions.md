# Copilot Instructions — Localio

## Proyecto

Localio es un servicio web para comercios locales argentinos, construido con ASP.NET Core Razor Pages (.NET 10), datos en JSON local, sin base de datos ni panel admin.
El repositorio tiene un solo proyecto: `Localio.Web`.

Para el estado completo del proyecto, ver `LOCALIO_PROJECT_STATE.md` en la raíz del repo.

---

## Reglas generales

- Responder siempre en español.
- Actuar como arquitecto .NET 10 / C# / Razor Pages para Localio.
- Priorizar soluciones simples, mantenibles y de bajo costo operativo.
- No agregar base de datos, CMS, panel admin ni dependencias NuGet salvo pedido explícito.
- No sobreingeniería: la menor cantidad de cambios que resuelva el problema.

---

## Rutas que no se deben romper

- `/` — landing pública de Localio (nunca tocar sin pedido explícito)
- `/demos` — lista de demos genéricas
- `/demo/{siteId}` — demo genérica por siteId
- `/demos/citivet` — demo privada de Veterinaria Citivet

---

## CSS / JS

- Usar `asp-append-version="true"` en todo CSS y JS propio.
- Mantener SVGs inline en la landing; no introducir librerías de iconos externas.
- No generar scroll horizontal.
- Cuidar Mobile Safari: usar `min-height: 100dvh`, evitar `vh` puro, evitar `position: fixed` sin `env(safe-area-inset-*)`.

---

## Demos privadas

- `noindex, nofollow` por defecto en toda demo privada.
- Solo `ActivePublic` y `Converted` son indexables — lógica en `PrivateDemoService`, no duplicar.
- No enlazar demos privadas desde la landing pública.
- No exponer datos de prospectos reales en código público ni en comentarios.

### Regla anti-duplicados (obligatoria al modificar demos)

Cuando se modifique una demo privada existente, tratar la tarea como **reemplazo/refactor**, no como agregado incremental. No agregar una segunda versión de una sección sin eliminar la anterior. No ocultar contenido viejo con CSS. No dejar contenido viejo renderizado en la ruta afectada.

**Alcance normal:** la demo modificada y sus secciones tocadas; partials/layouts compartidos solo si fueron modificados; HTML renderizado de la ruta afectada. No revisar otras demos ni todo el proyecto salvo que se haya modificado un componente compartido o el cambio sea explícitamente global.

**Checklist acotado — duplicaciones (solo para la demo afectada):**

- [ ] La demo afectada tiene un solo hero.
- [ ] La demo afectada tiene una sola sección principal de servicios, salvo decisión explícita.
- [ ] La demo afectada no tiene dos bloques de contacto equivalentes.
- [ ] La demo afectada no tiene dos footers.
- [ ] No queda contenido viejo renderizado en la ruta afectada.
- [ ] No quedan placeholders o textos obsoletos en la ruta afectada.
- [ ] Mantener `noindex, nofollow` mientras el estado sea `ActivePrivate`.

### Regla de encoding/caracteres (obligatoria al modificar demos)

Antes de dar por terminado un cambio en una demo privada, validar encoding y caracteres en español **dentro del alcance del cambio**. Este problema ya ocurrió en **Citivet** y **Veterinaria Urquiza**.

**Alcance normal:** archivos modificados de esa demo; partials/layouts compartidos solo si fueron modificados; HTML renderizado de la ruta afectada. No revisar todo el proyecto salvo que se haya modificado un componente compartido o el cambio sea global.

**Buscar en los archivos y HTML afectados:**
`Ã`, `Â`, `�`, `ClÃ`, `SÃ`, `atenciÃ`, `ubicaciÃ`, `calificaciÃ`, `reseÃ`, `artÃ`, `mÃ`

**Confirmar que se rendericen correctamente (en la ruta afectada):**
Clínica, Cardiología, Cirugía, Dermatología, Endocrinología, Radiología, Ecografía, atención, ubicación, calificación, reseñas, artículos, Sábados, Cómo

**Checklist de cierre — encoding (solo para la ruta afectada):**

- [ ] No aparecen `Ã`, `Â` ni `�` en el HTML renderizado de la ruta afectada.
- [ ] Los acentos y ñ se ven correctamente en el browser.
- [ ] No se eliminaron acentos como falsa solución (`Clinica` en lugar de `Clínica` **no es aceptable**).
- [ ] Los archivos **modificados** están guardados como UTF-8 sin BOM.
- [ ] Si la demo usa `Layout = null`, incluye `<meta charset="utf-8">` en el `<head>` antes de cualquier texto con acentos.

Ver regla 17 de `LOCALIO_PROJECT_STATE.md` para los comandos PowerShell de diagnóstico y corrección.

---

## Datos de negocios

- No inventar reseñas, matrículas, trayectoria, horarios, servicios ni precios.
- Solo documentar o mostrar datos confirmados explícitamente por el usuario.

---

## Cuando realices cambios relevantes

Actualizar `LOCALIO_PROJECT_STATE.md` para reflejar el estado real del proyecto:
- Si se agrega una ruta nueva, agregarla en la sección 5.
- Si cambia el estado de Citivet, actualizarlo en las secciones 9 y 10.
- Si se implementa algo pendiente, moverlo de "Pendiente" a "Implementado".
- Si se detectan nuevos riesgos, agregarlos en la sección 19.
- Si se modifica una demo privada, aplicar el checklist anti-duplicados **y** el checklist de encoding/caracteres de la sección "Demos privadas" de este archivo.
