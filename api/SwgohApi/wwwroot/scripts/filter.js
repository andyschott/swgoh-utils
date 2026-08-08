const items = [];

function onFilter(event) {
  const searchTerm = event.target.value;
  if (searchTerm.length === 0) {
    const rows = getDataTableRows();
    for (let index = 0; index < rows.length; index++) {
      rows[index].style.display = '';
    }
    return;
  }

  for (let index = 0; index < items.length; index++) {
    const item = items[index];
    const match = item.name.toLocaleLowerCase().includes(searchTerm.toLocaleLowerCase());
    item.row.style.display = match ? '' : 'none';
  }
}

function getDataTableRows() {
  return document.querySelectorAll('#data-table tbody tr');
}

document.getElementById('filter')
  .addEventListener("input", onFilter);

document.addEventListener('DOMContentLoaded', () => {
  const rows = getDataTableRows();
  for (let index = 0; index < rows.length; index++) {
    const row = rows[index];
    const th = row.cells[0];
    items.push({
      name: th.innerText,
      row: row
    });
  }
});
