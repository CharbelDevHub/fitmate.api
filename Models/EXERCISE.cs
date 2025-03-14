namespace fitmate.api.Models
{
    public class EXERCISE 
    {
        public int id { get; set; }
        public string name { get; set; }
        public string focus_area { get; set; }
        public string equipment { get; set; }
        public string description { get; set; }
        public string image_url { get; set; }
        public string video_url { get; set; }
        public int exercise_category_id { get; set; }
        public bool is_deleted { get; set; }
    }
}
