// Mobile off-canvas sidebar: toggled by the hamburger button in the
// topbar, dismissed by tapping the overlay, a nav link, or resizing
// back to desktop width.
(function () {
  var toggle = document.getElementById('menuToggle');
  var sidebar = document.getElementById('appSidebar');
  var overlay = document.getElementById('sidebarOverlay');
  if (!toggle || !sidebar || !overlay) return;

  function setOpen(isOpen) {
    sidebar.classList.toggle('is-open', isOpen);
    overlay.classList.toggle('is-open', isOpen);
    toggle.setAttribute('aria-expanded', isOpen ? 'true' : 'false');
    document.body.style.overflow = isOpen ? 'hidden' : '';
  }

  toggle.addEventListener('click', function () {
    setOpen(!sidebar.classList.contains('is-open'));
  });
  overlay.addEventListener('click', function () { setOpen(false); });
  sidebar.querySelectorAll('a, button').forEach(function (el) {
    el.addEventListener('click', function () { setOpen(false); });
  });
  window.addEventListener('resize', function () {
    if (window.innerWidth > 860) setOpen(false);
  });
})();
