using BlazingPizza;
using Microsoft.EntityFrameworkCore;

public class PizzaStoreContext : DbContext
{
    // Constructor que recibe opciones de configuración para el contexto
    public PizzaStoreContext(DbContextOptions options) : base(options)
    {
    }

    // DbSet para la tabla de órdenes
    public DbSet<Order> Orders { get; set; }

    // DbSet para la tabla de pizzas
    public DbSet<Pizza> Pizzas { get; set; }

    // DbSet para la tabla de especialidades de pizza
    public DbSet<PizzaSpecial> Specials { get; set; }

    // DbSet para la tabla de toppings
    public DbSet<Topping> Toppings { get; set; }

    // Configuración del modelo mediante Fluent API
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de una relación muchos-a-muchos entre Pizza y Topping, adecuada para la serialización
        modelBuilder.Entity<PizzaTopping>().HasKey(pst => new { pst.PizzaId, pst.ToppingId });
        modelBuilder.Entity<PizzaTopping>().HasOne<Pizza>().WithMany(ps => ps.Toppings);
        modelBuilder.Entity<PizzaTopping>().HasOne(pst => pst.Topping).WithMany();
    }
}
