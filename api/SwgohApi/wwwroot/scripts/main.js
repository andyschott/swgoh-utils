const hamburger = document.querySelector('.hamburger');
const sidebar = document.querySelector('.sidebar');
const overlay = document.querySelector('.overlay');

function toggleSidebar(open) {
  sidebar.classList.toggle('open', open);
  overlay.classList.toggle('visible', open);
  hamburger.setAttribute('aria-expanded', open);
}

hamburger.addEventListener('click', () =>
  toggleSidebar(!sidebar.classList.contains('open')));

overlay.addEventListener('click', () =>
  toggleSidebar(false));

document.addEventListener('keydown', e => {
  if (e.key === 'Escape') {
    toggleSidebar(false);
  }
});

