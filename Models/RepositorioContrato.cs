using System.Data;

using MySql.Data.MySqlClient;

namespace inmobiliaria.Models
{

    public class RepositorioContrato : RepositorioBase
    {

        public RepositorioContrato() : base()
        {
            //https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/
        }

        // public Inmueble controlFechas(DateTime fechaI, DateTime fechaF){
        //       IList<Inmueble> res = new List<Inmueble>();
        //       MySqlConnection conn = ObtenerConexion();
        //       {
        //           string sql= @"       SELECT i.Id, i.Direccion, i.precio
        //                           FROM inmueble i
        //                           WHERE NOT EXISTS (
        //                           SELECT 1
        //                           FROM contrato c
        //                           WHERE c.IdInmueble = i.Id
        //           AND (
        //                 (@fechaI BETWEEN FechaInicio AND FechaFin)
        //                 OR (@fechaF BETWEEN FechaInicio AND FechaFin)
        //                 OR (FechaInicio BETWEEN @fechaI AND @fechaF)
        //                 OR (FechaFin BETWEEN  @fechaI AND @fechaF)
        //             );";
        //             using (var command = new MySqlCommand(sql, conn))
        //             {
        //             command.CommandType = CommandType.Text;
        // 			command.Parameters.AddWithValue("@fechaF", fechaF);
        //             command.Parameters.AddWithValue("@fechaI", fechaI);
        //             var reader = command.ExecuteReader();
        // 			while (reader.Read())
        // 			{
        // 				Inmueble i = new Inmueble{
        //                 Id = reader.GetInt32(0),
        // 				Direccion = reader["Direccion"] == DBNull.Value ? "" : reader.GetString("Direccion"),
        //                 Precio = reader.GetInt32(2),
        //                 };
        //                 res.Add(i);
        //             }
        //       }

        // }
        //  return res;
        // }

