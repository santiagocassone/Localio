/**
 * Localio — minimal vanilla JS
 * Mobile menu · scroll animations · WhatsApp tracking · catalog tabs · smooth anchor
 */
(function () {
  'use strict';

  // ── Mobile navbar toggle ─────────────────────────────────────────────────
  function initNavbar() {
    const toggle = document.querySelector('.navbar-toggle');
    const menu   = document.querySelector('.navbar-mobile');
    if (!toggle || !menu) return;

    toggle.addEventListener('click', () => {
      const open = menu.classList.toggle('is-open');
      toggle.classList.toggle('is-open', open);
      toggle.setAttribute('aria-expanded', String(open));
    });

    // Close when a mobile link is tapped
    menu.querySelectorAll('a').forEach(a => {
      a.addEventListener('click', () => {
        menu.classList.remove('is-open');
        toggle.classList.remove('is-open');
        toggle.setAttribute('aria-expanded', 'false');
      });
    });

    // Close on outside click
    document.addEventListener('click', e => {
      const nav = toggle.closest('.localio-navbar');
      if (nav && !nav.contains(e.target)) {
        menu.classList.remove('is-open');
        toggle.classList.remove('is-open');
      }
    });
  }

  // ── Scroll-triggered entrance animations (IntersectionObserver) ──────────
  function initAnimations() {
    if (!('IntersectionObserver' in window)) {
      // Fallback: show everything immediately
      document.querySelectorAll('[data-animate]').forEach(el => el.classList.add('is-visible'));
      return;
    }

    const observer = new IntersectionObserver((entries) => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          entry.target.classList.add('is-visible');
          observer.unobserve(entry.target);
        }
      });
    }, { threshold: 0.12, rootMargin: '0px 0px -40px 0px' });

    document.querySelectorAll('[data-animate]').forEach(el => observer.observe(el));
  }

  // ── WhatsApp click tracking ───────────────────────────────────────────────
  function initWhatsAppTracking() {
    document.querySelectorAll('a[data-wa-track]').forEach(link => {
      link.addEventListener('click', () => {
        const label = link.dataset.waTrack || 'unknown';
        // Custom event for analytics integrations
        window.dispatchEvent(new CustomEvent('localio:wa_click', { detail: { label } }));
        // Store to localStorage for simple in-house reporting
        try {
          const key  = 'localio_wa_clicks';
          const data = JSON.parse(localStorage.getItem(key) || '[]');
          data.push({ label, ts: new Date().toISOString() });
          localStorage.setItem(key, JSON.stringify(data.slice(-50)));
        } catch (_) { /* ignore */ }
      });
    });
  }

  // ── Catalog tabs ─────────────────────────────────────────────────────────
  function initCatalogTabs() {
    document.querySelectorAll('.catalog-tabs').forEach(tabsEl => {
      const catalog = tabsEl.closest('.localio-catalog');
      if (!catalog) return;

      tabsEl.querySelectorAll('.catalog-tab').forEach(tab => {
        tab.addEventListener('click', () => {
          const target = tab.dataset.target;
          tabsEl.querySelectorAll('.catalog-tab').forEach(t => t.classList.remove('active'));
          catalog.querySelectorAll('.catalog-category').forEach(c => c.classList.remove('active'));
          tab.classList.add('active');
          const targetEl = catalog.querySelector(`.catalog-category[data-cat="${target}"]`);
          if (targetEl) targetEl.classList.add('active');
        });
      });
    });
  }

  // ── Smooth anchor scroll (offsets for sticky navbar) ─────────────────────
  function initSmoothScroll() {
    const navbarHeight = () => {
      const nav = document.querySelector('.localio-navbar');
      return nav ? nav.offsetHeight + 8 : 8;
    };

    document.querySelectorAll('a[href^="#"]').forEach(a => {
      a.addEventListener('click', e => {
        const id = a.getAttribute('href')?.slice(1);
        if (!id) return;
        const target = document.getElementById(id);
        if (!target) return;
        e.preventDefault();
        const top = target.getBoundingClientRect().top + window.scrollY - navbarHeight();
        window.scrollTo({ top, behavior: 'smooth' });
      });
    });
  }

  // ── Init ──────────────────────────────────────────────────────────────────
  document.addEventListener('DOMContentLoaded', () => {
    initNavbar();
    initAnimations();
    initWhatsAppTracking();
    initCatalogTabs();
    initSmoothScroll();
  });
})();
