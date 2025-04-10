using System.Data;
using MySql.Data.MySqlClient;

namespace inmobiliaria.Models
{
	public class RepositorioInmueble : RepositorioBase
	{

		public RepositorioInmueble() : base()
		{
			//https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/
		}
		public int Alta(Inmueble entidad)
		{
			int res = -1;
			MySqlConnection conn = ObtenerConexion();
			{
				string sql = @"INSERT INTO Inmueble 
					(Direccion, Ambientes, Superficie, Latitud, Longitud, idPropietario, Uso, Tipo, Precio)
					VALUES (@direccion, @ambientes, @superficie, @latitud, @longitud, @propietarioId, @uso, @tipo, @precio);
					SELECT LAST_INSERT_ID();";//devuelve el id insertado (SCOPE_IDENTITY para sql)

				using (var command = new MySqlCommand(sql, conn))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@direccion", entidad.Direccion == null ? DBNull.Value : entidad.Direccion);
					command.Parameters.AddWithValue("@ambientes", entidad.Ambientes);
					command.Parameters.AddWithValue("@superficie", entidad.Superficie);
					command.Parameters.AddWithValue("@latitud", entidad.Latitud);
					command.Parameters.AddWithValue("@longitud", entidad.Longitud);
					command.Parameters.AddWithValue("@propietarioId", entidad.PropietarioId);
					command.Parameters.AddWithValue("@uso", entidad.Uso);
					command.Parameters.AddWithValue("@tipo", entidad.Tipo);
					command.Parameters.AddWithValue("@precio", entidad.Precio);
					res = Convert.ToInt32(command.ExecuteScalar());
					entidad.Id = res;

				}
			}
			return res;
		}

		public IList<Inmueble> ObtenerTodos()
		{
			IList<Inmueble> res = new List<Inmueble>();
			MySqlConnection conn = ObtenerConexion();
			{
				string sql = @"SELECT i.id, i.Direccion, Ambientes, Superficie, Latitud, Longitud, idPropietario, uso, tipo, precio, p.Nombre, p.Apellido, p.Dni 
                       FROM Inmueble i 
                       INNER JOIN Propietario p ON i.idPropietario = p.id";
				using (var command = new MySqlCommand(sql, conn))
				{
					command.CommandType = CommandType.Text;

					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Inmueble entidad = new Inmueble
						{
							Id = reader.GetInt32(0),
							Direccion = reader["Direccion"] == DBNull.Value ? "" : reader.GetString("Direccion"),
							Ambientes = reader.GetInt32(2),
							Superficie = reader.GetInt32(3),
							Latitud = reader.GetDecimal(4),
							Longitud = reader.GetDecimal(5),
							PropietarioId = reader.GetInt32(6),
							Uso = reader["uso"] == DBNull.Value ? "" : reader.GetString("uso"),
							Tipo = reader["tipo"] == DBNull.Value ? "" : reader.GetString("tipo"),
							Precio = reader.GetInt32(9),
							Duenio = new Propietario
							{
								Id = reader.GetInt32(6),
								Nombre = reader.GetString(11),
								Apellido = reader.GetString(12),
								//Dni = reader.GetString(11), 
							}
						};
						res.Add(entidad);
					}
				}
			}
			return res;
		}

		public int Modificacion(Inmueble entidad)
		{
			int res = -1;
			MySqlConnection conn = ObtenerConexion();
			{
				string sql = "UPDATE Inmueble SET " +
	"Direccion=@direccion, Ambientes=@ambientes, Superficie=@superficie, Latitud=@latitud, Longitud=@longitud, Uso=@uso, Tipo=@tipo, Precio=@precio,  idPropietario=@propietarioId " +
	"WHERE Id = @id";
				using (var command = new MySqlCommand(sql, conn))
				{
					command.Parameters.AddWithValue("@direccion", entidad.Direccion);
					command.Parameters.AddWithValue("@ambientes", entidad.Ambientes);
					command.Parameters.AddWithValue("@superficie", entidad.Superficie);
					command.Parameters.AddWithValue("@latitud", entidad.Latitud);
					command.Parameters.AddWithValue("@longitud", entidad.Longitud);
					command.Parameters.AddWithValue("@uso", entidad.Uso);
					command.Parameters.AddWithValue("@tipo", entidad.Tipo);
					command.Parameters.AddWithValue("@precio", entidad.Precio);
					command.Parameters.AddWithValue("@propietarioId", entidad.PropietarioId);
					command.Parameters.AddWithValue("@id", entidad.Id);
					command.CommandType = CommandType.Text;

					res = command.ExecuteNonQuery();

				}
			}
			return res;
		}

		public Inmueble ObtenerPorId(int id)
		{
			Inmueble entidad = null;
			MySqlConnection conn = ObtenerConexion();
			{
				string sql = @$"
					SELECT i.{nameof(Inmueble.Id)}, i.Direccion, Ambientes, Superficie, Latitud, Longitud, Uso, Tipo, Precio, idPropietario, p.Nombre, p.Apellido
					FROM Inmueble i JOIN Propietario p ON i.idPropietario = p.Id
					WHERE i.{nameof(Inmueble.Id)}=@id";
				using (var command = new MySqlCommand(sql, conn))
				{
					command.Parameters.AddWithValue("@id", id);
					command.CommandType = CommandType.Text;

					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						entidad = new Inmueble
						{
							Id = reader.GetInt32(nameof(Inmueble.Id)),
							Direccion = reader["Direccion"] == DBNull.Value ? "" : reader.GetString("Direccion"),
							Ambientes = reader.GetInt32("Ambientes"),
							Superficie = reader.GetInt32("Superficie"),
							Latitud = reader.GetDecimal("Latitud"),
							Longitud = reader.GetDecimal("Longitud"),
							Uso = reader.GetString("Uso"),
							Tipo = reader.GetString("Tipo"),
							Precio = reader.GetInt32("Precio"),
							PropietarioId = reader.GetInt32("idPropietario"),
							Duenio = new Propietario
							{
								Id = reader.GetInt32("idPropietario"),
								Nombre = reader.GetString("Nombre"),
								Apellido = reader.GetString("Apellido"),
							}
						};
					}

				}
			}
			return entidad;
		}

		public int Baja(int id)
		{
			int res = -1;
			MySqlConnection conn = ObtenerConexion();
			{
				string sql = @$"DELETE FROM Inmueble WHERE Id = @id";
				using (var command = new MySqlCommand(sql, conn))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@id", id);

					res = command.ExecuteNonQuery();

				}
			}
			return res;
		}

	}
}