var datatable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    datatable = $('#tblDatos').DataTable({
        "rowReorder": {
            "selector": 'td:nth-child(2)'
        },
        "responsive": true,
        "scrollX": true,
        "language": {
            "url": "https://cdn.datatables.net/plug-ins/1.10.21/i18n/Spanish.json"
        },
        "ajax": {
            "url": "/Inventario/Inventario/ObtenerHistorial"
        },
        "columns": [
            {
                "data": "fechaInicial", "width": "15%",
                "render": function (data) {
                    var d = new Date(data);
                    return d.toLocaleString();
                }
            },
            {
                "data": "fechaFinal", "width": "15%",
                "render": function (data) {
                    var d = new Date(data);
                    return d.toLocaleString();
                }
            },
            { "data": "bodega.nombre", "width": "15%" },
            {
                "data": function nombreUsuario(data) {
                    return data.usuarioAplicacion.nombres + " " + data.usuarioAplicacion.apellidos;
                }, "width": "20%"
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Inventario/Inventario/DetalleHistorial/${data}" class="btn btn-primary text-white" style="cursor:pointer">
                             Detalle
                            </a>                           
                        </div>
                        `;
                }, "width": "10%"
            }
        ]
    });
}


