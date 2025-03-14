using fitmate.api.Models;

namespace fitmate.api.Services.Exercise
{
    public interface IExerciseHelper
    {
        List<EXERCISE_CATEGORY> GetExerciseCategories();
        List<EXERCISE> GetExercisesByCategoryID(int _category_id);


    }
}
