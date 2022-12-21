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
            "url": "/Admin/Compania/ObtenerTodos"
        },
        "columns": [
            { "data": "nombre", "width": "15%" },
            { "data": "descripcion", "width": "15%" },
            { "data": "pais", "width": "15%" },
            { "data": "ciudad", "width": "15%" },
            { "data": "telefono", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/Compania/Upsert/${data}" class="btn btn-success text-white" style="cursor:pointer">
                                <i class="fas fa-edit"></i>
                            </a>                            
                        </div>
                        `;
                }, "width": "20%"
            }
        ]
    });
}