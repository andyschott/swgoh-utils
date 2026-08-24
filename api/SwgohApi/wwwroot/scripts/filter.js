function onFilter(event) {
  const searchTerm = event.target.value;
  const rows = document.querySelectorAll('#data-table tbody tr');
  if (searchTerm.length === 0) {
    for (let index = 0; index < rows.length; index++) {
      rows[index].style.display = '';
    }
    return;
  }

  for (let index = 0; index < rows.length; index++) {
    const row = rows[index];
    const th = row.cells[0];
    const match = th.innerText.toLocaleLowerCase().includes(searchTerm.toLocaleLowerCase());
    rows[index].style.display = match ? '' : 'none';
  }
}

document.getElementById('filter')
  .addEventListener("input", onFilter);

function sortTable(th, colIndex) {
  const table = th.closest('table');
  const tbody = table.querySelector('tbody');
  const rows = Array.from(tbody.querySelectorAll('tr'));
  const type = th.dataset.type || 'text';

  const ascending = th.dataset.sortDir !== 'asc';
  table.querySelectorAll('th').forEach(h => delete h.dataset.sortDir);
  th.dataset.sortDir = ascending ? 'asc' : 'desc';

  rows.sort((a, b) => {
    let valA = a.children[colIndex].textContent || 0;
    let valB = b.children[colIndex].textContent || 0;

    if (type === 'number') {
      valA = parseFloat(valA) || 0;
      valB = parseFloat(valB) || 0;
    } else if (type === 'date') {
      valA = new Date(valA);
      valB = new Date(valB);
    }

    if (valA < valB) {
      return ascending ? -1 : 1;
    }
    if (valA > valB) {
      return ascending ? 1 : -1;
    }
    return 0;
  });

  rows.forEach(row => tbody.appendChild(row));

  table.querySelectorAll('th')
    .forEach(h => h.classList.remove('sort-asc', 'sort-desc'));
  th.classList.add(ascending ? 'sort-asc' : 'sort-desc');
}

document.querySelectorAll('#data-table thead th').forEach((th, colIndex) => {
  th.style.cursor = 'pointer';
  th.addEventListener('click', () => sortTable(th, colIndex));
});

function setDefaultSortColumn() {
  const dataTable = document.querySelector('#data-table');
  const defaultSortColumn = dataTable.dataset.defaultSortColumn;
  const defaultSortAscending = dataTable.dataset.defaultSortAscending === 'true';
  const tableHeaders = dataTable.querySelectorAll('thead th');

  for (const entry of tableHeaders.entries()) {
    if (entry[1].innerText === defaultSortColumn) {
      entry[1].dataset.sortDir = defaultSortAscending ? 'asc' : 'desc';
      entry[1].classList.add(defaultSortAscending ? 'sort-asc' : 'sort-desc');

      break;
    }
  }
}
setDefaultSortColumn();


