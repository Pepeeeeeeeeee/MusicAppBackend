using Microsoft.AspNetCore.Mvc;
using BackendMusic.Domain;
using System.Collections.Generic;
using VideoLibrary;
using MediaToolkit.Model;
using MediaToolkit;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using System.Data;
using Microsoft.AspNetCore.Cors;

namespace BackendMusic.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayerController : ControllerBase
    {
        [HttpGet]
        [Route("PlayList")]
        public PlayList PlayList(int idPL) //zadefinovanie typu funkcie a dát ktoré má očakávať od frontendu
        {
            var conn = new MySql.Data.MySqlClient.MySqlConnection("server = 169.254.59.75; database = *******; uid = *******; pwd = *******; SslMode=None");
            //connection string - prihlasovacie údaje do databázy

            var cmd = new MySql.Data.MySqlClient.MySqlCommand(@$"select pls.id, pls.name, lp.id, lp.name, lp.url, lp.path from PlayLists pls
                                                             join PlayList pl on pls.id = pl.id_playlist 
                                                             join LastPlayed lp on pl.Url = lp.url
                                                              where pls.id={idPL}");
            //SQL príkaz na uloženie dát do databázy
            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;
            conn.Open();
            //otvorenie spojenia
            MySql.Data.MySqlClient.MySqlDataReader reader = cmd.ExecuteReader();
            //zadefinovanie čítača dát, ktoré prídu s databázy
            bool firstRead = true;
            var returnList = new PlayList();
            //lokálna premenná ktorá bude vyplnená a poslaná naspäť na frontend
            while (reader.Read())
            //podmienený cyklus, ktorý bude vykonávaný pokým budú dáta čítané
            {
                if (firstRead)
                {
                    returnList.Name = reader[1].ToString();
                    returnList.Id = (int)reader[0];
                    firstRead = false;
                }
                returnList.songs.Add(new Songs()
                {
                    Song_id = reader[2].ToString(),
                    Name = reader[3].ToString(),
                    Url = reader[4].ToString(),
                    Path = reader[5].ToString()
                });
                //napĺňanie predošle zadefinovanej premennej
            }
            conn.Close();
            //zatvorenie spojenia

            return returnList;
            //vrátenie hodnoty na frontend
        }

        [HttpGet]
        [Route("PlayLists")]
        public List<PlayList> PlayLists() //zadefinovanie typu funkcie a dát, ktoreé ma očakávať od frontendu
        {
            var conn = new MySql.Data.MySqlClient.MySqlConnection("server = 169.254.59.75; database = *******; uid = *******; pwd = *******; SslMode=None");
            //connection string - prihlasovacie údaje do databázy

            var cmd = new MySql.Data.MySqlClient.MySqlCommand("select id, name from PlayLists");
            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;
            conn.Open();
            MySql.Data.MySqlClient.MySqlDataReader reader = cmd.ExecuteReader();
            List<PlayList> returnList = new List<PlayList>();
            while (reader.Read())
            {
                returnList.Add(new PlayList()
                {
                    Name = reader[1].ToString(),
                    Id = (int)reader[0],
                });
            }
            conn.Close();

            return returnList;
        }

        [HttpGet]
        [Route("AllLastPlayed")]
        public List<Songs> AllLastPlayed()
        {
            var conn = new MySql.Data.MySqlClient.MySqlConnection("server = 169.254.59.75; database = *******; uid = *******; pwd = *******; SslMode=None");
            //connection string - prihlasovacie údaje do databázy

            var cmd = new MySql.Data.MySqlClient.MySqlCommand("select name, id, url, path from LastPlayed");
            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;
            conn.Open();
            MySql.Data.MySqlClient.MySqlDataReader reader = cmd.ExecuteReader();

            List<Songs> returnList = new List<Songs>();
            while (reader.Read())
            {
                returnList.Add(new Songs()
                {
                    Song_id = reader[1].ToString(),
                    Name = reader[0].ToString(),
                    Url = reader[2].ToString(),
                    Path = reader[3].ToString()
                });
            }
            conn.Close();

            return returnList;
        }

        [HttpGet]
        [Route("LastPlayed")]
        public Songs LastPlayed(int idLP)
        {

            /*
             *******
             169.254.59.75
             *******	
             *******
             */
            User cred = new User();
            // SqlConnection conn = new SqlConnection("Data Source = 169.254.59.75, Initial Catalog = *******, User ID = *******, Password = *******"); //connection string
            var conn = new MySql.Data.MySqlClient.MySqlConnection("server = 169.254.59.75; database = *******; uid = *******; pwd = *******; SslMode=None");

            var cmd = new MySql.Data.MySqlClient.MySqlCommand($"select id, name, url, path from LastPlayed where id = {idLP}");
            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;
            conn.Open();
            MySql.Data.MySqlClient.MySqlDataReader reader = cmd.ExecuteReader();
            var temp = new Songs();
            while (reader.Read())
            {
                //Console.WriteLine(String.Format("{0}", reader[0]));
                temp.Song_id = reader[0].ToString();
                temp.Name = reader[1].ToString();
                temp.Url = reader[2].ToString();
                temp.Path = reader[3].ToString();
                //return "read";
            }
            conn.Close();
            return temp;
        }

        [HttpGet]
        [Route("DownloadAndConvert")]
        public Task<string> DownloadAndConvert(string url)
        {
            var source = @"e:/MusicSrc/YTVid/"; //place to save the file
            var youtube = YouTube.Default;
            var vid = youtube.GetVideo(url); //fetch video
            System.IO.File.WriteAllBytes(source + vid.FullName, vid.GetBytes()); //write bytes to file

            var inputFile = new MediaFile { Filename = source + vid.FullName }; // fetch downloaded mp4 file
            var outputFile = new MediaFile { Filename = $"{source + vid.FullName}.mp3" }; // create new mp3 

            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFile); // fetch the data from the file

                engine.Convert(inputFile, outputFile); // convert the file to mp3
            }

            var path = $"{source + vid.FullName}.mp3";

            return Task.FromResult(path);
        }

        [HttpGet]
        [Route("DownloadAndConvertV2")]
        public async Task<string> DownloadAndConvertV2(string videoUrl, string songName)
        {
            var conn = new MySql.Data.MySqlClient.MySqlConnection("server = 169.254.59.75; database = *******; uid = *******; pwd = *******; SslMode=None");
            //connection string - prihlasovacie údaje do databázy
            var youtube = new YoutubeClient();

            var address = $"https://www.youtube.com/watch?v={videoUrl}";
            //zadefinovanie premennej obsahujúcej link na vybrané video
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(address);
            //získanie manifestov daného videa
            var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
            //presun špecifických dát do lokálnej premennej streamInfo
            await youtube.Videos.Streams.DownloadAsync(streamInfo, $"E:/Music/mp3\\{songName}.{streamInfo.Container}");
            //stiahnutie a uloženie skladby na špecifikované miesto
            var path = $"../../mp3/{songName}.{streamInfo.Container}";//fix

            var cmd = new MySql.Data.MySqlClient.MySqlCommand($"insert into LastPlayed (name, url, path) values ( @p_name, @p_url, @p_path)");
            //SQL príkaz na uloženie dát do databázy
            cmd.Parameters.AddWithValue("p_name", songName);
            cmd.Parameters.AddWithValue("p_url", videoUrl);
            cmd.Parameters.AddWithValue("p_path", path);
            //priradenie poslaných hodnôt k dynamickým premenným posielaných do databázy
            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;
            conn.Open();
            //otvorenie spojenia
            cmd.ExecuteNonQuery();
            //realizácia SQL príkazu
            conn.Close();
            //zatvorenie spojenia

            return path;
            //vrátenie hodnoty na frontend
        }

        [HttpGet]
        [Route("CreatePlayList")]
        public bool CreatePlayList(string name) {
            var conn = new MySql.Data.MySqlClient.MySqlConnection("server = 169.254.59.75; database = *******; uid = *******; pwd = *******; SslMode=None");

            var cmd = new MySql.Data.MySqlClient.MySqlCommand($"insert into PlayLists (name) values ( @p_name )");
            cmd.Parameters.AddWithValue("p_name", name);
            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();

            return true;
        }

        [HttpGet]
        [Route("AddToPlaylist")]
        public bool AddToPlaylist(int id, string url, string path)
        {
            var conn = new MySql.Data.MySqlClient.MySqlConnection("server = 169.254.59.75; database = *******; uid = *******; pwd = *******; SslMode=None");

            var cmd = new MySql.Data.MySqlClient.MySqlCommand($"insert into PlayList (url, Id_playlist, path) values ( @p_url , @p_Id_playlist, @p_path)");
            cmd.Parameters.AddWithValue("p_url", url);
            cmd.Parameters.AddWithValue("p_Id_playlist", id);
            cmd.Parameters.AddWithValue("p_path", path);
            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();

            return true;
        }

        [HttpGet]
        [Route("AddToFavorites")]
        public bool AddToFavorites(string name, string path, string url)
        {
            var conn = new MySql.Data.MySqlClient.MySqlConnection("server = 169.254.59.75; database = *******; uid = *******; pwd = *******; SslMode=None");

            var cmd = new MySql.Data.MySqlClient.MySqlCommand($"insert into Favorites (url, name, path) values ( @p_url , @p_name, @p_path)");
            cmd.Parameters.AddWithValue("p_url", url);
            cmd.Parameters.AddWithValue("p_name", name);
            cmd.Parameters.AddWithValue("p_path", path);
            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();

            return true;
        }

        [HttpGet]
        [Route("GetFavorites")]
        public List<Songs> GetFavorites()
        {

            /*
             *******
             169.254.59.75
             *******	
             *******
             */
            // SqlConnection conn = new SqlConnection("Data Source = 169.254.59.75, Initial Catalog = *******, User ID = *******, Password = *******"); //connection string
            var conn = new MySql.Data.MySqlClient.MySqlConnection("server = 169.254.59.75; database = *******; uid = *******; pwd = *******; SslMode=None");

            var cmd = new MySql.Data.MySqlClient.MySqlCommand("select name, id, url, path from Favorites");
            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;
            conn.Open();
            MySql.Data.MySqlClient.MySqlDataReader reader = cmd.ExecuteReader();

            List<Songs> returnList = new List<Songs>();
            while (reader.Read())
            {
                returnList.Add(new Songs()
                {
                    Song_id = reader[1].ToString(),
                    Name = reader[0].ToString(),
                    Url = reader[2].ToString(),
                    Path = reader[3].ToString()
                });
            }
            conn.Close();

            return returnList;
        }
    }
}
