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

        public bool CheckSimilaryMatrixFileExists()
        {
            var path = Environment.CurrentDirectory + "\\DataBase\\saveSimilarityMatrix.txt";

            return File.Exists(path);
        }

        public void SaveSimilarityMatrix(float[,] matrix, int usersCount, int moviesCount)
        {
            var path = Environment.CurrentDirectory + "..\\..\\..\\..\\DataBase\\saveSimilarityMatrix.txt";

            using (var sw = File.AppendText(path))
            {
                for (var i = 0; i < usersCount; i++)
                {
                    for (int j = 0; j < moviesCount; j++)
                    {
                        if (j == moviesCount - 1) sw.Write(matrix[i, j]);
                        else sw.Write(matrix[i, j] + "::");
                    }

                    sw.WriteLine();
                }
            }
        }

        public string ReadSaveSimilarityMatrix(int userId)
        {
            var path = Environment.CurrentDirectory + "\\DataBase\\saveSimilarityMatrix.txt";
            string line;

            using (var sr = new StreamReader(path))
            {
                
                var count = 0;

                while ((line = sr.ReadLine()) != null && count != userId - 1)
                {
                    count++;                 
                }

                return line;
            }
        }
    }
}
