# YaVa

Tablero de pedidos en tiempo real para un bar, hecho con ASP.NET Core y SignalR.

## Contexto

Trabajo práctico de investigación de la materia Programación Web III, de la Tecnicatura en Desarrollo
Web de la Universidad Nacional de La Matanza. El tema asignado fue Real Time Communication con
SignalR, y esta aplicación es la parte práctica: demuestra los conceptos investigados funcionando con
varios usuarios conectados al mismo tiempo.

## El problema

En un bar, un pedido pasa por tres manos: alguien lo toma en la caja, alguien lo prepara en la cocina
y alguien lo entrega en el mostrador. Sin un tablero, esa coordinación se hace a los gritos o con
papelitos, y los pedidos se pierden.

Una página web común no alcanza, porque funciona por pedido y respuesta: el navegador pregunta y el
servidor contesta. Si un dato cambia, el usuario no se entera hasta que vuelve a cargar la página. La
cocina tendría que estar apretando F5 para ver si entró algo nuevo.

El nombre de la aplicación es lo que le contesta el mozo al cliente que pregunta por su pedido.

## Cómo funciona

El recorrido de un pedido es `Pendiente` → `En preparación` → `Listo` → `Entregado`.

- **Caja** carga el pedido con la mesa y los productos.
- **Cocina** lo empieza a preparar y lo marca listo.
- **Mostrador** registra la entrega.

Hoy cualquiera puede hacer los tres pasos: todavía no hay login. El tablero igual se actualiza solo
cuando alguien cambia un pedido, porque SignalR avisa a las pantallas conectadas.

Cuando esté el login, cada puesto debería ver y poder hacer solo lo suyo, y el aviso debería llegar
solo al grupo que lo necesita, no a todos.

## El tiempo real

La comunicación en tiempo real se resuelve con [SignalR](https://learn.microsoft.com/aspnet/core/signalr/introduction),
que usa WebSockets cuando el entorno lo permite y cambia solo a otro mecanismo si una red o un proxy
lo bloquean.

Las pantallas se conectan a un Hub (`Hubs/PedidosHub.cs`) y quedan escuchando. El aviso no sale del Hub
sino del código que guarda el cambio, usando `IHubContext`, y siempre **después** de que el pedido
quedó guardado:

```csharp
pedido.Estado = estado;
await _db.SaveChangesAsync();

await _avisador.AvisarAsync(pedido);
```

## Tecnologías

- ASP.NET Core 8 con el patrón MVC y vistas Razor
- SignalR para la comunicación en tiempo real
- Entity Framework Core 8 con SQL Server (LocalDB)
- Bootstrap 5 para la interfaz

## Cómo correrlo

El camino principal es **Visual Studio 2022**. LocalDB viene con esa instalación.

1. Cloná el repositorio: `https://github.com/elian-dominguez/YaVa-Pedidos.git`
2. Abrí `YaVa.sln` con doble clic, o desde Visual Studio: Archivo > Abrir > Proyecto o solución.
3. En la barra de depuración, elegí el perfil `http`.
4. F5 (con depuración) o Ctrl+F5 (sin depuración).

La aplicación queda en `http://localhost:5002`.

La primera vez que arranca se crea sola la base de datos y se carga una carta de productos.

Si no tenés Visual Studio, también se puede con el SDK de .NET 8:

```bash
dotnet run --launch-profile http
```

Para ver el tiempo real, abrí el tablero en dos ventanas, cargá un pedido en una y mirá cómo se mueve
la otra sin recargar.

## Decisiones técnicas

**La base de datos guarda el estado, SignalR solo transmite el aviso.** Un pedido primero se guarda y
recién después se notifica. Si alguien recarga la página o se le corta internet, el pedido sigue
estando, porque el aviso no es el dato.

**El precio se toma de la base al guardar, no del formulario.** El HTML se puede editar, así que el
precio que llega en el POST se ignora y se busca el del producto en la base.

**El HTML de las tarjetas vive en un solo lugar.** Cuando llega un aviso, el navegador vuelve a pedir
el tablero al servidor y reemplaza el bloque, en vez de dibujar la tarjeta con JavaScript. Así la
vista parcial de Razor (`Views/Pedidos/_Tablero.cshtml`) es la única fuente del marcado.

## Estructura

```
Hubs/PedidosHub.cs               El Hub de SignalR
Services/AvisadorPedidos.cs      A quién se le manda el aviso
Controllers/PedidosController.cs Tablero, carga de pedidos y cambios de estado
Models/                          Pedido, PedidoItem, Producto y el recorrido de estados
Data/ApplicationDbContext.cs     El contexto de Entity Framework
Data/SeedDatos.cs                Migraciones automáticas y carta inicial
Views/Pedidos/_Tablero.cshtml    Las tres columnas del tablero
wwwroot/js/tablero.js            El cliente de SignalR en el navegador
```

## Lo que falta

La cátedra pide login básico. Eso todavía no está, ni un panel para administrar la carta y los
usuarios. Cuando esté el login, el Hub tiene que agrupar las conexiones por rol y el aviso no tiene
que mandarse a todos.

## Integrantes

Grupo 3:

- Aleman, Axel Fabricio
- Cordoba, Agustina Abigail
- Dominguez, Elian
- Federico, Sabrina
- Gil, Gonzalo
- Jaimes, Lautaro
- Sandoval, Antonio Mario
