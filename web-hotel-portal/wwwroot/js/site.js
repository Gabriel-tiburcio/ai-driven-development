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

// Toast on page load: reads the message/type the server stashed in TempData (rendered as
// data attributes on <body> by _Layout.cshtml) and shows it via SweetAlert2 once.
(function () {
  if (typeof Swal === 'undefined') return;
  var message = document.body.dataset.toastMessage;
  if (!message) return;

  Swal.fire({
    icon: document.body.dataset.toastType || 'success',
    title: message,
    toast: true,
    position: 'top-end',
    timer: 3000,
    showConfirmButton: false,
  });
})();

// Delete confirmation via SweetAlert2 instead of the native confirm() dialog. Any <form>
// tagged with .js-confirm-delete gets intercepted; a data-confirm-submitted flag lets the
// programmatic re-submit through without looping back into this same handler.
(function () {
  if (typeof Swal === 'undefined') return;

  document.addEventListener('submit', function (e) {
    var form = e.target;
    if (!(form instanceof HTMLFormElement) || !form.classList.contains('js-confirm-delete')) return;
    if (form.dataset.confirmSubmitted === 'true') return;

    e.preventDefault();
    Swal.fire({
      title: 'Tem certeza?',
      text: form.dataset.confirmMessage || 'Essa ação não pode ser desfeita.',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Excluir',
      cancelButtonText: 'Cancelar',
      confirmButtonColor: '#d33',
    }).then(function (result) {
      if (!result.isConfirmed) return;
      form.dataset.confirmSubmitted = 'true';
      form.requestSubmit ? form.requestSubmit() : form.submit();
    });
  });
})();

// Edit modals: any [data-modal-target] button opens the <dialog> with that id; any
// [data-modal-close] button closes its nearest <dialog>. Native <dialog> already handles
// backdrop click / Escape via the browser, except backdrop click, which we wire manually.
(function () {
  document.querySelectorAll('[data-modal-target]').forEach(function (btn) {
    var dialog = document.getElementById(btn.dataset.modalTarget);
    if (dialog) btn.addEventListener('click', function () { dialog.showModal(); });
  });

  document.querySelectorAll('dialog.modal').forEach(function (dialog) {
    dialog.querySelectorAll('[data-modal-close]').forEach(function (btn) {
      btn.addEventListener('click', function () { dialog.close(); });
    });
    dialog.addEventListener('click', function (e) {
      if (e.target === dialog) dialog.close();
    });
  });
})();
