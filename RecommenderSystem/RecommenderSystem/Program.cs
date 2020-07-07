using System;
using System.Collections.Generic;
using System.Linq;

namespace RecommenderSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get data from files
            var fileHelper = new FileHelper();
            var users = fileHelper.GetUsers();
            var movies = fileHelper.GetMovies();
            var ratings = fileHelper.GetRatings();

            var usersCount = users.Count();
            var moviesCount = movies.Count();
            var ratingsCount = ratings.Count();

            var matrix = GetMatrix(ratings, ratingsCount, usersCount, moviesCount);

            // ----------------- Create and Save similarityMatrix -----------------
            //var similarityMatrix = GetCosineSimilarityMatrix(matrix, usersCount, moviesCount);
            //fileHelper.SaveSimilarityMatrix(similarityMatrix, usersCount, moviesCount);

            var currentLine = fileHelper.ReadSaveSimilarityMatrix(1481);
            var parsedLine = ParseMatrixSimilarityLine(currentLine, usersCount);
            var mostSimilarUser = GetMostSimilarUser(parsedLine, 1481);

            var t = true;

            // TODO
            // Trouver les recommendations d'un userId
        }

        private static int[,] GetMatrix(IEnumerable<Rating> ratings, int ratingsCount, int usersCount, int moviesCount)
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
    }
}
