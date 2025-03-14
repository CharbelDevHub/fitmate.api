using Dapper;
using fitmate.api.Models;

namespace fitmate.api.Services.Exercise
{
    public class ExerciseHelper : SharedHelper, IExerciseHelper
    {
        protected string _tableName = "EXERCISE";
        public ExerciseHelper(IConfiguration configuration) : base(configuration)
        {

        }
        public List<EXERCISE_CATEGORY> GetExerciseCategories()
        {
            var result = GetAll("EXERCISE_CATEGORY");
            return result;
        }

        public List<EXERCISE> GetExercisesByCategoryID(int id)
        {
            try
            {
                string query = 
                        " SELECT * " +
                        " FROM EXERCISE WHERE EXERCISE_CATEGORY_ID = @ID ";
                OpenConnection();
                var result = Connection.Query<EXERCISE>(query, new {id});
                CloseConnection();

                return (List<EXERCISE>)result;
            }
            catch (Exception ex) { 
                return new List<EXERCISE>();
            }
        }
    }
}
