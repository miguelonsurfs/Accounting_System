﻿// Paginacion
var currentPage = 1;
var rowsPerPage = 10;

function setupPagination() {
    var table = document.querySelector('#tablaTipo tbody');
    var rows = table.rows;
    var totalRows = rows.length;
    var totalPages = Math.ceil(totalRows / rowsPerPage);

    function showPage(page) {
        for (let i = 0; i < totalRows; i++) {
            rows[i].style.display = (i >= (page - 1) * rowsPerPage && i < page * rowsPerPage) ? "" : "none";
        }

        document.getElementById('pageIndicator').textContent = `${page} / ${totalPages}`;
    }

    window.changePage = function (direction) {
        currentPage += direction;

        if (currentPage < 1) currentPage = 1;
        if (currentPage > totalPages) currentPage = totalPages;

        showPage(currentPage);
    };

    showPage(currentPage);
}

document.addEventListener('DOMContentLoaded', setupPagination);

// Busqueda
document.getElementById('busqueda').addEventListener('keyup', function () {
    var searchTerm = this.value.toLowerCase();
    var rows = document.querySelectorAll('#tablaTipo tbody tr');

    rows.forEach(row => {
        var text = row.textContent.toLowerCase();
        row.style.display = text.includes(searchTerm) ? '' : 'none';
    });
});

// Ordernar tabla
document.querySelectorAll('#tablaTipo th').forEach(header => {
    if (!header.id.includes("acciones")) {
        header.addEventListener('click', function () {
            var table = document.querySelector('#tablaTipo');
            var tableBody = table.querySelector('tbody');
            var rowsArray = Array.from(tableBody.rows);
            var column = this.cellIndex;
            var isAscending = header.classList.contains('asc');
            var isDateColumn = this.id === 'Fecha';

            rowsArray.sort(function (rowA, rowB) {
                var textA = rowA.cells[column].textContent.trim();
                var textB = rowB.cells[column].textContent.trim();

                if (isDateColumn) {
                    var dateA = new Date(textA.split('/').reverse().join('-'));
                    var dateB = new Date(textB.split('/').reverse().join('-'));
                    return (dateA - dateB) * (isAscending ? -1 : 1);
                } else {
                    return textA.localeCompare(textB, undefined, { numeric: true }) * (isAscending ? -1 : 1);
                }
            });

            document.querySelectorAll('#tablaTipo th').forEach(th => {
                th.innerHTML = th.textContent;
                th.classList.remove('asc', 'desc');
            });

            var newIconHtml = isAscending ? '<i class="fas fa-arrow-down"></i>' : '<i class="fas fa-arrow-up"></i>';
            this.innerHTML += ' ' + newIconHtml;
            this.classList.toggle('asc', !isAscending);
            this.classList.toggle('desc', isAscending);

            rowsArray.forEach(row => tableBody.appendChild(row));
        });
    }
});

// Agregar tipo de pago
document.getElementById("AddTipoP").addEventListener("click", function () {
    $("#newTipoPModal").modal("show");
});

document.getElementById('submitTipoPForm').addEventListener('click', function () {
    var formData = new FormData(document.getElementById('TipoPForm'));

    fetch('/TipoPagos/Create', {
        method: 'POST',
        body: formData,
        headers: {
            'RequestVerificationToken': document.getElementsByName('__RequestVerificationToken')[0].value
        }
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                $('#newTipoPModal').modal('hide');
                Swal.fire({
                    title: '¡Éxito!',
                    text: data.message,
                    icon: 'success',
                    confirmButtonColor: '#0DBCB5'
                }).then((result) => {
                    if (result.isConfirmed || result.isDismissed) {
                        window.location.reload();
                    }
                });
            } else {
                let errorMessage = "";
                if (data.errors && data.errors.length > 0) {
                    errorMessage += "\n" + data.errors.join("\n");
                }

                Swal.fire({
                    title: 'Error',
                    text: errorMessage,
                    icon: 'warning',
                    confirmButtonColor: '#0DBCB5'
                });
            }
        })
        .catch(error => {
            Swal.fire({
                title: 'Error',
                text: 'Hubo un problema con la solicitud.',
                icon: 'error',
                confirmButtonColor: '#0DBCB5'
            });
        });
});

// Editar tipo de pago
document.querySelectorAll('.edit-TipoP').forEach(button => {
    button.addEventListener('click', function () {
        var tipoId = this.getAttribute('data-id');
        fetch(`/TipoPagos/Edit/${tipoId}`)
            .then(response => response.text())
            .then(html => {
                document.querySelector('#editTipoPModal .modal-body').innerHTML = html;
                $('#editTipoPModal').modal('show');

                document.querySelector('#editTipoPModal #editTipoPForm').addEventListener('submit', function (e) {
                    e.preventDefault();

                    var formData = new FormData(this);

                    fetch(`/TipoPagos/Edit/${tipoId}`, {
                        method: 'POST',
                        body: formData,
                        headers: {
                            'RequestVerificationToken': document.getElementsByName('__RequestVerificationToken')[0].value
                        }
                    })
                        .then(response => response.json())
                        .then(data => {
                            $('#editTipoPModal').modal('hide');
                            if (data.success) {
                                Swal.fire({
                                    title: '¡Éxito!',
                                    text: data.message,
                                    icon: 'success',
                                    confirmButtonColor: '#0DBCB5'
                                }).then(() => {
                                    window.location.reload();
                                });
                            } else {
                                let errorMessage = "";
                                if (data.errors && data.errors.length > 0) {
                                    errorMessage += "\n" + data.errors.join("\n");
                                }

                                Swal.fire({
                                    title: 'Error',
                                    text: errorMessage,
                                    icon: 'warning',
                                    confirmButtonColor: '#0DBCB5'
                                });
                            }
                        })
                        .catch(error => {
                            $('#editTipoPModal').modal('hide');
                            Swal.fire({
                                title: 'Error',
                                text: 'Hubo un problema con la solicitud.',
                                icon: 'error',
                                confirmButtonColor: '#0DBCB5'
                            });
                        });
                });
            })
            .catch(error => console.error('Error:', error));
    });
});

// Eliminar tipo de pago
document.querySelectorAll('.delete-TipoP').forEach(button => {
    button.addEventListener('click', function () {
        var tipoId = this.getAttribute('data-id');

        Swal.fire({
            title: '¿Estás seguro?',
            text: "¡No podrás revertir este cambio!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#0DBCB5',
            cancelButtonColor: '#9DB2BF',
            confirmButtonText: 'Sí, elimínalo!',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                fetch(`/TipoPagos/Delete/${tipoId}`, {
                    method: 'POST',
                    headers: {
                        'RequestVerificationToken': document.getElementsByName('__RequestVerificationToken')[0].value
                    }
                })
                    .then(response => response.json())
                    .then(data => {
                        if (data.success) {
                            Swal.fire({
                                title: '¡Eliminado!',
                                text: data.message,
                                icon: 'success',
                                confirmButtonColor: '#0DBCB5'
                            }).then(() => {
                                window.location.reload();
                            });
                        } else {
                            Swal.fire({
                                title: 'Error',
                                text: 'Hubo un problema al eliminar el tipo de pago.',
                                icon: 'error',
                                confirmButtonColor: '#0DBCB5'
                            });
                        }
                    })
                    .catch(error => {
                        Swal.fire({
                            title: 'Error',
                            text: 'Hubo un problema con la solicitud.',
                            icon: 'error',
                            confirmButtonColor: '#0DBCB5'
                        });
                    });
            }
        })
    });
});