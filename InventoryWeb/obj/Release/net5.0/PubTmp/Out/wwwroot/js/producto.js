var datatable;

$(document).ready(function () {
    cargarTabla();
})


function cargarTabla() {
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
            "url": "/Admin/Producto/ObtenerTodos"
        },
        "columns": [
            { "data": "nombre" },
            { "data": "descripcion" },
            { "data": "precio" },
            { "data": "existencia" },
            { "data": "categoria.nombre" },
            { "data": "marca.nombre" },
            {
                "data": "productoId",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/Producto/Edit/${data}"
                               class="btn btn-success text-white" style="cursor:pointer">
                                <i class="fas fa-edit"></i>
                            </a>
                             <a href="/Admin/Producto/Delete/${data}"
                               class="btn btn-danger text-white" style="cursor:pointer">
                                <i class="fas fa-trash"></i>
                            </a>
                        </div>
                        `;
                },
            }
        ]
    });
}

