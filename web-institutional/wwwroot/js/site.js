// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Close the mobile navbar menu after tapping a link, so it doesn't stay
// open covering the page content.
(function () {
  var navCollapse = document.querySelector('.navbar-collapse');
  if (!navCollapse) return;

  navCollapse.querySelectorAll('.nav-link, .btn').forEach(function (el) {
    el.addEventListener('click', function () {
      if (!navCollapse.classList.contains('show')) return;
      var instance = window.bootstrap && window.bootstrap.Collapse
        ? window.bootstrap.Collapse.getOrCreateInstance(navCollapse)
        : null;
      if (instance) instance.hide();
    });
  });
})();
