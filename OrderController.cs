using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazingPizza;

// Configuración de la ruta base para el controlador y marcación como controlador de API
[Route("orders")]
[ApiController]
public class OrderController : Controller
{
    // Campo privado para el contexto de la base de datos
    private readonly PizzaStoreContext _db;

    // Constructor que recibe el contexto de la base de datos mediante inyección de dependencias
    public OrderController(PizzaStoreContext db)
    {
        _db = db;
    }

    // Método HTTP GET para obtener todas las órdenes
    [HttpGet]
    public async Task<ActionResult<List<OrderWithStatus>>> GetOrders()
    {
        // Obtener las órdenes de la base de datos, incluyendo las pizzas, sus especialidades y toppings, ordenadas por fecha de creación descendente
        var orders = await _db.Orders
            .Include(o => o.Pizzas).ThenInclude(p => p.Special)
            .Include(o => o.Pizzas).ThenInclude(p => p.Toppings).ThenInclude(t => t.Topping)
            .OrderByDescending(o => o.CreatedTime)
            .ToListAsync();

        // Convertir cada orden en una instancia de OrderWithStatus y devolver la lista
        return orders.Select(o => OrderWithStatus.FromOrder(o)).ToList();
    }

    // Método HTTP POST para colocar una nueva orden
    [HttpPost]
    public async Task<ActionResult<int>> PlaceOrder(Order order)
    {
        // Establecer el tiempo de creación de la orden al momento actual
        order.CreatedTime = DateTime.Now;

        // Asegurar que los IDs de Pizza.SpecialId y Topping.ToppingId existan en la base de datos
        // y evitar que el remitente cree nuevas especialidades y toppings
        foreach (var pizza in order.Pizzas)
        {
            pizza.SpecialId = pizza.Special.Id;
            pizza.Special = null;
        }

        // Adjuntar la orden al contexto de la base de datos y guardar los cambios
        _db.Orders.Attach(order);
        await _db.SaveChangesAsync();

        // Devolver el ID de la orden recién creada
        return order.OrderId;
    }
}
