using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace ListadoProductosApp
{
    class Program
    {
        // Cadena de conexión a la base de datos SQLite
        static string connectionString = @"Data Source=productos.db;Version=3;";

        static void Main(string[] args)
        {
            // Crear la tabla si no existe
            CrearTabla();

            var productos = new List<Producto>();
            var salir = false;
            while (!salir)
            {
                MostrarMenu();
                try
                {
                    salir = EjecutarOperacion(productos);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Ocurrió un error: " + e.Message);
                }
                Console.WriteLine();
            }
        }

        private static void MostrarMenu()
        {
            Console.WriteLine(@"
                ****Listados Productos App****
                1. Agregar producto
                2. Listar productos
                3. Actualizar producto
                4. Eliminar producto
                5. Salir
                ");
            Console.Write("Proporciona la opción: ");
        }

        private static bool EjecutarOperacion(List<Producto> productos)
        {
            var opcion = int.Parse(Console.ReadLine());
            var salir = false;
            switch (opcion)
            {
                case 1:
                    AgregarProducto(productos);
                    break;
                case 2:
                    ListarProducto(productos);
                    break;
                case 3:
                    ActualizarProducto(productos);
                    break;
                case 4:
                    EliminarProducto(productos);
                    break;
                case 5:
                    Console.WriteLine("Hasta pronto...");
                    salir = true;
                    break;
                default:
                    Console.WriteLine("Opción incorrecta: " + opcion);
                    break;
            }
            return salir;
        }

        private static void AgregarProducto(List<Producto> productos)
        {
            Console.Write("Proporciona el nombre: ");
            var nombre = Console.ReadLine();
            Console.Write("Proporciona el marca: ");
            var marca = Console.ReadLine();
            Console.Write("Proporciona el distribuidor: ");
            var distribuidor = Console.ReadLine();
            var producto = new Producto(nombre, marca, distribuidor);

            // Agregar producto a la lista
            productos.Add(producto);

            // Insertar producto en la base de datos
            InsertarProducto(producto);

            Console.WriteLine("Producto agregado correctamente.");
        }

        private static void ListarProducto(List<Producto> productos)
        {
            if (productos.Count == 0)
            {
                Console.WriteLine("No hay productos registrados.");
            }
            else
            {
                Console.WriteLine("Listado de productos:");
                foreach (var producto in productos)
                {
                    Console.WriteLine(producto);
                }
            }
        }

        private static void ActualizarProducto(List<Producto> productos)
        {
            Console.Write("Proporciona el índice de la producto a actualizar: ");
            if (int.TryParse(Console.ReadLine(), out var indice) && indice >= 0 && indice < productos.Count)
            {
                var producto = productos[indice];
                Console.WriteLine("Productos a actualizar: " + producto);
                Console.Write("Nuevo nombre: ");
                producto.Nombre = Console.ReadLine();
                Console.Write("Nueva marca: ");
                producto.Marca = Console.ReadLine();
                Console.Write("Nuevo distribuidor: ");
                producto.Distribuidor = Console.ReadLine();
                Console.WriteLine("Producto actualizada correctamente.");
            }
            else
            {
                Console.WriteLine("Índice no válido.");
            }
        }

        private static void EliminarProducto(List<Producto> productos)
        {
            Console.Write("Proporciona el índice de la producto a eliminar: ");
            if (int.TryParse(Console.ReadLine(), out var indice) && indice >= 0 && indice < productos.Count)
            {
                var producto = productos[indice];
                productos.RemoveAt(indice);
                Console.WriteLine("Producto eliminada: " + producto);
            }
            else
            {
                Console.WriteLine("Índice no válido.");
            }
        }

        private static void CrearTabla()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                var sql = @"
                    CREATE TABLE IF NOT EXISTS Productos (
                        Nombre TEXT,
                        Marca TEXT,
                        Distribuidor TEXT
                    );
                ";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void InsertarProducto(Producto producto)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                var sql = @"
                    INSERT INTO Productos (Nombre, Marca, Distribuidor) 
                    VALUES (@Nombre, @Marca, @Distribuidor);
                ";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", producto.Nombre);
                    command.Parameters.AddWithValue("@Marca", producto.Marca);
                    command.Parameters.AddWithValue("@Distribuidor", producto.Distribuidor);
                    command.ExecuteNonQuery();
                }
            }
        }
    }

    class Producto
    {
        public string Nombre { get; set; }
        public string Marca { get; set; }
        public string Distribuidor { get; set; }

        public Producto(string nombre, string marca, string distribuidor)
        {
            Nombre = nombre;
            Marca = marca;
            Distribuidor = distribuidor;
        }

        public override string ToString()
        {
            return $"Producto{{nombre='{Nombre}', marca='{Marca}', distribuidor='{Distribuidor}'}}";
        }
    }
}