using System.Data;

using MySql.Data.MySqlClient;

namespace inmobiliaria.Models
{
    public class RepositorioUsuario : RepositorioBase
    {

        public RepositorioUsuario() : base()
        {
            //https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/
        }


        public IList<Usuario> ObtenerTodos()
        {
            IList<Usuario> res = new List<Usuario>();
            MySqlConnection conn = ObtenerConexion();
            {
                string sql = @"
					SELECT Id, Nombre, Apellido, Avatar, Email, Clave, Rol
					FROM Usuario
                    WHERE estado=1";
                using (var command = new MySqlCommand(sql, conn))
                {
                    command.CommandType = CommandType.Text;

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Usuario e = new Usuario
                        {
                            Id = reader.GetInt32("Id"),
                            Nombre = reader.GetString("Nombre"),
                            Apellido = reader.GetString("Apellido"),
                            Avatar = reader.IsDBNull(3) ? null : reader.GetString(3),
                            Email = reader.GetString("Email"),
                            Clave = reader.GetString("Clave"),
                            Rol = reader.GetInt32("Rol"),
                        };
                        res.Add(e);
                    }

                }
            }
            return res;
        }


        public int Alta(Usuario e)
        {
            int res = -1;
            MySqlConnection conn = ObtenerConexion();
            {
                string sql = @"INSERT INTO Usuario 
					(Nombre, Apellido, Avatar, Email, Clave, Rol, Estado) 
					VALUES (@nombre, @apellido, @avatar, @email, @clave, @rol, 1);
					Select LAST_INSERT_ID();";//devuelve el id insertado (LAST_INSERT_ID para mysql)
                using (var command = new MySqlCommand(sql, conn))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@nombre", e.Nombre);
                    command.Parameters.AddWithValue("@apellido", e.Apellido);
                    if (String.IsNullOrEmpty(e.Avatar))
                        command.Parameters.AddWithValue("@avatar", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@avatar", e.Avatar);
                    command.Parameters.AddWithValue("@email", e.Email);
                    command.Parameters.AddWithValue("@clave", e.Clave);
                    command.Parameters.AddWithValue("@rol", e.Rol);

                    res = Convert.ToInt32(command.ExecuteScalar());
                    e.Id = res;

                }
            }
            return res;
        }

        public int Modificacion(Usuario e)
        {
            int res = -1;
            MySqlConnection conn = ObtenerConexion();
            {
                string sql = @"UPDATE Usuario 
					SET Nombre=@nombre, Apellido=@apellido, Avatar=@avatar, Email=@email, Clave=@clave, Rol=@rol
					WHERE Id = @id";
                using (var command = new MySqlCommand(sql, conn))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@nombre", e.Nombre);
                    command.Parameters.AddWithValue("@apellido", e.Apellido);
                    command.Parameters.AddWithValue("@avatar", e.Avatar);
                    command.Parameters.AddWithValue("@email", e.Email);
                    command.Parameters.AddWithValue("@clave", e.Clave);
                    command.Parameters.AddWithValue("@rol", e.Rol);
                    command.Parameters.AddWithValue("@id", e.Id);

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
				string sql = "UPDATE Usuario SET Estado= 0 WHERE Id = @id";
				 using (var command = new MySqlCommand(sql, conn))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@id", id);
					res = command.ExecuteNonQuery();
				
				}
			}
			return res;
		}
        
		public Usuario ObtenerPorId(int id)
		{
			Usuario? e = null;
			    MySqlConnection conn = ObtenerConexion();
			{
				string sql = @"SELECT 
					Id, Nombre, Apellido, Avatar, Email, Clave, Rol 
					FROM Usuario
					WHERE Id=@id";
				   using (var command = new MySqlCommand(sql, conn))
				{
					command.Parameters.AddWithValue("@id", id);
					command.CommandType = CommandType.Text;
					
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						e = new Usuario
						{
							Id = reader.GetInt32("Id"),
							Nombre = reader.GetString("Nombre"),
							Apellido = reader.GetString("Apellido"),
							Avatar = reader["Avatar"] == DBNull.Value ? "" : reader.GetString("Avatar"),
							Email = reader.GetString("Email"),
							Clave = reader.GetString("Clave"),
							Rol = reader.GetInt32("Rol"),
						};
					}
				
				}
			}
			return e;
		}

		public Usuario ObtenerPorEmail(string email)
		{
			Usuario? e = null;
				    MySqlConnection conn = ObtenerConexion();
			{
				string sql = @"SELECT
					Id, Nombre, Apellido, Avatar, Email, Clave, Rol FROM Usuario
					WHERE Email=@email";
			 using (var command = new MySqlCommand(sql, conn))
				{
					command.CommandType = CommandType.Text;
                    	command.Parameters.AddWithValue("@email",  email);
                 
			
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						e = new Usuario
						{
							Id = reader.GetInt32("Id"),
							Nombre = reader.GetString("Nombre"),
							Apellido = reader.GetString("Apellido"),
                            Avatar = reader["Avatar"] == DBNull.Value ? "" : reader.GetString("Avatar"),
                            Email = reader.GetString("Email"),
							Clave = reader.GetString("Clave"),
							Rol = reader.GetInt32("Rol"),
						};
					}
				
				}
			}
			return e;
		}
    }
}

