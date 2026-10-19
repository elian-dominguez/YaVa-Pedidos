const connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/pedidos")
    .withAutomaticReconnect()
    .build();

const badge = document.getElementById("estado-conexion");
const avisos = document.getElementById("avisos");
const tablero = document.getElementById("tablero");

function marcarConexion(texto, color) {
    badge.textContent = texto;
    badge.className = "badge bg-" + color + " ms-1";
}

async function refrescarTablero() {
    const respuesta = await fetch("/Pedidos/Tablero", { headers: { "X-Requested-With": "fetch" } });
    tablero.innerHTML = await respuesta.text();
}

function mostrarAviso(pedido) {
    const aviso = document.createElement("div");
    aviso.className = "alert alert-info py-2";
    aviso.textContent = "Mesa " + pedido.mesa + ": " + pedido.estado;
    avisos.prepend(aviso);
    setTimeout(() => aviso.remove(), 5000);
}

connection.on("PedidoActualizado", async (pedido) => {
    await refrescarTablero();
    mostrarAviso(pedido);
});

connection.onreconnecting(() => marcarConexion("reconectando...", "warning"));

connection.onreconnected(async () => {
    marcarConexion("en vivo", "success");
    // Al volver de un corte, el estado se pide de nuevo a la base.
    await refrescarTablero();
});

connection.onclose(() => marcarConexion("sin conexión", "danger"));

connection.start()
    .then(() => marcarConexion("en vivo", "success"))
    .catch(() => marcarConexion("sin conexión", "danger"));
