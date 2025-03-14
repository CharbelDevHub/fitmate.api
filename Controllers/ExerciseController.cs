using fitmate.api.Models;
using fitmate.api.Services.Exercise;
using Microsoft.AspNetCore.Mvc;

namespace fitmate.api.Controllers
{
    [Route("api/[controller]")]
    public class ExerciseController : Controller
    {
        private readonly IExerciseHelper _exerciseHelper;

        public ExerciseController(IExerciseHelper exerciseHelper)
        {
            _exerciseHelper = exerciseHelper;
        }

        [HttpGet("GetAllCategories")]
        public List<EXERCISE_CATEGORY> GetAllCategories()
        {
            return _exerciseHelper.GetExerciseCategories();
        }

    }
}
