﻿// Agregar Cuenta por Pagar
document.getElementById("AddCuentaPagar").addEventListener("click", function () {
    $.ajax({
        url: '/CuentaPagars/Create',
        type: 'GET',
        success: function (result) {
            $('#newCuentaPagarModal .modal-body').html(result);
            $('#newCuentaPagarModal').modal('show');
        },
        error: function (error) {
            console.error("Error al cargar la vista parcial", error);
        }
    });
});

document.addEventListener('click', function (e) {
    if (e.target && e.target.id === 'submitCuentaPagar') {
        var formData = new FormData(document.getElementById('CuentaPagarForm'));

        fetch('/CuentaPagars/Create', {
            method: 'POST',
            body: formData,
            headers: {
                'RequestVerificationToken': document.getElementsByName('__RequestVerificationToken')[0].value
            }
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    $('#newCuentaPagarModal').modal('hide');
                    Swal.fire({
                        title: '¡Éxito!',
                        text: data.message,
                        icon: 'success'
                    }).then((result) => {
                        if (result.isConfirmed || result.isDismissed) {
                            window.location.reload();
                        }
                    });
                } else {
                    Swal.fire({
                        title: 'Error',
                        text: data.message,
                        icon: 'error'
                    });
                }
            })
            .catch(error => {
                Swal.fire({
                    title: 'Error',
                    text: 'Hubo un problema con la solicitud.',
                    icon: 'error'
                });
            });
    }
});

// Editar Cuenta por Pagar
document.querySelectorAll('.edit-CuentaPagar').forEach(button => {
    button.addEventListener('click', function () {
        var IdCuentaPagar = this.getAttribute('data-id');
        fetch(`/CuentaPagars/Edit/${IdCuentaPagar}`)
            .then(response => response.text())
            .then(html => {
                document.querySelector('#editCuentaPagarModal .modal-body').innerHTML = html;
                $('#editCuentaPagarModal').modal('show');
                document.querySelector('#editCuentaPagarModal #editCuentaPagarForm').addEventListener('submit', function (e) {
                    e.preventDefault();

                    var formData = new FormData(this);

                    fetch(`/CuentaPagars/Edit/${IdCuentaPagar}`, {
                        method: 'POST',
                        body: formData,
                        headers: {
                            'RequestVerificationToken': document.getElementsByName('__RequestVerificationToken')[0].value
                        }
                    })
                        .then(response => response.json())
                        .then(data => {
                            $('#editCuentaPagarModal').modal('hide');
                            if (data.success) {
                                Swal.fire({
                                    title: '¡Éxito!',
                                    text: data.message,
                                    icon: 'success'
                                }).then(() => {
                                    window.location.reload();
                                });
                            } else {
                                Swal.fire({
                                    title: 'Error',
                                    text: data.message,
                                    icon: 'error'
                                });
                            }
                        })
                        .catch(error => {
                            $('#editCuentaPagarModal').modal('hide');
                            Swal.fire({
                                title: 'Error',
                                text: 'Hubo un problema con la solicitud.',
                                icon: 'error'
                            });
                        });
                });
            })
            .catch(error => console.error('Error:', error));
    });
});

// Eliminar Cuenta por Pagar
document.querySelectorAll('.delete-CuentaPagar').forEach(button => {
    button.addEventListener('click', function () {
        var IdCuentaPagar = this.getAttribute('data-id');

        Swal.fire({
            title: '¿Estás seguro?',
            text: "¡No podrás revertir esto!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Sí, elimínalo!',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                fetch(`/CuentaPagars/Delete/${IdCuentaPagar}`, {
                    method: 'POST',
                    headers: {
                        'RequestVerificationToken': document.getElementsByName('__RequestVerificationToken')[0].value
                    }
                })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        Swal.fire(
                            '¡Eliminado!',
                            'El impuesto ha sido eliminado.',
                            'success'
                        ).then(() => {
                            window.location.reload();
                        });
                    } else {
                        Swal.fire(
                            'Error',
                            'Hubo un problema al eliminar el impuesto.',
                            'error'
                        );
                    }
                })
                .catch(error => {
                    Swal.fire(
                        'Error',
                        'Hubo un problema con la solicitud.',
                        'error'
                    );
                });
            }
        })
    });
});