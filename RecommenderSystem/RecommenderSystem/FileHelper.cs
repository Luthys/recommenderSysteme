using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace RecommenderSystem
{
    public class FileHelper
    {
        public IEnumerable<User> GetUsers()
        {            
            var path = Environment.CurrentDirectory + "\\DataBase\\users.dat";
            return ReadFile<User>(path);
        }

        public IEnumerable<Rating> GetRatings()
        {
            var path = Environment.CurrentDirectory + "\\DataBase\\ratings.dat";
            return ReadFile<Rating>(path);
        }

        public IEnumerable<Movie> GetMovies()
        {
            var path = Environment.CurrentDirectory + "\\DataBase\\movies.dat";
            return ReadFile<Movie>(path);
        }

        private List<T> ReadFile<T>(string path, int limit = -1)
        {
            var content = new List<T>();

            using (var reader = new StreamReader(path))
            {
                var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                csv.Configuration.HasHeaderRecord = false;
                csv.Configuration.Delimiter = "::";

                int count = 0;

                while (csv.Read() && count != limit)
                {
                    var item = csv.GetRecord<T>();
                    content.Add(item);
                    count++;
                }

                return content;
            }
        }
    }
}
