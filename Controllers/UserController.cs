using Microsoft.AspNetCore.Mvc;
using BackendMusic.Domain;
using System.Data.SqlClient;
using System.Data;
using System;
using System.Collections.Generic;

namespace BackendMusic.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        [Route("Login")]
        public User Login(string uName, string password) //zadefinovanie typu funkcie a dát, ktoré má očakávať od frontendu
        {
            var conn = new MySql.Data.MySqlClient.MySqlConnection("server = 169.254.59.75; database = *******; uid = *******; pwd = *******; SslMode=None");
            //connection string - prihlasovacie údaje do databázy

            var cmd = new MySql.Data.MySqlClient.MySqlCommand("select COUNT(userMail) from Users where userMail = @p_name and Heslo = @p_password");
            //SQL príkaz na zistenie či užívateľ zadal správne údaje 
            cmd.Parameters.AddWithValue("p_name", uName);
            cmd.Parameters.AddWithValue("p_password", password);
            //priradenie poslaných hodnôt k dynamickým premenným posielaných do databázy
            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;
            conn.Open();
            //otvorenie spojenia
            MySql.Data.MySqlClient.MySqlDataReader reader = cmd.ExecuteReader();
            //zadefinovanie čítača dát, ktoré prišli z databázy
            int temp = 0;
            while (reader.Read())
            //podmienený cyklus, ktorý bude vykonávaný pokým budú dáta čítané
            {
                temp = Convert.ToInt32(reader[0]);
                //priradenie prečítanej hodnoty
            }
            conn.Close();
            //zatvorenie spojenia

            if (temp == 1 )
            //podmienka pre skontrolovanie či boli zadané správne prihlasovacie údaje
            {
                return new User
                {
                    isOk = true,
                    uId = 1
                    //dáta odoslané na frontend, ak boli zadané správne údaje
                };
            }

            return new User() { isOk = false };
            //dáta odoslané na frontend, ak boli zadané nesprávne údaje
        }

        [HttpGet]
        [Route("Register")]
        public User Register(string mail, string name, string password)
        {
            var conn = new MySql.Data.MySqlClient.MySqlConnection("server = 169.254.59.75; database = *******; uid = *******; pwd = *******; SslMode=None");

            var cmd = new MySql.Data.MySqlClient.MySqlCommand("select COUNT(UserMail) from Users where UserMail = @p_mail");
            cmd.Parameters.AddWithValue("p_mail", mail);
            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;
            conn.Open();
            MySql.Data.MySqlClient.MySqlDataReader reader = cmd.ExecuteReader();
            int temp = 0;
            while (reader.Read())
            {
                temp = Convert.ToInt32(reader[0]);
                //return "read";
            }

            if (temp == 0)
            {
                cmd.Parameters.Clear();
                reader.Close();
                cmd.CommandText = "insert into Users (userMail, meno, heslo) values (@p_email, @p_meno, @p_heslo)";
                cmd.Parameters.AddWithValue("p_email", mail);
                cmd.Parameters.AddWithValue("p_meno", name);
                cmd.Parameters.AddWithValue("p_heslo", password);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                conn.Close();

                return new User
                {
                    isOk = true
                    
                };
            }

            conn.Close();
            return new User() { isOk = false };
        }
    }
}
