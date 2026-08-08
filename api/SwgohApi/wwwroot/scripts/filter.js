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
