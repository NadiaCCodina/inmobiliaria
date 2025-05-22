
using System.Data;
using MySql.Data.MySqlClient;

namespace inmobiliaria.Models
{

	public class RepositorioPago : RepositorioBase
	{
		public RepositorioPago() : base()
		{
			//https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/
		}
		public int Alta(Pago entidad)
		{
			int res = -1;
			MySqlConnection conn = ObtenerConexion();
			{
				string sql = @"INSERT INTO pago
                (idContrato, nroCuota, monto, fechaPago, mes) 
                VALUES ( @idContrato, @nroCuota, @monto, @fechaPago, @mes);
                SELECT LAST_INSERT_ID(); ";
				using (var command = new MySqlCommand(sql, conn))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@idContrato", entidad.ContratoId);
					command.Parameters.AddWithValue("@nroCuota", entidad.NroCuota);
					command.Parameters.AddWithValue("@monto", entidad.Monto);
					command.Parameters.AddWithValue("@fechaPago", entidad.FechaPago);
					command.Parameters.AddWithValue("@mes", entidad.Mes);

					res = Convert.ToInt32(command.ExecuteScalar());
					entidad.Id = res;

				}
			}
			return res;

		}

		public int AltaAutomatico(int idContrato, int nroCuota, decimal monto, string estado)
		{
			int res = -1;
			MySqlConnection conn = ObtenerConexion();
			{
				string sql = @"INSERT INTO pago
                (idContrato, nroCuota, monto, estado) 
                VALUES ( @idContrato, @nroCuota, @monto, @estado);
                SELECT LAST_INSERT_ID(); ";
				using (var command = new MySqlCommand(sql, conn))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@idContrato", idContrato);
					command.Parameters.AddWithValue("@nroCuota", nroCuota);
					command.Parameters.AddWithValue("@monto", monto);
					command.Parameters.AddWithValue("@estado", estado);

					res = Convert.ToInt32(command.ExecuteScalar());
					idContrato = res;

				}
			}
			return res;

		}

		public int Modificacion(Pago entidad)
		{
			int res = -1;
			MySqlConnection conn = ObtenerConexion();
			{
				string sql = @$"UPDATE `pago` SET 
                fechaPago=@fechaPago, mes=@mes, estado=@estado, usuarioId_alta=@usuarioId_alta, `alta_fecha`= NOW()
                WHERE idContrato= @idContrato
                and id= @id ; ";
				using (var command = new MySqlCommand(sql, conn))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@fechaPago", entidad.FechaPago);
					command.Parameters.AddWithValue("@mes", entidad.Mes);
					command.Parameters.AddWithValue("@estado", "Pago");
					command.Parameters.AddWithValue("@id", entidad.Id);
					command.Parameters.AddWithValue("@idContrato", entidad.ContratoId);
					command.Parameters.AddWithValue("@usuarioid_alta", entidad.UsuarioAltaId);
					res = command.ExecuteNonQuery();

				}
			}
			return res;

		}
		public IList<Pago> ObtenerTodosPorId(int idContrato)
		{
			IList<Pago> res = new List<Pago>();
			MySqlConnection conn = ObtenerConexion();
			{
				string sql = @$"
					SELECT idContrato, nroCuota, monto, estado, id, mes, fechaPago
                    FROM pago
                    WHERE idContrato = @idContrato
                   ";
				using (var command = new MySqlCommand(sql, conn))
				{
					command.Parameters.AddWithValue("@idContrato", idContrato);
					command.CommandType = CommandType.Text;

					var reader = command.ExecuteReader();
					while (reader.Read())
					{
					Pago entidad = new Pago
					{
						ContratoId = reader.GetInt32(0),
						NroCuota = reader.GetInt32(1),
						Monto = reader.GetDecimal(2),
						Estado = reader.GetString(3),
						Id = reader.GetInt32(4),
						
						Mes = reader["Mes"] == DBNull.Value ? "" : reader.GetString("Mes"),
						FechaPago = reader["FechaPago"] == DBNull.Value ? new DateTime(0000-00-0) : reader.GetDateTime("FechaPago"),
                        
						};
							res.Add(entidad);
					}
				}
			}
			return res;
		}
		
	public IList<Pago> ObtenerTodosPorIdAuditoria(int idContrato)
{
    IList<Pago> res = new List<Pago>();
    MySqlConnection conn = ObtenerConexion();
    {
        string sql = @$"
            SELECT 
  p.id, 
  p.idContrato, 
  p.nroCuota, 
  p.monto, 
  p.fechaPago, 
  p.mes, 
  p.estado,  
  usuario_alta.nombre, 
  usuario_alta.apellido, 
  p.alta_fecha, 
  usuario_baja.nombre, 
  usuario_baja.apellido, 
  p.baja_fecha
FROM pago p
LEFT JOIN Usuario usuario_alta ON usuario_alta.id = p.usuarioid_alta
LEFT JOIN Usuario usuario_baja ON usuario_baja.id = p.usuarioid_baja
WHERE p.idContrato = @idContrato

        ";
        using (var command = new MySqlCommand(sql, conn))
        {
            command.Parameters.AddWithValue("@idContrato", idContrato);
            command.CommandType = CommandType.Text;

            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                Pago entidad = new Pago
				{
					Id = reader.GetInt32(reader.GetOrdinal("id")),
					ContratoId = reader.GetInt32(reader.GetOrdinal("idContrato")),
					NroCuota = reader.GetInt32(reader.GetOrdinal("nroCuota")),
					Monto = reader.GetDecimal(reader.GetOrdinal("monto")),
					FechaPago = reader.IsDBNull(reader.GetOrdinal("fechaPago"))
						? (DateTime?)null
						: reader.GetDateTime(reader.GetOrdinal("fechaPago")),
					Mes = reader.IsDBNull(reader.GetOrdinal("mes"))
						? string.Empty
						: reader.GetString(reader.GetOrdinal("mes")),
					Estado = reader.IsDBNull(reader.GetOrdinal("estado"))
						? string.Empty
						: reader.GetString(reader.GetOrdinal("estado")),
						 
				    UsuAlta = reader.IsDBNull(7) && reader.IsDBNull(8) ? null : new Usuario
                            {
                                Nombre = reader.IsDBNull(7) ? "" : reader.GetString(7),
                                Apellido = reader.IsDBNull(8) ? "" : reader.GetString(8)
                            },

                    UsuBaja = reader.IsDBNull(11) && reader.IsDBNull(12) ? null : new Usuario
                            {
                                Nombre = reader.IsDBNull(11) ? "" : reader.GetString(11),
                                Apellido = reader.IsDBNull(12) ? "" : reader.GetString(12)
                            },
                };

                res.Add(entidad);
            }
        }
    }
    return res;
}

		
		public Pago ObtenerPorId(int idContrato)
		{
			Pago entidad = null;
			MySqlConnection conn = ObtenerConexion();
			{
				string sql = @$"
					SELECT idContrato, nroCuota, monto, estado, id
                    FROM pago
                    WHERE idContrato = @idContrato
					 AND estado = 'pendiente'
                    ORDER BY nroCuota LIMIT 1
                   ";
				using (var command = new MySqlCommand(sql, conn))
				{
					command.Parameters.AddWithValue("@idContrato", idContrato);
					command.CommandType = CommandType.Text;

					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						entidad = new Pago
						{
							ContratoId = reader.GetInt32(0),
							NroCuota = reader.GetInt32(1),
							Monto = reader.GetDecimal(2),
							Estado = reader.GetString(3),
							Id = reader.GetInt32(4),

						};
					}
				}
			}
			return entidad;
		}
    }
}
