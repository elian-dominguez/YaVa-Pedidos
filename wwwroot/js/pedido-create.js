
document.addEventListener("DOMContentLoaded", function () {

    const filas = document.querySelectorAll(".pedido-fila");
    const totalImporte = document.getElementById("pedido-total-importe");
    const resumenProductos = document.getElementById("pedido-productos-seleccionados");

    function actualizarTotal() {

        let total = 0;
        resumenProductos.replaceChildren();

        filas.forEach(function (fila) {

            const precio = Number(fila.dataset.precio);
            const input = fila.querySelector(".pedido-input-cantidad");
            const cantidad = Number(input.value);

            if (Number.isInteger(cantidad) && cantidad >= 0 && cantidad <= 50) {

                total += precio * cantidad;
                
                if (cantidad > 0) {
                    const nombre = fila.dataset.nombre;
                    const subtotal = precio * cantidad;

                    const producto = document.createElement("div");
                    producto.className = "pedido-resumen-info";

                    const descripcion = document.createElement("span");
                    descripcion.textContent = `${cantidad} × ${nombre}`;

                    const importe = document.createElement("strong");
                    importe.textContent = subtotal.toLocaleString("es-AR", {
                        style: "currency",
                        currency: "ARS",
                        maximumFractionDigits: 0
                    });

                    producto.append(descripcion, importe);
                    resumenProductos.appendChild(producto);
                }

            }

        });

        totalImporte.textContent = total.toLocaleString("es-AR", {
            style: "currency",
            currency: "ARS",
            maximumFractionDigits: 0
        });

        const ayuda = document.getElementById("pedido-resumen-ayuda");
        ayuda.hidden = total > 0;

    }

    filas.forEach(function (fila) {

        const input = fila.querySelector(".pedido-input-cantidad");

        input.addEventListener("input", actualizarTotal);

    });

    actualizarTotal();

});
