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
				string sql = @"SELECT i.id, i.direccion, `ambientes`, `superficie`, `latitud`, `longitud`, `idPropietario`, `uso`, `tipo`, `precio`, p.nombre, p.apellido,  MAX(fechaInicio) AS fechaInicio, MAX(fechaFin) AS fechaFin, c.id
								FROM `inmueble` i 
								left join contrato c on i.id = c.idInmueble
								join propietario p on i.idPropietario = p.id
								GROUP BY 
 								i.id;";
				using (var command = new MySqlCommand(sql, conn))
				{
					command.CommandType = CommandType.Text;

					var reader = command.ExecuteReader();
					while (reader.Read())
					{

						Contrato contrato = null;

						// Validación de datos nulos para contrato
						if (!reader.IsDBNull(12) && !reader.IsDBNull(13) && !reader.IsDBNull(14))
						{
							contrato = new Contrato
							{
								Id = reader.GetInt32(14),
								FechaInicio = reader.GetDateTime(12),
								FechaFin = reader.GetDateTime(13),
							};
						}
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
								Nombre = reader.GetString(10),
								Apellido = reader.GetString(11),
								//Dni = reader.GetString(11), 
							},
							ContratoInmueble = contrato

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

		public IList<Inmueble> BuscarPorPropietario(int idPropietario)
		{
			List<Inmueble> res = new List<Inmueble>();
			Inmueble entidad = null;
			MySqlConnection conn = ObtenerConexion();
			{
				string sql = @$"
					SELECT i.Id AS InmuebleId, i.Direccion, i.Ambientes, i.Superficie, i.Latitud, i.Longitud, Tipo,
       			    i.idPropietario AS PropietarioId, p.Nombre, p.Apellido, MAX(fechaInicio) AS fechaInicio, MAX(fechaFin) AS fechaFin, c.id
					FROM Inmueble i 
					left join contrato c on i.id = c.idInmueble
					JOIN Propietario p ON i.idPropietario = p.Id
					WHERE i.idPropietario = @idPropietario
                    GROUP BY i.id;";
				using (var command = new MySqlCommand(sql, conn))
				{
					command.Parameters.AddWithValue("@idPropietario", idPropietario);
					//command.Parameters.Add("@idPropietario", SqlDbType.Int).Value = idPropietario;
					command.CommandType = CommandType.Text;

					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Contrato contrato = null;

						if (!reader.IsDBNull(10) && !reader.IsDBNull(11) && !reader.IsDBNull(12))
						{
							contrato = new Contrato
							{
								Id = reader.GetInt32(12),
								FechaInicio = reader.GetDateTime(10),
								FechaFin = reader.GetDateTime(11),
							};
						}
						entidad = new Inmueble
						{
							Id = reader.GetInt32("InmuebleId"),
							Direccion = reader["Direccion"] == DBNull.Value ? "" : reader.GetString("Direccion"),
							Ambientes = reader.GetInt32("Ambientes"),
							Superficie = reader.GetInt32("Superficie"),
							Latitud = reader.GetDecimal("Latitud"),
							Longitud = reader.GetDecimal("Longitud"),
							Tipo = reader.GetString("Tipo"),
							PropietarioId = reader.GetInt32("PropietarioId"),
							Duenio = new Propietario
							{
								Id = reader.GetInt32("PropietarioId"),
								Nombre = reader.GetString("Nombre"),
								Apellido = reader.GetString("Apellido"),
							},
							ContratoInmueble = contrato
						};
						res.Add(entidad);
					}

				}
			}
			return res;
		}

		public Inmueble controlFechaId(int id, DateTime fechaI, DateTime fechaF)
		{
			Inmueble entidad = null;
			MySqlConnection conn = ObtenerConexion();
			{
				string sql = @$" SELECT i.id, i.direccion, `ambientes`, `superficie`, `latitud`, `longitud`, `idPropietario`, `uso`, `tipo`, `precio`, p.nombre, p.apellido
            FROM inmueble i
			join propietario p on i.idPropietario = p.id
            WHERE i.id = @id
            and NOT EXISTS (
                SELECT 1
                FROM contrato c
                WHERE c.IdInmueble = i.Id
                  AND ( (@fechaI BETWEEN c.FechaInicio AND c.FechaFin)
                     OR (@fechaF BETWEEN c.FechaInicio AND c.FechaFin)
                     OR (c.FechaInicio BETWEEN @fechaI AND @fechaF)
                     OR (c.FechaFin BETWEEN @fechaI AND @fechaF)
                  )
            );";
				using (var command = new MySqlCommand(sql, conn))
				{
					command.Parameters.AddWithValue("@id", id);
					command.Parameters.AddWithValue("@fechaI", fechaI);
					command.Parameters.AddWithValue("@fechaF", fechaF);
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

		public IList<Inmueble> controlFechas(DateTime fechaI, DateTime fechaF)
		{
			IList<Inmueble> res = new List<Inmueble>();

			MySqlConnection conn = ObtenerConexion();
			{


				string sql = @"
            SELECT i.id, i.direccion, `ambientes`, `superficie`, `latitud`, `longitud`, `idPropietario`, `uso`, `tipo`, `precio`, p.nombre, p.apellido
            FROM inmueble i
			join propietario p on i.idPropietario = p.id
            WHERE NOT EXISTS (
                SELECT 1
                FROM contrato c
                WHERE c.IdInmueble = i.Id
                  AND (
                        (@fechaI BETWEEN c.FechaInicio AND c.FechaFin)
                     OR (@fechaF BETWEEN c.FechaInicio AND c.FechaFin)
                     OR (c.FechaInicio BETWEEN @fechaI AND @fechaF)
                     OR (c.FechaFin BETWEEN @fechaI AND @fechaF)
                  )
            );";

				using (var command = new MySqlCommand(sql, conn))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@fechaI", fechaI);
					command.Parameters.AddWithValue("@fechaF", fechaF);

					using (var reader = command.ExecuteReader())
					{
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
									Nombre = reader.GetString(10),
									Apellido = reader.GetString(11),
									//Dni = reader.GetString(11), 
								},


							};
							res.Add(entidad);
						}
					}
				}
				return res;
			}


		}

		public IList<Inmueble> BuscarPorAmbientes(int ambientes)
		{
			List<Inmueble> res = new List<Inmueble>();
			Inmueble entidad = null;
			MySqlConnection conn = ObtenerConexion();
			{
				string sql = @"
					SELECT i.Id AS InmuebleId, i.Direccion, i.Ambientes  AS Ambientes, i.Superficie, i.Latitud, i.Longitud, Tipo,
       			    i.idPropietario AS PropietarioId, p.Nombre, p.Apellido, fechaInicio, fechaFin, c.id
					FROM Inmueble i 
					left join contrato c on i.id = c.idInmueble
					JOIN Propietario p ON i.idPropietario = p.Id
					WHERE i.Ambientes = @ambientes
					GROUP BY 
 								i.id
								ORDER BY fechaInicio;";
				using (var command = new MySqlCommand(sql, conn))
				{
					command.Parameters.AddWithValue("@ambientes", ambientes);
					//command.Parameters.Add("@idPropietario", SqlDbType.Int).Value = idPropietario;
					command.CommandType = CommandType.Text;

					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Contrato contrato = null;

						if (!reader.IsDBNull(10) && !reader.IsDBNull(11) && !reader.IsDBNull(12))
						{
							contrato = new Contrato
							{
								Id = reader.GetInt32(12),
								FechaInicio = reader.GetDateTime(10),
								FechaFin = reader.GetDateTime(11),
							};
						}
						entidad = new Inmueble
						{
							Id = reader.GetInt32("InmuebleId"),
							Direccion = reader["Direccion"] == DBNull.Value ? "" : reader.GetString("Direccion"),
							Ambientes = reader.GetInt32("Ambientes"),
							Superficie = reader.GetInt32("Superficie"),
							Latitud = reader.GetDecimal("Latitud"),
							Longitud = reader.GetDecimal("Longitud"),
							Tipo = reader.GetString("Tipo"),
							PropietarioId = reader.GetInt32("PropietarioId"),
							Duenio = new Propietario
							{
								Id = reader.GetInt32("PropietarioId"),
								Nombre = reader.GetString("Nombre"),
								Apellido = reader.GetString("Apellido"),
							},
							ContratoInmueble = contrato
						};
						res.Add(entidad);
					}

				}
			}
			return res;
		}


		public IList<Inmueble> BuscarPorTipo(String tipo)
		{
			List<Inmueble> res = new List<Inmueble>();
			Inmueble entidad = null;
			MySqlConnection conn = ObtenerConexion();
			{
				string sql = @"
					SELECT i.Id AS InmuebleId, i.Direccion, i.Ambientes  AS Ambientes, i.Superficie, i.Latitud, i.Longitud, Tipo,
       			    i.idPropietario AS PropietarioId, p.Nombre, p.Apellido, fechaInicio, fechaFin, c.id
					FROM Inmueble i 
					left join contrato c on i.id = c.idInmueble
					JOIN Propietario p ON i.idPropietario = p.Id
					WHERE i.Tipo = @tipo
					GROUP BY 
 								i.id
								ORDER BY fechaInicio;";
				using (var command = new MySqlCommand(sql, conn))
				{
					command.Parameters.AddWithValue("@tipo", tipo);
					//command.Parameters.Add("@idPropietario", SqlDbType.Int).Value = idPropietario;
					command.CommandType = CommandType.Text;

					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Contrato contrato = null;

						if (!reader.IsDBNull(10) && !reader.IsDBNull(11) && !reader.IsDBNull(12))
						{
							contrato = new Contrato
							{
								Id = reader.GetInt32(12),
								FechaInicio = reader.GetDateTime(10),
								FechaFin = reader.GetDateTime(11),
							};
						}
						entidad = new Inmueble
						{
							Id = reader.GetInt32("InmuebleId"),
							Direccion = reader["Direccion"] == DBNull.Value ? "" : reader.GetString("Direccion"),
							Ambientes = reader.GetInt32("Ambientes"),
							Superficie = reader.GetInt32("Superficie"),
							Latitud = reader.GetDecimal("Latitud"),
							Longitud = reader.GetDecimal("Longitud"),
							Tipo = reader.GetString("Tipo"),
							PropietarioId = reader.GetInt32("PropietarioId"),
							Duenio = new Propietario
							{
								Id = reader.GetInt32("PropietarioId"),
								Nombre = reader.GetString("Nombre"),
								Apellido = reader.GetString("Apellido"),
							},
							ContratoInmueble = contrato
						};
						res.Add(entidad);
					}

				}
			}
			return res;
		}


		public IList<Inmueble> BuscarPor(String tipo, int? ambientes, int? idPropietario)
		{
			List<Inmueble> res = new List<Inmueble>();
			Inmueble entidad = null;
			MySqlConnection conn = ObtenerConexion();
			{
				string sql = @"SELECT 
    i.Id AS InmuebleId, 
    i.Direccion, 
    i.Ambientes, 
    i.Superficie, 
    i.Latitud, 
    i.Longitud, 
    i.Tipo,
    i.idPropietario AS PropietarioId, 
    p.Nombre, 
    p.Apellido, 
    MAX(c.fechaInicio) AS fechaInicio, 
    MAX(c.fechaFin) AS fechaFin, 
    c.id AS ContratoId,
	i.Uso,
	i.Precio
FROM 
    Inmueble i
LEFT JOIN 
    Contrato c ON i.id = c.idInmueble
JOIN 
    Propietario p ON i.idPropietario = p.Id
WHERE 
    (@tipo IS NULL OR i.Tipo = @tipo) AND
    (@ambientes IS NULL OR i.Ambientes = @ambientes) AND
    (@idPropietario IS NULL OR i.idPropietario = @idPropietario)
GROUP BY 
    i.Id, i.Direccion, i.Ambientes, i.Superficie, i.Latitud, i.Longitud, i.Tipo, 
    i.idPropietario, p.Nombre, p.Apellido
ORDER BY 
    MAX(c.fechaInicio);
";
				using (var command = new MySqlCommand(sql, conn))
				{
					command.Parameters.AddWithValue("@ambientes", ambientes);
					command.Parameters.AddWithValue("@tipo", tipo);
					command.Parameters.AddWithValue("@idPropietario", idPropietario);

					command.CommandType = CommandType.Text;

					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Contrato contrato = null;

						if (!reader.IsDBNull(reader.GetOrdinal("fechaInicio")) &&
    !reader.IsDBNull(reader.GetOrdinal("fechaFin")) &&
    !reader.IsDBNull(reader.GetOrdinal("ContratoId")))
{
	contrato = new Contrato
	{
		Id = reader.GetInt32(reader.GetOrdinal("ContratoId")),
		FechaInicio = reader.GetDateTime(reader.GetOrdinal("fechaInicio")),
		FechaFin = reader.GetDateTime(reader.GetOrdinal("fechaFin")),
	};
}
						entidad = new Inmueble
						{
							Id = reader.GetInt32("InmuebleId"),
							Direccion = reader["Direccion"] == DBNull.Value ? "" : reader.GetString("Direccion"),
							Ambientes = reader.GetInt32("Ambientes"),
							Superficie = reader.GetInt32("Superficie"),
							Latitud = reader.GetDecimal("Latitud"),
							Longitud = reader.GetDecimal("Longitud"),
							Uso = reader.GetString("Uso"),
							Tipo = reader.GetString("Tipo"),
							Precio = reader.GetInt32("Precio"),
							PropietarioId = reader.GetInt32("PropietarioId"),
							Duenio = new Propietario
							{
								Id = reader.GetInt32("PropietarioId"),
								Nombre = reader.GetString("Nombre"),
								Apellido = reader.GetString("Apellido"),
							},
							ContratoInmueble = contrato
						};
						res.Add(entidad);
					}

				}
			}
			return res;
		}


	}
}