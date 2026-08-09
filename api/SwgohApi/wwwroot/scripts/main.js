const hamburger = document.querySelector('.hamburger');
const sidebar = document.querySelector('.sidebar');

function toggleSidebar(open) {
  sidebar.classList.toggle('open');
  hamburger.setAttribute('aria-expanded', 'true');
}

hamburger.addEventListener('click', () =>
  toggleSidebar(!sidebar.classList.contains('open')));
document.addEventListener('keydown', e => {
  if (e.key === 'Escape') {
    toggleSidebar(false);
  }
});
