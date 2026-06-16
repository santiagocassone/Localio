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

Las modificaciones de demos privadas existentes deben tratarse como **reemplazo/refactor**, no como agregado incremental.

Antes de agregar una nueva versión de una sección, identificar la sección previa equivalente y eliminarla del markup renderizado. No ocultar contenido viejo con CSS. No dejar duplicados de hero, servicios, galería, contacto, header o footer. Al finalizar, buscar textos obsoletos en el repo y validar que el HTML renderizado no contenga placeholders ni contenido anterior.

**Checklist obligatorio al modificar o crear una demo privada:**

- [ ] Buscar textos viejos o placeholders (`pendiente`, `placeholder`, `Foto principal`, `imagen pendiente`, etc.) y eliminarlos del markup.
- [ ] Validar que haya un solo `<header>` y un solo hero.
- [ ] Validar que haya una sola sección principal de servicios.
- [ ] Validar que no haya sección de galería si la demo no usa imágenes reales.
- [ ] Validar que haya un solo `<footer>`.
- [ ] Validar que no haya contenido viejo oculto con `display:none`.
- [ ] Validar mobile sin scroll horizontal.
- [ ] Mantener `noindex, nofollow` mientras el estado sea `ActivePrivate`.

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
- Si se modifica una demo privada, aplicar el checklist anti-duplicados de la sección "Demos privadas" de este archivo.
