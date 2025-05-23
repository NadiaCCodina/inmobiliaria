

using System.Data;
using MySql.Data.MySqlClient;

namespace inmobiliaria.Models
{

	public class RepositorioPropietario : RepositorioBase
	{

		public RepositorioPropietario() : base()
		{
			//https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/
		}
		public int Alta(Propietario p)
		{
			int res = -1;
			MySqlConnection conn = ObtenerConexion();
			{
				string sql = @"INSERT INTO Propietario 
					(Nombre, Apellido, Dni, Telefono, direccion, Email) 
					VALUES (@nombre, @apellido, @dni, @telefono, @direccion, @email);
					SELECT LAST_INSERT_ID();";//devuelve el id insertado (SCOPE_IDENTITY para sql)
				using (var command = new MySqlCommand(sql, conn))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@nombre", p.Nombre);
					command.Parameters.AddWithValue("@apellido", p.Apellido);
					command.Parameters.AddWithValue("@dni", p.Dni);
					command.Parameters.AddWithValue("@telefono", p.Telefono);
					command.Parameters.AddWithValue("@direccion", p.Direccion);
					command.Parameters.AddWithValue("@email", p.Email);

					res = Convert.ToInt32(command.ExecuteScalar());
					p.Id = res;
				
				}
			}
			return res;
		}
		public int Baja(int id)
		{
			int res = -1;
			MySqlConnection conn = ObtenerConexion();
			{
				string sql = @$"UPDATE Propietario
					SET estado=0 WHERE {nameof(Propietario.Id)} = @id";
				using (var command = new MySqlCommand(sql, conn))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@id", id);
					res = command.ExecuteNonQuery();

				}
			}
			return res;
		}

		public IList<Propietario> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
		{
			IList<Propietario> res = new List<Propietario>();
			MySqlConnection conn = ObtenerConexion();
			{
				string sql = @$"
					SELECT Id, Direccion, Nombre, Apellido, Dni, Telefono, Email
					FROM Propietario
					WHERE estado = 1
					LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina}
				";
				using (var command = new MySqlCommand(sql, conn))
				{
					command.CommandType = CommandType.Text;

					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Propietario p = new Propietario
						{
							Id = reader.GetInt32(nameof(Propietario.Id)),
							Direccion = reader.GetString("Direccion"),
							Nombre = reader.GetString("Nombre"),
							Apellido = reader.GetString("Apellido"),
							Dni = reader.GetString("Dni"),
							Telefono = reader.GetString("Telefono"),
							Email = reader.GetString("Email"),

						};
						res.Add(p);
					}

				}
			}
			return res;
		}

		public Propietario ObtenerPorId(int id)
		{
			Propietario? p = null;
			MySqlConnection conn = ObtenerConexion();
			{
				string sql = @"SELECT 
					Id, Nombre, Apellido, Dni, Telefono, Email
					FROM Propietario
					WHERE Id=@id";
				using (var command = new MySqlCommand(sql, conn))
				{
					command.Parameters.AddWithValue("@id", id);
					command.CommandType = CommandType.Text;

					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						p = new Propietario
						{
							Id = reader.GetInt32(nameof(Propietario.Id)),
							Nombre = reader.GetString("Nombre"),
							Apellido = reader.GetString("Apellido"),
							Dni = reader.GetString("Dni"),
							Telefono = reader.GetString("Telefono"),
							Email = reader.GetString("Email"),

						};
					}

				}
			}
			return p;
		}
       //MODIFICAR
		public int Modificacion(Propietario p)
		{
			int res = -1;
				MySqlConnection conn = ObtenerConexion();
			{
				string sql = @$"UPDATE Propietario
					SET Nombre=@nombre, Apellido=@apellido, Dni=@dni, Telefono=@telefono, Email=@email
					WHERE {nameof(Propietario.Id)} = @id";
				using (var command = new MySqlCommand(sql, conn))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@nombre", p.Nombre);
					command.Parameters.AddWithValue("@apellido", p.Apellido);
					command.Parameters.AddWithValue("@dni", p.Dni);
					command.Parameters.AddWithValue("@telefono", p.Telefono);
					command.Parameters.AddWithValue("@email", p.Email);
					command.Parameters.AddWithValue("@id", p.Id);
					res = command.ExecuteNonQuery();
					
				}
			}
			return res;
		}

		public IList<Propietario> BuscarPorNombre(string nombre)
		{
			MySqlConnection conn = ObtenerConexion();
			List<Propietario> res = new List<Propietario>();
			Propietario? p = null;
			nombre = "%" + nombre + "%";

			{
				string sql = @"SELECT
					Id, Nombre, Apellido, Dni, Telefono, Email 
					FROM Propietario
					WHERE Nombre LIKE @nombre OR Apellido LIKE @nombre";
				using (var command = new MySqlCommand(sql, conn))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@nombre", nombre);

					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						p = new Propietario
						{
							Id = reader.GetInt32(nameof(Propietario.Id)),
							Nombre = reader.GetString("Nombre"),
							Apellido = reader.GetString("Apellido"),
							Dni = reader.GetString("Dni"),
							Telefono = reader.GetString("Telefono"),
							Email = reader.GetString("Email"),

						};
						res.Add(p);
					}

				}
			}
			return res;
		}

	}
}