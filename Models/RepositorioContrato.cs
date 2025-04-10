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

        public int Alta(Contrato entidad)
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
                    command.Parameters.AddWithValue("@monto", entidad.Monto);
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
                string sql = @"SELECT c.id, inmueble.Direccion, c.monto, c.fechaInicio, c.fechaFin, inquilino.Nombre, inquilino.Apellido, inquilino.Dni 
                       FROM Contrato c
                       INNER JOIN Inquilino inquilino ON inquilino.id = c.idInquilino
                       INNER JOIN Inmueble inmueble ON inmueble.id = c.idInmueble;";
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

                            Inqui = new Inquilino
                            {
                                Nombre = reader["Nombre"] == DBNull.Value ? "" : reader.GetString(5),
                                Apellido = reader["Apellido"] == DBNull.Value ? "" : reader.GetString(6),
                                Dni = reader["Dni"] == DBNull.Value ? "" : reader.GetString(7)
                            }
                        };

                        res.Add(entidad);
                    }

                }
            }
              return res;
        }
    }
}