        public int Alta(Contrato entidad, decimal monto)
        {
            int res = -1;
            MySqlConnection conn = ObtenerConexion();
            {

                string sql = @" INSERT INTO `contrato`(`idInquilino`, `idInmueble`, `monto`, `fechaInicio`, `fechaFin`, `usuarioid_alta`, `alta_fecha`)
              VALUES (@inquilinoId, @inmuebleId, @monto, @fechaInicio, @fechaFin, @usuarioid_alta, NOW());
              SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(sql, conn))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@inquilinoId", entidad.InquilinoId);
                    command.Parameters.AddWithValue("@inmuebleId", entidad.InmuebleId);
                    command.Parameters.AddWithValue("@monto", monto);
                    command.Parameters.AddWithValue("@fechaInicio", entidad.FechaInicio);
                    command.Parameters.AddWithValue("@fechaFin", entidad.FechaFin);
                    command.Parameters.AddWithValue("@usuarioid_alta", entidad.UsuarioAltaId);

                    res = Convert.ToInt32(command.ExecuteScalar());
                    entidad.Id = res;

                }
            }
            return res;


        }


        public IList<Contrato> ObtenerTodos()
        {
            IList<Contrato> res = new List<Contrato>();
            MySqlConnection conn = ObtenerConexion();
            {
                string sql = @"SELECT 
    c.id,
    inmueble.Direccion,
    c.monto,
    c.fechaInicio,
    c.fechaFin,
    inquilino.Nombre,
    inquilino.Apellido,
    inquilino.Dni,
    inmueble.precio,
    c.fechaFinalizacionEfectiva,
    CASE 
        WHEN EXISTS (
            SELECT 1 FROM Pago p 
            WHERE p.idContrato = c.Id AND p.Estado = 'Pendiente'
        )
        THEN 1 ELSE 0
    END AS TienePagosPendientes
FROM Contrato c
INNER JOIN Inquilino inquilino ON inquilino.id = c.idInquilino
INNER JOIN Inmueble inmueble ON inmueble.id = c.idInmueble;
";
                using (var command = new MySqlCommand(sql, conn))
                {
                    command.CommandType = CommandType.Text;

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Contrato entidad = new Contrato
                        {
                            Id = reader.GetInt32(0),
                            Monto = reader.GetDecimal(2),
                            FechaInicio = reader.GetDateTime(3),
                            FechaFin = reader.GetDateTime(4),
                            FechaFinEfectiva = reader.IsDBNull(9) ? (DateTime?)null : reader.GetDateTime(9),
                            Pagos = reader.GetInt32(10),

                            Inqui = new Inquilino
                            {
                                Nombre = reader["Nombre"] == DBNull.Value ? "" : reader.GetString(5),
                                Apellido = reader["Apellido"] == DBNull.Value ? "" : reader.GetString(6),
                                Dni = reader["Dni"] == DBNull.Value ? "" : reader.GetString(7)
                            },
                            Inmue = new Inmueble
                            {
                                Direccion = reader["Direccion"] == DBNull.Value ? "" : reader.GetString(1),
                                Precio = reader["Precio"] == DBNull.Value ? 0 : reader.GetInt32(8),
                            }
                        };


                        res.Add(entidad);
                    }

                }
            }
            return res;
        }

        public IList<Contrato> ObtenerTodosPorInmueble(int id)
        {
            IList<Contrato> res = new List<Contrato>();
            MySqlConnection conn = ObtenerConexion();
            {
                string sql = @"SELECT 
    c.id,
    inmueble.Direccion,
    c.monto,
    c.fechaInicio,
    c.fechaFin,
    inquilino.Nombre,
    inquilino.Apellido,
    inquilino.Dni,
    inmueble.precio,
    CASE 
        WHEN EXISTS (
            SELECT 1 FROM Pago p 
            WHERE p.idContrato = c.Id AND p.Estado = 'Pendiente'
        )
        THEN 1 ELSE 0
    END AS TienePagosPendientes
FROM Contrato c

INNER JOIN Inquilino inquilino ON inquilino.id = c.idInquilino
INNER JOIN Inmueble inmueble ON inmueble.id = c.idInmueble
WHERE c.idInmueble = @id
";
                using (var command = new MySqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.CommandType = CommandType.Text;

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Contrato entidad = new Contrato
                        {
                            Id = reader.GetInt32(0),

                            Monto = reader.GetDecimal(2),
                            FechaInicio = reader.GetDateTime(3),
                            FechaFin = reader.GetDateTime(4),
                            Pagos = reader.GetInt32(9),

                            Inqui = new Inquilino
                            {
                                Nombre = reader["Nombre"] == DBNull.Value ? "" : reader.GetString(5),
                                Apellido = reader["Apellido"] == DBNull.Value ? "" : reader.GetString(6),
                                Dni = reader["Dni"] == DBNull.Value ? "" : reader.GetString(7)
                            },
                            Inmue = new Inmueble
                            {
                                Direccion = reader["Direccion"] == DBNull.Value ? "" : reader.GetString(1),
                                Precio = reader["Precio"] == DBNull.Value ? 0 : reader.GetInt32(8),
                            }
                        };

                        res.Add(entidad);
                    }

                }
            }
            return res;
        }


        public Contrato ObtenerPorId(int id)
        {
            Contrato entidad = null;
            MySqlConnection conn = ObtenerConexion();
            {
                string sql = @$"
					SELECT c.id, inmueble.Direccion, c.monto, c.fechaInicio, c.fechaFin, inquilino.Nombre, inquilino.Apellido, inquilino.Dni, idInquilino, idInmueble, inmueble.ambientes
                       FROM Contrato c
                       INNER JOIN Inquilino inquilino ON inquilino.id = c.idInquilino
                       INNER JOIN Inmueble inmueble ON inmueble.id = c.idInmueble
                       WHERE c.id = @id";
                using (var command = new MySqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.CommandType = CommandType.Text;

                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        entidad = new Contrato
                        {
                            Id = reader.GetInt32(0),

                            Monto = reader.GetDecimal(2),
                            FechaInicio = reader.GetDateTime(3),
                            FechaFin = reader.GetDateTime(4),

                            Inqui = new Inquilino
                            {
                                Nombre = reader["Nombre"] == DBNull.Value ? "" : reader.GetString(5),
                                Apellido = reader["Apellido"] == DBNull.Value ? "" : reader.GetString(6),
                                Dni = reader["Dni"] == DBNull.Value ? "" : reader.GetString(7),
                                Id = reader.GetInt32(8)
                            },
                            Inmue = new Inmueble
                            {
                                Direccion = reader["Direccion"] == DBNull.Value ? "" : reader.GetString(1),
                                Id = reader.GetInt32(9),
                                Ambientes = reader.GetInt32(10)
                            }
                        };
                    }
                    return entidad;
                }

            }
        }

        public Contrato ObtenerPorIdAuditoria(int id)
        {
            Contrato entidad = null;
            MySqlConnection conn = ObtenerConexion();
            {
                string sql = @$"
					SELECT 
    c.id, 
    inmueble.Direccion, 
    c.monto, 
    c.fechaInicio, 
    c.fechaFin, 
    inquilino.Nombre, 
    inquilino.Apellido, 
    inquilino.Dni, 
    idInquilino, 
    idInmueble, 
    usuario_alta.nombre, 
    usuario_alta.apellido, 
    c.alta_fecha, 
    usuario_baja.nombre, 
    usuario_baja.apellido, 
    c.baja_fecha,
    inmueble.ambientes
FROM Contrato c
INNER JOIN Inquilino inquilino ON inquilino.id = c.idInquilino
INNER JOIN Inmueble inmueble ON inmueble.id = c.idInmueble
LEFT JOIN Usuario usuario_alta ON usuario_alta.id = c.usuarioid_alta
LEFT JOIN Usuario usuario_baja ON usuario_baja.id = c.usuarioid_baja
WHERE c.id = @id;
";
                using (var command = new MySqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.CommandType = CommandType.Text;

                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        entidad = new Contrato
                        {
                            Id = reader.GetInt32(0),

                            Monto = reader.GetDecimal(2),
                            FechaInicio = reader.GetDateTime(3),
                            FechaFin = reader.GetDateTime(4),
                            FechaAltaUsuario = reader.IsDBNull(12) ? (DateTime?)null : reader.GetDateTime(12),
                            FechaFinUsuario = reader.IsDBNull(15) ? (DateTime?)null : reader.GetDateTime(15),
                            Inqui = new Inquilino
                            {
                                Nombre = reader["Nombre"] == DBNull.Value ? "" : reader.GetString(5),
                                Apellido = reader["Apellido"] == DBNull.Value ? "" : reader.GetString(6),
                                Dni = reader["Dni"] == DBNull.Value ? "" : reader.GetString(7),
                                Id = reader.GetInt32(8)
                            },
                            Inmue = new Inmueble
                            {
                                Direccion = reader["Direccion"] == DBNull.Value ? "" : reader.GetString(1),
                                Id = reader.GetInt32(9),
                                Ambientes = reader.GetInt32(16)
                            },
                            UsuAlta = reader.IsDBNull(10) && reader.IsDBNull(11) ? null : new Usuario
                            {
                                Nombre = reader.IsDBNull(10) ? "" : reader.GetString(10),
                                Apellido = reader.IsDBNull(11) ? "" : reader.GetString(11)
                            },

                            UsuBaja = reader.IsDBNull(13) && reader.IsDBNull(14) ? null : new Usuario
                            {
                                Nombre = reader.IsDBNull(13) ? "" : reader.GetString(13),
                                Apellido = reader.IsDBNull(14) ? "" : reader.GetString(14)
                            },
                        };

                    }
                    return entidad;
                }

            }
        }

        public int Modificacion(Contrato entidad, decimal monto)
        {
            int res = -1;
            MySqlConnection conn = ObtenerConexion();
            {
                string sql = "UPDATE contrato SET idInquilino=@inquilinoId ,idInmueble=@inmuebleId, monto= @monto, fechaInicio= @fechaInicio, fechaFin= @fechaFin " +
                "WHERE id = @id";
                using (var command = new MySqlCommand(sql, conn))
                {

                    command.Parameters.AddWithValue("@inquilinoId", entidad.InquilinoId);
                    command.Parameters.AddWithValue("@inmuebleId", entidad.InmuebleId);
                    command.Parameters.AddWithValue("@monto", monto);
                    command.Parameters.AddWithValue("@fechaInicio", entidad.FechaInicio);
                    command.Parameters.AddWithValue("@fechaFin", entidad.FechaFin);
                    command.Parameters.AddWithValue("@id", entidad.Id);
                    command.CommandType = CommandType.Text;


                    res = command.ExecuteNonQuery();

                }
            }
            return res;
        }
        public int Baja(int id)
        {
            int res = -1;
            MySqlConnection conn = ObtenerConexion();
            {
                string sql = @$"DELETE FROM Contrato WHERE Id = @id";
                using (var command = new MySqlCommand(sql, conn))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id", id);

                    res = command.ExecuteNonQuery();

                }
            }
            return res;
        }

        public int Finalizar(Contrato entidad)
        {
            int res = -1;
            MySqlConnection conn = ObtenerConexion();
            {
                string sql = "UPDATE contrato SET `fechaFinalizacionEfectiva`= NOW(), `usuarioid_baja`= 2,`baja_fecha`= NOW()" +
                            "WHERE id = 8";
                using (var command = new MySqlCommand(sql, conn))
                {


                    // command.Parameters.AddWithValue("@fechaFin", DateTime.Now);
                    command.Parameters.AddWithValue("@id", entidad.Id);
                    command.Parameters.AddWithValue("@usuarioid_baja", entidad.UsuarioBajaId);

                    command.CommandType = CommandType.Text;


                    res = command.ExecuteNonQuery();

                }
            }
            return res;
        }


        public IList<Contrato> ObtenerPorFecha(DateTime fechaI, DateTime fechaF)
        {
            IList<Contrato> res = new List<Contrato>();
            MySqlConnection conn = ObtenerConexion();
            {

                string sql = @"
            SELECT c.Id, c.idInquilino, c.idInmueble, c.Monto, c.FechaInicio, c.FechaFin,
                   i.direccion, i.uso, i.tipo, i.precio,
                   inq.nombre AS NombreInquilino, inq.apellido AS ApellidoInquilino
            FROM contrato c
            JOIN inmueble i ON c.idInmueble = i.Id
            JOIN inquilino inq ON c.idInquilino = inq.Id
            WHERE (
                    (@fechaI BETWEEN c.FechaInicio AND c.FechaFin)
                 OR (@fechaF BETWEEN c.FechaInicio AND c.FechaFin)
                 OR (c.FechaInicio BETWEEN @fechaI AND @fechaF)
                 OR (c.FechaFin BETWEEN @fechaI AND @fechaF)
            );";

                using (var command = new MySqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@fechaI", fechaI);
                    command.Parameters.AddWithValue("@fechaF", fechaF);
                    command.CommandType = CommandType.Text;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Contrato entidad = new Contrato
                            {
                                Id = reader.GetInt32(0),
                                InquilinoId = reader.GetInt32(1),
                                InmuebleId = reader.GetInt32(2),
                                Monto = reader.GetDecimal(3),
                                FechaInicio = reader.GetDateTime(4),
                                FechaFin = reader.GetDateTime(5),

                                Inmue = new Inmueble
                                {
                                    Direccion = reader.IsDBNull(6) ? "" : reader.GetString(6),
                                    Uso = reader.IsDBNull(7) ? "" : reader.GetString(7),
                                    Tipo = reader.IsDBNull(8) ? "" : reader.GetString(8),
                                    Precio = reader.IsDBNull(9) ? 0 : reader.GetInt32(9)
                                },
                                Inqui = new Inquilino
                                {
                                    Nombre = reader.IsDBNull(10) ? "" : reader.GetString(10),
                                    Apellido = reader.IsDBNull(11) ? "" : reader.GetString(11)
                                }
                            };

                            res.Add(entidad);
                        }
                    }
                }
            }

            return res;
        }





    }


}