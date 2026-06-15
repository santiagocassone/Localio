/**
 * localio-analytics.js
 * ─────────────────────────────────────────────────────────────────────────────
 * Analytics liviano para Localio.
 * Depende de Microsoft Clarity (window.clarity) si está disponible.
 * No bloquea la navegación si Clarity no está cargado.
 *
 * Para conectar otra herramienta en el futuro, reemplazá el bloque
 * marcado con "// [ADAPTER]" en window.localioAnalytics.track().
 * ─────────────────────────────────────────────────────────────────────────────
 */
(function () {
    'use strict';

    // ── Metadata de página ────────────────────────────────────────────────────
    // Se lee desde el elemento <main> o <body> con atributos data-*.
    function getPageMeta() {
        var root = document.querySelector('[data-page-type]') || document.body;
        return {
            pageType:     root.getAttribute('data-page-type')     || '',
            demoId:       root.getAttribute('data-demo-id')       || '',
            businessName: root.getAttribute('data-business-name') || ''
        };
    }

    // ── API pública ───────────────────────────────────────────────────────────
    window.localioAnalytics = {

        /**
         * Enviá un evento con metadata opcional.
         * @param {string} eventName  p.ej. "click_whatsapp"
         * @param {object} [extra]    pares clave/valor adicionales
         */
        track: function (eventName, extra) {
            if (!eventName) return;

            var meta   = getPageMeta();
            var merged = Object.assign({}, meta, extra || {});

            // [ADAPTER] Microsoft Clarity ─────────────────────────────────────
            // Para cambiar de herramienta, reemplazá este bloque.
            if (typeof window.clarity === 'function') {
                try {
                    // Evento nombrado
                    window.clarity('event', eventName);

                    // Tags de metadata
                    Object.keys(merged).forEach(function (key) {
                        if (merged[key]) {
                            window.clarity('set', key, merged[key]);
                        }
                    });
                } catch (e) {
                    // Clarity puede no estar listo; no propagamos el error.
                }
            }
            // ─────────────────────────────────────────────────────────────────
        }
    };

    // ── Click tracking ────────────────────────────────────────────────────────
    // Captura clics en elementos con atributo data-analytics-event.
    document.addEventListener('click', function (e) {
        var target = e.target.closest('[data-analytics-event]');
        if (!target) return;

        var eventName = target.getAttribute('data-analytics-event');
        window.localioAnalytics.track(eventName);
    }, { passive: true });

    // ── Section visibility tracking ───────────────────────────────────────────
    // Dispara section_contact_view cuando la sección de contacto es visible.
    // Solo se dispara una vez por carga de página.
    if (typeof IntersectionObserver !== 'undefined') {
        var contactSection = document.querySelector('[data-analytics-section="contact"]');
        if (contactSection) {
            var fired = false;
            var observer = new IntersectionObserver(function (entries) {
                entries.forEach(function (entry) {
                    if (entry.isIntersecting && !fired) {
                        fired = true;
                        window.localioAnalytics.track('section_contact_view');
                        observer.disconnect();
                    }
                });
            }, { threshold: 0.2 });
            observer.observe(contactSection);
        }
    }

})();
