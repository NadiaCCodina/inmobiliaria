

using System.Data;
using MySql.Data.MySqlClient;

namespace inmobiliaria.Models
{

public class RepositorioInquilino :RepositorioBase
{

		public RepositorioInquilino() : base()
		{
			//https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/
		}
    public int Alta(Inquilino i)
    {
     int res = -1;
	MySqlConnection conn = ObtenerConexion();
			{
				string sql = @"INSERT INTO Inquilino 
					(Nombre, Apellido, Dni, Telefono, Email) 
					VALUES (@nombre, @apellido, @dni, @telefono, @email);
					SELECT LAST_INSERT_ID();";//devuelve el id insertado (SCOPE_IDENTITY para sql)
				using (var command = new MySqlCommand(sql, conn))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@nombre", i.Nombre);
					command.Parameters.AddWithValue("@apellido", i.Apellido);
					command.Parameters.AddWithValue("@dni", i.Dni);
					command.Parameters.AddWithValue("@telefono", i.Telefono);		
					command.Parameters.AddWithValue("@email", i.Email);
				
					res = Convert.ToInt32(command.ExecuteScalar());
					i.Id = res;
				
				}
			}
			return res;
		}
		public int Baja(int id)
		{
			int res = -1;
				MySqlConnection conn = ObtenerConexion();
			{
				string sql = @$"DELETE FROM Inquilino WHERE {nameof(Inquilino.Id)} = @id";
				using (var command = new MySqlCommand(sql, conn))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@id", id);
					res = command.ExecuteNonQuery();
				
				}
			}
			return res;
		}


	public int Modificacion(Inquilino i)
		{
			int res = -1;
				MySqlConnection conn = ObtenerConexion();
			{
				string sql = @$"UPDATE Inquilino
					SET Nombre=@nombre, Apellido=@apellido, Dni=@dni, Telefono=@telefono, Email=@email
					WHERE {nameof(Inquilino.Id)} = @id";
				using (var command = new MySqlCommand(sql, conn))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@nombre", i.Nombre);
					command.Parameters.AddWithValue("@apellido", i.Apellido);
					command.Parameters.AddWithValue("@dni", i.Dni);
					command.Parameters.AddWithValue("@telefono", i.Telefono);
					command.Parameters.AddWithValue("@email", i.Email);
					command.Parameters.AddWithValue("@id", i.Id);
					res = command.ExecuteNonQuery();
					
				}
			}
			return res;
		}
		public IList<Inquilino> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
		{
			IList<Inquilino> res = new List<Inquilino>();
			MySqlConnection conn = ObtenerConexion();
			{
				string sql = @$"
					SELECT Id, Nombre, Apellido, Dni, Telefono, Email
					FROM Inquilino
					LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina}
				";
				using (var command = new MySqlCommand(sql, conn))
				{
					command.CommandType = CommandType.Text;
					
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Inquilino i = new Inquilino
						{
							Id = reader.GetInt32(nameof(Inquilino.Id)),
							Nombre = reader.GetString("Nombre"),
							Apellido = reader.GetString("Apellido"),
							Dni = reader.GetString("Dni"),
							Telefono = reader.GetString("Telefono"),
							Email = reader.GetString("Email"),
							
						};
						res.Add(i);
					}
					
				}
			}
			return res;
		}

			 public Inquilino ObtenerPorId(int id)
		{
			Inquilino? i = null;
		MySqlConnection conn = ObtenerConexion();
			{
				string sql = @"SELECT 
					Id, Nombre, Apellido, Dni, Telefono, Email
					FROM Inquilino
					WHERE Id=@id";
				using (var command = new MySqlCommand(sql, conn))
				{
					command.Parameters.AddWithValue("@id", id);
					command.CommandType = CommandType.Text;
				
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						i = new Inquilino
						{
							Id = reader.GetInt32(nameof(Inquilino.Id)),
							Nombre = reader.GetString("Nombre"),
							Apellido = reader.GetString("Apellido"),
							Dni = reader.GetString("Dni"),
							Telefono = reader.GetString("Telefono"),
							Email = reader.GetString("Email"),
						
						};
					}
					
				}
			}
			return i;
		}

		public IList<Inquilino> BuscarPorNombre(string nombre)
		{
			MySqlConnection conn = ObtenerConexion();
			List<Inquilino> res = new List<Inquilino>();
			Inquilino? i = null;
			nombre = "%" + nombre + "%";
		
			{
				string sql = @"SELECT
					Id, Nombre, Apellido, Dni, Telefono, Email 
					FROM Inquilino
					WHERE Nombre LIKE @nombre OR Apellido LIKE @nombre";
				using (var command = new MySqlCommand(sql, conn))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@nombre", nombre);
					
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						i = new Inquilino
						{
							Id = reader.GetInt32(nameof(Inquilino.Id)),
							Nombre = reader.GetString("Nombre"),
							Apellido = reader.GetString("Apellido"),
							Dni = reader.GetString("Dni"),
							Telefono = reader.GetString("Telefono"),
							Email = reader.GetString("Email"),
							
						};
						res.Add(i);
					}
				
				}
			}
			return res;
		}
		
}
}