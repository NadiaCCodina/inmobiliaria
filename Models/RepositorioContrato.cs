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

                string sql = @" INSERT INTO `contrato`(`idInquilino`, `idInmueble`, `monto`, `fechaInicio`, `fechaFin`)
              VALUES (@inquilinoId, @inmuebleId, @monto, @fechaInicio, @fechaFin);
              SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(sql, conn))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@inquilinoId", entidad.InquilinoId);
                    command.Parameters.AddWithValue("@inmuebleId", entidad.InmuebleId);
                    command.Parameters.AddWithValue("@monto", monto);
                    command.Parameters.AddWithValue("@fechaInicio", entidad.FechaInicio);
                    command.Parameters.AddWithValue("@fechaFin", entidad.FechaFin);
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
                            Pagos= reader.GetInt32(9),

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
					SELECT c.id, inmueble.Direccion, c.monto, c.fechaInicio, c.fechaFin, inquilino.Nombre, inquilino.Apellido, inquilino.Dni 
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
                                Dni = reader["Dni"] == DBNull.Value ? "" : reader.GetString(7)
                            },
                            Inmue = new Inmueble
                            {
                                Direccion = reader["Direccion"] == DBNull.Value ? "" : reader.GetString(1),
                            }
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
                string sql = "UPDATE contrato SET fechaFin= @fechaFin " +
                "WHERE id = @id";
                using (var command = new MySqlCommand(sql, conn))
                {


                    command.Parameters.AddWithValue("@fechaFin", DateTime.Now);
                    command.Parameters.AddWithValue("@id", entidad.Id);
                    command.CommandType = CommandType.Text;


                    res = command.ExecuteNonQuery();

                }
            }
            return res;
        }

    }


}