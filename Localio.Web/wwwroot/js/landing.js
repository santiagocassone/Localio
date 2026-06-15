/* landing.js  –  Localio landing page
   Vanilla JS, sin dependencias.
   ---------------------------------------------------------------- */
(function () {
  'use strict';

  /* ── Header scroll effect ─────────────────────────────────────── */
  const header = document.querySelector('.l-header');
  if (header) {
    const onScroll = () => {
      header.classList.toggle('scrolled', window.scrollY > 10);
    };
    window.addEventListener('scroll', onScroll, { passive: true });
    onScroll();
  }

  /* ── Mobile nav toggle ────────────────────────────────────────── */
  const menuBtn  = document.getElementById('menuBtn');
  const navDrawer = document.getElementById('navDrawer');

  if (menuBtn && navDrawer) {
    menuBtn.addEventListener('click', () => {
      const isOpen = navDrawer.classList.toggle('open');
      menuBtn.setAttribute('aria-expanded', isOpen ? 'true' : 'false');
      menuBtn.setAttribute('aria-label', isOpen ? 'Cerrar menú' : 'Abrir menú');
    });

    // Cerrar al hacer click en un link del drawer
    navDrawer.querySelectorAll('a').forEach(link => {
      link.addEventListener('click', () => {
        navDrawer.classList.remove('open');
        menuBtn.setAttribute('aria-expanded', 'false');
        menuBtn.setAttribute('aria-label', 'Abrir menú');
      });
    });

    // Cerrar con Escape
    document.addEventListener('keydown', e => {
      if (e.key === 'Escape' && navDrawer.classList.contains('open')) {
        navDrawer.classList.remove('open');
        menuBtn.setAttribute('aria-expanded', 'false');
        menuBtn.setAttribute('aria-label', 'Abrir menú');
        menuBtn.focus();
      }
    });
  }

  /* ── Intersection Observer: fade-in on scroll ─────────────────── */
  if ('IntersectionObserver' in window) {
    const style = document.createElement('style');
    style.textContent = `
      .js-fade { opacity: 0; transform: translateY(18px); transition: opacity .45s ease, transform .45s ease; }
      .js-fade.visible { opacity: 1; transform: none; }
    `;
    document.head.appendChild(style);

    const targets = document.querySelectorAll(
      '.l-benefit-card, .l-forwho-chip, .l-process-step, .l-trust-card, .l-include-item, .l-problem__item, .l-demo-featured'
    );

    const observer = new IntersectionObserver(
      (entries) => {
        entries.forEach((entry, i) => {
          if (entry.isIntersecting) {
            // escalonado por índice dentro del grupo
            const delay = (entry.target.dataset.delay || 0);
            setTimeout(() => entry.target.classList.add('visible'), delay);
            observer.unobserve(entry.target);
          }
        });
      },
      { threshold: 0.12 }
    );

    targets.forEach((el, i) => {
      el.classList.add('js-fade');
      // calcular delay dentro del grupo de hermanos
      const siblings = el.parentElement ? Array.from(el.parentElement.children) : [];
      const idx = siblings.indexOf(el);
      el.dataset.delay = idx * 70;
      observer.observe(el);
    });
  }

})();
