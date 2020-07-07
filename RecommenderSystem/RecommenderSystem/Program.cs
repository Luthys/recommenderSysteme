using System;
using System.Collections.Generic;
using System.Linq;

namespace RecommenderSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            do
            {
                #region INIT
                // Get data from files
                var fileHelper = new FileHelper();

                var users = fileHelper.GetUsers();
                var movies = fileHelper.GetMovies();
                var ratings = fileHelper.GetRatings();

                var usersCount = users.Count();

                int targetUser;

                do
                {
                    Console.Write("Enter a userId between 1 and " + usersCount + ": ");

                    if (int.TryParse(Console.ReadLine(), out targetUser) && targetUser > 0 && targetUser <= usersCount) break;

                    Console.WriteLine("Please enter a valid value!");

                } while (true);

                #endregion

                // ----------------- Create and Save similarityMatrix -----------------
                if (!fileHelper.CheckSimilaryMatrixFileExists())
                {
                    var moviesCount = movies.Count();

                    var matrix = GetMatrix(ratings, usersCount, moviesCount);
                    var similarityMatrix = GetCosineSimilarityMatrix(matrix, usersCount, moviesCount);

                    Console.WriteLine("Processing data... This operation could take few minutes");
                    Console.WriteLine();

                    fileHelper.SaveSimilarityMatrix(similarityMatrix, usersCount, moviesCount);
                }


                var currentLine = fileHelper.ReadSaveSimilarityMatrix(targetUser);
                var parsedLine = ParseMatrixSimilarityLine(currentLine, usersCount);
                var mostSimilarUser = GetMostSimilarUser(parsedLine, targetUser);

                Console.WriteLine("Most similar user id : " + mostSimilarUser);
                Console.WriteLine();

                var similarUserMovies = GetUserMovies(mostSimilarUser, ratings, movies);
                var targetUserMovies = GetUserMovies(targetUser, ratings, movies);

                var recommenderMovies = GetMoviesNeverSeen(targetUserMovies, similarUserMovies);

                int count = 1;
                foreach (var movie in recommenderMovies)
                {
                    Console.WriteLine(count++ + ". " + movie.Value);
                }

                Console.WriteLine();
                Console.WriteLine("You have " + count + " movies in common with " + mostSimilarUser);
                Console.WriteLine();

            } while (true);            
        }

        private static int[,] GetMatrix(IEnumerable<Rating> ratings, int usersCount, int moviesCount)
        {
            var matrix = new int[usersCount, moviesCount];

            foreach(var rating in ratings)
            {
                matrix[rating.UserId - 1, rating.MovieId - 1] = rating.Rate;
            }

            return matrix;
        }

        private static float[,] GetCosineSimilarityMatrix(int[,] matrix, int userCount, int articleCount)
        {
            var similarityMatrix = new float[userCount, articleCount];
            var matrixLength = matrix.GetLength(1);

            for (int i = 0; i < matrixLength; i++)
            {
                for (int j = 0; j < matrixLength; j++)
                {
                    similarityMatrix[i, j] = CosineSimilarity(matrix, i, j);
                }
            }

            return similarityMatrix;
        }

        private static float CosineSimilarity(int[,] matrix, int user1Idx, int user2Idx)
        {
            var dotProduct = DotProduct(matrix, user1Idx, user2Idx);
            var mag1 = Magnitude(matrix, user1Idx);
            var mag2 = Magnitude(matrix, user2Idx);

            return dotProduct / (mag1 * mag2);
        }

        private static float DotProduct(int[,] matrix, int userAidx, int userBidx)
        {
            var dotProduct = 0.0f;
            var size = matrix.GetLength(1);

            for (var n = 0; n < size; n++)
            {
                dotProduct += matrix[userAidx, n] * matrix[userBidx, n];
            }

            return dotProduct;
        }

        private static float Magnitude(int[,] matrix, int userIdx)
        {
            return (float)Math.Sqrt(DotProduct(matrix, userIdx, userIdx));
        }

        private static Dictionary<int,float> ParseMatrixSimilarityLine(string line, int usersCount)
        {
            var result = new Dictionary<int, float>();

            var tmp = line.Split("::");

            for (var i = 0; i < tmp.Length; i++)
            {
                result.Add(i + 1, float.Parse(tmp[i]));
            }

            return result;
        }

        private static int GetMostSimilarUser(Dictionary<int, float> rates, int userIdTarget)
        {
            var maxValue = -1f;
            var result = -1;

            foreach (var item in rates)
            {
                if (item.Key != userIdTarget && item.Value > maxValue)
                {
                    maxValue = item.Value;
                    result = item.Key;
                }
            }

            return result;
        }

        private static Dictionary<int, string> GetUserMovies(int userId, IEnumerable<Rating> ratings, IEnumerable<Movie> movies)
        {
            var moviesTitle = new Dictionary<int, string>();

            var userRatings = ratings.Where(x => x.UserId == userId);

            foreach(var item in userRatings)
            {
                var movieId = item.MovieId;
                var movie = movies.Where(x => x.MovieId == movieId).Single();

                moviesTitle.Add(movie.MovieId, movie.MovieTitle);
            }

            return moviesTitle;
        }

        private static Dictionary<int, string> GetMoviesNeverSeen(Dictionary<int, string> targetUserMovies, Dictionary<int, string> similarUserMovies)
        {
            var result = new Dictionary<int, string>();

            var res = similarUserMovies.Keys.Except(targetUserMovies.Keys);

            foreach(var id in res)
            {
                result.Add(id, similarUserMovies.GetValueOrDefault(id));
            }


            return result;
        }
    }
}
